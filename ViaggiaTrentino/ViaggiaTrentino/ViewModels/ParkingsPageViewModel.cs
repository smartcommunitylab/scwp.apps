using System;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;
using System.Windows;
using MobilityServiceLibrary;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Views.Controls;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;

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

      if (Settings.IsTokenExpired)
        Settings.RefreshToken();
      publicTransLib = new PublicTransportLibrary(Settings.AppToken.AccessToken);
    }

    protected async override void OnActivate()
    {
      base.OnActivate();
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
      MessagePrompt mp = new MessagePrompt();
      mp.Title = "parking";
      mp.Body = new ParkingPopupView() { DataContext = pp };
      mp.ActionPopUpButtons.Clear();
      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.HorizontalAlignment = HorizontalAlignment.Center;
      mp.Show();
    }

    
  }
}
