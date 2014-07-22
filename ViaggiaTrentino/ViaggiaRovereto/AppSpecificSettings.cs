﻿using Models.MobilityService;
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
      clientId = "52482826-891e-4ee0-9f79-9153a638d6e4";
      clientSecret = "f3ea5378-43ba-42c3-b2bf-5f7cd10b6e6e";
      redirectUrl = "http://localhost";
      serverUrl = "https://vas-dev.smartcampuslab.it/";
    }
  }
}
