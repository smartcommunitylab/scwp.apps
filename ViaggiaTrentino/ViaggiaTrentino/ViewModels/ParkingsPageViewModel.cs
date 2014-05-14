using System;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;
using System.Windows;
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
      //IsPopupShown = false;

      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;

      //START only for debugging purpose
      parkings = new ObservableCollection<Parking>();
      Parking pa = new Parking()
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
        Name = "Garage autorimessa africa",
        SlotsAvailable = 900,
        SlotsTotal = 900,
        Position = new double[] { 46.0976, 11.1550 }
      };
      parkings.Add(pa2);
      //END only for debugging purpose
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      eventAggregator.Publish(parkings);
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

    public void TappedPushPin( Parking pp )
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
