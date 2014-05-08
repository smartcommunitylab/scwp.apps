using Caliburn.Micro;
using Models.MobilityService.Journeys;
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
      Itinerary it = new Itinerary()
      {
        EndTime = 342341232,
        StartTime = 342341232,
      };

      BasicItinerary bi = new BasicItinerary()
      {
        Data = it,
        Monitor = true,
        Name = "Viaggio cit-lavoro",
        OriginalFrom = new Position() { Name = "Trento Sud, Trento, 38123, Italia" },
        OriginalTo = new Position() { Name = "sayService, Via alla Cascata, Trento, 38100, Italia" },

      };
      Viaggi.Add(bi);

    }
  }
}
