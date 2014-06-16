using AuthenticationLibrary;
using Microsoft.Phone.Controls;
using Models.AuthorizationService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace ViaggiaTrentino
{

  public class Settings
  {
    private static IsolatedStorageSettings iss;
    private static AuthLibrary authLib;

    public static async void RefreshToken()
    {
      AppToken = await authLib.RefreshAccessToken();
    }

    static string clientId;
    public static string ClientId { get { return clientId; } }

    static string clientSecret;
    public static string ClientSecret { get { return clientSecret; } }

    static string redirectUrl;
    public static string RedirectUrl { get { return redirectUrl; } }

    static string serverUrl;
    public static string ServerUrl { get { return serverUrl; } }

    public static bool IsTokenExpired
    {
      get { return DateTime.Now > TokenExpiration; }
    }

    private static DateTime TokenExpiration
    {
      get { return (DateTime)iss["tokenExpiration"]; }
      set
      {
        iss["tokenExpiration"] = value;
        iss.Save();
      }
    }

    public static Token AppToken
    {
      get { return iss["token"] as Token; }
      set
      {
        iss["token"] = value;
        iss.Save();
        TokenExpiration = DateTime.Now + new TimeSpan(0, 0, value.ExpiresIn);
      }
    }

    public static PreferencesModel AppPreferences
    {
      get { return iss["preferences"] as PreferencesModel; }
      set { iss["preferences"] = value; iss.Save(); }
    }

    public static bool IsLogged
    {
      get { return iss["token"] != null; }
    }

    public static bool HasBeenStarted
    {
      get { return iss.Contains("hasBeenStarted"); }
    }

    public static bool LocationConsent
    {
      get { return (bool)iss["LocationConsent"]; }
      set { iss["LocationConsent"] = value; iss.Save(); }
    }

    public static GeoCoordinate GPSPosition
    {
      get { return iss["lastGPSPosition"] as GeoCoordinate; }
      set
      {
        iss["lastGPSPosition"] = value;
        iss.Save();
        Debug.WriteLine(value.Latitude + ", " + value.Longitude);
      }
    }

    public static GeoCoordinate TrentoCoordinate
    {
      get { return new GeoCoordinate(46.0697, 11.1211); }
    }

    public static string UserID
    {
      get { return iss["UserID"] as string; }
      set { iss["UserID"] = value; iss.Save(); }
    }

    public static void Initialize()
    {
      iss = IsolatedStorageSettings.ApplicationSettings;
      clientId = "52482826-891e-4ee0-9f79-9153a638d6e4";
      clientSecret = "f3ea5378-43ba-42c3-b2bf-5f7cd10b6e6e";
      redirectUrl = "http://localhost";
      serverUrl = "https://vas-dev.smartcampuslab.it/";

      authLib = new AuthLibrary(clientId, clientSecret, redirectUrl, serverUrl);
      if (!HasBeenStarted)
      {
        iss["token"] = iss["lastGPSPosition"] = null;
        iss["tokenExpiration"] = DateTime.Now;
        iss["LocationConsent"] = false;
        iss.Save();

        AppPreferences = new PreferencesModel()
        {
          PreferredRoute = new PreferredRoutePreferences()
          {
            Fastest = true,
            FewestChanges = false,
            LeastWalking = false
          },
          Transportation = new TransportationPreferences()
          {
            Transit = true,
            Bike = false,
            Car = false,
            SharedBike = false,
            SharedCar = false,
            Walking = false
          }
        };
      }
      //ClearHasBeenStarted();
    }

    public static void ForceSave()
    {
      iss.Save();
    }
    public static void ClearHasBeenStarted()
    {
      iss.Remove("hasBeenStarted");
    }

    public static void LaunchGPS()
    {
      Geolocator geolocator = new Geolocator()
      {
        DesiredAccuracy = PositionAccuracy.High
      };

      if (!HasBeenStarted)
      {
        iss["hasBeenStarted"] = null;
        iss.Save();

        if (geolocator.LocationStatus == PositionStatus.Disabled)
        {
          #region MessageBox for System GPS Location
          CustomMessageBox messageBox = new CustomMessageBox()
          {
            Caption = Resources.AppResources.SystemLocationDisabledCaption,
            Message = Resources.AppResources.SystemLocationDisabledMessage,
            LeftButtonContent = Resources.AppResources.MessageBoxGoToSettings,
            RightButtonContent = Resources.AppResources.MessageBoxCancel
          };

          messageBox.Dismissed += async (s1, e1) =>
          {
            switch (e1.Result)
            {
              case CustomMessageBoxResult.LeftButton:
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:")); break;
              default: break;
            }
          };
          messageBox.Show();
          #endregion
          LocationConsent = false;
        }
        else
        {
          #region MessageBox for App GPS Location
          CustomMessageBox messageBox = new CustomMessageBox()
          {
            Caption = Resources.AppResources.AppLocationDisabledCaption,
            Message = Resources.AppResources.AppLocationDisabledMessage,
            LeftButtonContent = Resources.AppResources.MessageBoxAllow,
            RightButtonContent = Resources.AppResources.MessageBoxCancel
          };

          messageBox.Dismissed += (s1, e1) =>
          {
            switch (e1.Result)
            {
              case CustomMessageBoxResult.LeftButton:
                LocationConsent = true; break;
              default:
                LocationConsent = false; break;
            }
            RetrieveGPSPosition(geolocator);
          };

          messageBox.Show();
          #endregion
        }
      }
      else
      {
        RetrieveGPSPosition(geolocator);
      }
    }

    private async static void RetrieveGPSPosition(Geolocator geolocator)
    {
      if (!LocationConsent || geolocator.LocationStatus == PositionStatus.Disabled)
      {
        Settings.GPSPosition = Settings.TrentoCoordinate;
        return;
      }

      IAsyncOperation<Geoposition> locationTask = null;
      try
      {
        locationTask = geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(15));
        Geoposition position = await locationTask;
        Settings.GPSPosition = position.Coordinate.ToGeoCoordinate();
      }
      catch
      {
        Settings.GPSPosition = Settings.TrentoCoordinate;
      }
      finally
      {
        if (locationTask != null)
        {
          if (locationTask.Status == AsyncStatus.Started)
            locationTask.Cancel();

          locationTask.Close();
        }
      }
    }
  }
}
