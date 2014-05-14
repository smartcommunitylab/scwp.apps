using System;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;
using System.Windows;
using MobilityServiceLibrary;

namespace ViaggiaTrentino.ViewModels
{
  public class ParkingsPageViewModel : Screen
  {
    private readonly IEventAggregator eventAggregator;
    private readonly INavigationService navigationService;
    ObservableCollection<Parking> parkings;
    PublicTransportLibrary publicTransLib;

    public ParkingsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      publicTransLib = new PublicTransportLibrary(Settings.AppToken.AccessToken);

    }

    protected async override void OnActivate()
    {
      base.OnActivate();

      //parkings = new ObservableCollection<Parking>();
      //Parking pa = new Parking()
      // {
      //   Description = "parcheggio descrizione",
      //   Name = "Garage autorimessa europa",
      //   SlotsAvailable = 633,
      //   SlotsTotal = 900,
      //   Position = new double[] { 46.0676, 11.1247 }
      // };
      //parkings.Add(pa);
      //Parking pa2 = new Parking()
      //{
      //  Description = "parcheggio descrizione",
      //  Name = "Garage autorimessa africa",
      //  SlotsAvailable = 900,
      //  SlotsTotal = 900,
      //  Position = new double[] { 46.0976, 11.1550 }
      //};
      //parkings.Add(pa2);
      Parkings = new ObservableCollection<Parking>(await publicTransLib.GetParkingsByAgency(Models.MobilityService.AgencyType.ComuneDiTrento));
      eventAggregator.Publish(Parkings);
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

    public void TappedPushPin(Parking pp)
    {

      MessageBox.Show(pp.ToString());
    }
  }
}