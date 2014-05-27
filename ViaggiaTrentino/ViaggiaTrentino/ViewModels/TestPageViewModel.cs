using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using MobilityServiceLibrary;
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
    ObservableCollection<Models.MobilityService.Journeys.Position> placeList;
    private readonly INavigationService navigationService;
    GoogleAddressLibrary gal;
    bool readyToShow;

    public TestPageViewModel(INavigationService navigationService)
     
    {
      viaggi = new ObservableCollection<BasicItinerary>();
      placeList = new ObservableCollection<Models.MobilityService.Journeys.Position>();
      this.navigationService = navigationService;
      gal = new GoogleAddressLibrary();
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

    public ObservableCollection<Models.MobilityService.Journeys.Position> PlaceList
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

    public async void Meh(object j)
    {
      string query = (j as AutoCompleteBox).Text;
      List<Models.MobilityService.Journeys.Position> poss = await gal.GetPositionsForAutocomplete(query);
      PlaceList = new ObservableCollection<Models.MobilityService.Journeys.Position>(poss);
    }    

    public void alert()
    {
      navigationService.UriFor<SelectAlertPageViewModel>().Navigate();

    }
  }
}
