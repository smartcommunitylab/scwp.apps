using Models.MobilityService;
using System.Device.Location;

namespace ViaggiaTrentino
{
  // THIS ONE IS ROVERETO
  public partial class Settings
  {
    public static GeoCoordinate DefaultCityCoordinate
    {
      get { return new GeoCoordinate(45.89064810, 11.03987030); }
    }

    public static AgencyType ParkingAgencyId
    {
      get { return AgencyType.ComuneDiRovereto; }
    }    
    
    public static void AppSpecificInitialize()
    {
      clientId = "0a9531e5-6c95-4291-848f-1ed8a5914379";
      clientSecret = "efc438a8-28ea-457c-9f4c-9f6b69ec4499";
      redirectUrl = "http://localhost";
      serverUrl = "https://vas-dev.smartcampuslab.it/";
    }
  }
}
