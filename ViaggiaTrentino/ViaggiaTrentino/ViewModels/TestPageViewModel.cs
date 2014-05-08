using Caliburn.Micro;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class TestPageViewModel : PropertyChangedBase
  {
    ObservableCollection<BasicItinerary> viaggi;

    public TestPageViewModel()
    {
      viaggi = new ObservableCollection<BasicItinerary>();
    }

    public ObservableCollection<BasicItinerary> Viaggi
    {
       get { return viaggi; }
       set {
        viaggi = value;
        NotifyOfPropertyChange(() => Viaggi);
      }
    }

    public void Popola()
    {
      List<Leg> gambe = new List<Leg>();
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "BICYCLE"}});
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "CAR"}});
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "BUS"}});
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "BUS"}});
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "CAR"}});
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "BICYCLE"}});
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = "BICYCLE"}});

      Itinerary it = new Itinerary()
      {
        EndTime = 342341232,
        StartTime = 342341232,
        Legs = gambe
      };

      BasicItinerary bi = new BasicItinerary()
      {
        Data = it,
        Monitor = true,
        Name = "Viaggio cit-lavoro",
        OriginalFrom = new Models.MobilityService.Journeys.Position() { Name = "Trento Sud, Trento, 38123, Italia" },
        OriginalTo = new Models.MobilityService.Journeys.Position() { Name = "sayService, Via alla Cascata, Trento, 38100, Italia" },

      };
      Viaggi.Add(bi);

    }
  }
}
