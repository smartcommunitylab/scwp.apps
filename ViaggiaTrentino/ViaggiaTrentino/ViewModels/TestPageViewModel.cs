using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class TestPageViewModel : Screen
  {
    ObservableCollection<BasicItinerary> viaggi;
    ObservableCollection<string> placeList;
    private readonly INavigationService navigationService;
    GeocodeQuery gq;
    bool readyToShow;

    public TestPageViewModel(INavigationService navigationService)
     
    {
      viaggi = new ObservableCollection<BasicItinerary>();
      placeList = new ObservableCollection<string>();
      this.navigationService = navigationService;
      gq = new GeocodeQuery();
      readyToShow = false;
    }

    public ObservableCollection<BasicItinerary> Viaggi
    {
       get { return viaggi; }
       set {
        viaggi = value;
        NotifyOfPropertyChange(() => Viaggi);
      }
    }

    public ObservableCollection<string> PlaceList
    {
      get { return placeList; }
      set
      {
        placeList = value;
        NotifyOfPropertyChange(() => PlaceList);
      }
    }

    public bool LoadedData
    {
      get { return readyToShow; }

      set
      {
        readyToShow = value;
        NotifyOfPropertyChange(() => LoadedData);
      }
    
    }

    public void planJourney()
    {
      navigationService.UriFor<MonitorJourneyViewModel>().Navigate();

    }

    public void Popola()
    {
      List<Leg> gambe = new List<Leg>();
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = TransportType.Bicycle}});
      gambe.Add(new Leg() { TransportInfo = new Transport() { Type = TransportType.Car } });
      gambe.Add(new Leg(){TransportInfo = new Transport(){Type = TransportType.Bus }});
      gambe.Add(new Leg() { TransportInfo = new Transport() { Type = TransportType.Bus } });

      gambe.Add(new Leg() { TransportInfo = new Transport() { Type = TransportType.Car } });
      gambe.Add(new Leg() { TransportInfo = new Transport() { Type = TransportType.Bicycle } }); 
      gambe.Add(new Leg() { TransportInfo = new Transport() { Type = TransportType.Bicycle } });

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

    public void Meh(object j)
    {
      if (!gq.IsBusy)
      {
        LoadedData = false;
        gq.SearchTerm = (j as AutoCompleteBox).Text;
        //gq.GeoCoordinate = GPSPos;
        gq.GeoCoordinate = new GeoCoordinate(0, 0);
        gq.MaxResultCount = 10;
        gq.QueryCompleted += gq_QueryCompleted;
        gq.QueryAsync();
      }
      else gq.CancelAsync();
    }

    void gq_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
    {
      try
      {
        var a = e.Result.Select(x => x.Information.Address.Street).ToList();

        PlaceList = new ObservableCollection<string>(a);
        LoadedData = true;
      }
      catch { }
    }

    public void alert()
    {
      navigationService.UriFor<SelectAlertPageViewModel>().Navigate();

    }
  }
}
