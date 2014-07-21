using Models.MobilityService;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino
{
  // THIS ONE IS TRENTO
  public partial class Settings
  {
    public static GeoCoordinate DefaultCityCoordinate
    {
      get { return new GeoCoordinate(46.0697, 11.1211); }
    }

    public static AgencyType ParkingAgencyId
    {
      get { return AgencyType.ComuneDiTrento; }
    }

    public static void AppSpecificInitialize()
    {
      clientId = "52482826-891e-4ee0-9f79-9153a638d6e4";
      clientSecret = "f3ea5378-43ba-42c3-b2bf-5f7cd10b6e6e";
      redirectUrl = "http://localhost";
      serverUrl = "https://vas-dev.smartcampuslab.it/";
    }

  }
}
