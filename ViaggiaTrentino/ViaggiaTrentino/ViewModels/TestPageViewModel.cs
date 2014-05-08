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
        EndTime = 342342,
        StartTime = 342142,
      };

      BasicItinerary bi = new BasicItinerary()
      {
        Data = it,
        Monitor = true,
        Name = "fakes",
        OriginalFrom = new Position() { Name = "genuinefake" },
        OriginalTo = new Position() { Name = "authenticfake" },

      };
      Viaggi.Add(bi);

    }
  }
}
