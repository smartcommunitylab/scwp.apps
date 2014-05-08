using Caliburn.Micro;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class ParkingsPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    ObservableCollection<Parking> parkings;

    public ParkingsPageViewModel(INavigationService navigationService)
    {
      parkings = new ObservableCollection<Parking>();
      this.navigationService = navigationService;

      /*Parking pa = new Parking()
      {
        Description = "parcheggio descrizione",
        Name = "Garage autorimessa europa",
        SlotsAvailable = 633,
        SlotsTotal = 900,
        Position = new double[] { 46.0676, 11.1247 }
      };
      parkings.Add(pa);
      Parking pa2 = new Parking()
      {
        Description = "parcheggio descrizione",
        Name = "Garage autorimessa europa",
        SlotsAvailable = 900,
        SlotsTotal = 900,
        Position = new double[] { 46.0676, 11.1247 }
      };
      parkings.Add(pa2);*/
    }

    public ObservableCollection<Parking> Parkings
    {
      get { return parkings; }
      set
      {
        parkings = value;
        NotifyOfPropertyChange(() => Parkings);
      }
    }
  }
}
