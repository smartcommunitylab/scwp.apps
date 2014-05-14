using AuthenticationLibrary;
using Models.AuthorizationService;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      get { return iss.Contains("token"); }
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

    public static void Initialize()
    {
      iss = IsolatedStorageSettings.ApplicationSettings;
      //ClearHasBeenStarted();
      clientId = "52482826-891e-4ee0-9f79-9153a638d6e4";
      clientSecret = "f3ea5378-43ba-42c3-b2bf-5f7cd10b6e6e";
      redirectUrl = "http://localhost";

      authLib = new AuthLibrary(clientId, clientSecret, redirectUrl);
      if (!HasBeenStarted)
      {
        iss["hasBeenStarted"] = iss["token"] = null;
        iss["tokenExpiration"] = DateTime.Now;
        iss["LocationConsent"] = true;
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
    }

    public static void ForceSave()
    {
      iss.Save();
    }
    public static void ClearHasBeenStarted()
    {
      iss.Remove("hasBeenStarted");
    }
  }
}
