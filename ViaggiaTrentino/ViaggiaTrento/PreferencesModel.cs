using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino
{
  public class PreferencesModel
  {
    public TransportationPreferences Transportation { get; set; }
    public PreferredRoutePreferences PreferredRoute { get; set; }

    public PreferencesModel Clone()
    {
      PreferencesModel pm = new PreferencesModel()
      {
        Transportation = new TransportationPreferences()
        {
          Bike = this.Transportation.Bike,
          Car = this.Transportation.Car,
          SharedBike = this.Transportation.SharedBike,
          SharedCar = this.Transportation.SharedCar,
          Transit = this.Transportation.Transit,
          Walking = this.Transportation.Walking
        },
        PreferredRoute = new PreferredRoutePreferences()
        {
          Fastest = this.PreferredRoute.Fastest,
          FewestChanges = this.PreferredRoute.FewestChanges,
          LeastWalking = this.PreferredRoute.LeastWalking,
        }
      };
      return pm;
    }

  }

  public class PreferredRoutePreferences
  {
    public bool Fastest { get; set; }
    public bool FewestChanges { get; set; }
    public bool LeastWalking { get; set; }
  }

  public class TransportationPreferences
  {
    public bool Transit { get; set; }
    public bool Walking { get; set; }
    public bool Car { get; set; }
    public bool Bike { get; set; }
    public bool SharedCar { get; set; }
    public bool SharedBike { get; set; }
  }
}
