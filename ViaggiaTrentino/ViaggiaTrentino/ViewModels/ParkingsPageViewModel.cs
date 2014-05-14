using System;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;
using System.Windows;
using MobilityServiceLibrary;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Views.Controls;
using System.Windows.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class ParkingsPageViewModel : Screen
  {
    private readonly IEventAggregator eventAggregator;
    private readonly INavigationService navigationService;
    ObservableCollection<Parking> parkings;
    PublicTransportLibrary publicTransLib;
    Parking selParking;
    bool isPopUp;


    public Parking SelectedParking
    {
      get
      {
        return selParking;
      }
      set
      {
        selParking = value;
        NotifyOfPropertyChange(() => SelectedParking);
      }
    }

    public bool IsPopupShown
    {
      get
      {
        return isPopUp;
      }
      set
      {
        isPopUp = value;
        NotifyOfPropertyChange(() => IsPopupShown);
      }
    }

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
      SelectedParking = pp;      
      IsPopupShown = true;

    }

    public void ClosePopup()
    {
      IsPopupShown = false;
    }

    public void GetDirectionsBtn()
    {
      MessageBox.Show("navigo verso " + selParking.Name);
    }
  }
}
