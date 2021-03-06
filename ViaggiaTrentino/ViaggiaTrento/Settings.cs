﻿using AuthenticationLibrary;
using Microsoft.Phone.Controls;
using Models.AuthorizationService;
using System;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Linq;
using ViaggiaTrentino.Helpers;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace ViaggiaTrentino
{

  public partial class Settings
  {
    private static IsolatedStorageSettings iss;
    
    private static AuthLibrary authLib;

    // bool return is just to have something on which to use a wait when using it
    public static async Task<bool> RefreshToken(bool overrideCheck = false)
    {
      try
      {
        if(IsLogged && (overrideCheck || IsTokenExpired))
          AppToken = await authLib.RefreshAccessToken();
      }
      catch
      {
        return false;
      }
      return true;
    }

    public static DateTime LastNetworkError { get; set; }

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

    public static string DBVersion
    {
      get { return iss["dbVersion"] as string; }
      set
      {
        iss["dbVersion"] = value;
        iss.Save();
      }
    }

    public static string AppVersion
    {
      get { return XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value; }
    }

    public static Token AppToken
    {
      get { return iss["token"] as Token; }
      set
      {
        iss["token"] = value;
        iss.Save();
        if (IsLogged)
        {
#pragma warning disable 4014
          new TimeTableCacheHelper().UpdateCachedCalendars();
#pragma warning restore 4014

          TokenExpiration = DateTime.Now + new TimeSpan(0, 0, value.ExpiresIn);
        }
      }
    }

    public static PreferencesModel AppPreferences
    {
      get { return iss["preferences"] as PreferencesModel; }
      set { iss["preferences"] = value; iss.Save(); }
    }

    public static bool IsLogged
    {
      get { return iss["token"] != null && !string.IsNullOrWhiteSpace((iss["token"] as Token).AccessToken); }
    }

    public static bool HasBeenStarted
    {
      get { return iss.Contains("hasBeenStarted"); }
    }

    public static bool IsTourAlreadyShown
    {
      get { return(bool)iss["isTourAlreadyShown"]; }
      set { iss["isTourAlreadyShown"] = value; iss.Save(); }

    }

    public static bool LocationConsent
    {
      get { return (bool)iss["LocationConsent"]; }
      set { iss["LocationConsent"] = value; iss.Save(); }
    }

    public static bool FeedbackEnabled
    {
      get { return (bool)iss["feedbackEnabled"]; }
      set { iss["feedbackEnabled"] = value; iss.Save(); }
    }

    public static GeoCoordinate GPSPosition
    {
      get { return iss["lastGPSPosition"] as GeoCoordinate; }
      set
      {
        iss["lastGPSPosition"] = value;
        iss.Save();
      }
    }

    public static string UserID
    {
      get { return iss["UserID"] as string; }
      set { iss["UserID"] = value; iss.Save(); }
    }

    public static void Initialize()
    {
      iss = IsolatedStorageSettings.ApplicationSettings;
      AppSpecificInitialize();
      
      if (!HasBeenStarted)
      {
        iss["token"] = iss["lastGPSPosition"] = null;
        iss["tokenExpiration"] = DateTime.Now;
        iss["isTourAlreadyShown"] = false;
        iss["LocationConsent"] = false;
        iss["feedbackEnabled"] = true;
        iss["dbVersion"] = AppVersion;
        iss.Save();

        // set to 1970 to make sure that if an exception happens in the first five 
        // minutes of life of the app it does not goes unnoticed
        LastNetworkError = new DateTime(1970, 1, 1);

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
      if(IsLogged)
          authLib = new AuthLibrary(Settings.ClientId, Settings.ClientSecret, Settings.RedirectUrl,
                                    Settings.AppToken.AccessToken, Settings.AppToken.RefreshToken, Settings.ServerUrl);        
      else
        authLib = new AuthLibrary(clientId, clientSecret, redirectUrl, serverUrl);

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

    public string ApplicationTitle
    {
      get
      {
        System.Reflection.AssemblyTitleAttribute ata =
            System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(
            typeof(System.Reflection.AssemblyTitleAttribute), false)[0] as System.Reflection.AssemblyTitleAttribute;
        return ata.Title;
      }
    }

    private async static void RetrieveGPSPosition(Geolocator geolocator)
    {
      if (!LocationConsent || geolocator.LocationStatus == PositionStatus.Disabled)
      {
        Settings.GPSPosition = Settings.DefaultCityCoordinate;
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
        Settings.GPSPosition = Settings.DefaultCityCoordinate;
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
