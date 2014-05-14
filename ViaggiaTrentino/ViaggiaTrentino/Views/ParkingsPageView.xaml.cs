using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;
using MobilityServiceLibrary;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System.Device.Location;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.Views
{
  public partial class ParkingsPageView : PhoneApplicationPage, IHandle<IEnumerable<Parking>>
  {
    private IEventAggregator eventAggregator;

    public ParkingsPageView()
    {
      InitializeComponent();

      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }
    private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
    {
      var myPos = await GetMyPosition();
      if (myPos != null)
        ParkingsMap.Center = myPos;
    }

    public void Handle(IEnumerable<Parking> parkings)
    {
      ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(ParkingsMap);
      var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
      obj.ItemsSource = parkings;
    }


    private async Task<GeoCoordinate> GetMyPosition()
    {
      //Check for the user agreement in use his position. If not, method returns.
      /*if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
      {
        // The user has opted out of Location.
        return;
      }*/

      Geolocator geolocator = new Geolocator();
      geolocator.DesiredAccuracyInMeters = 50;

      try
      {
        Geoposition geoposition = await geolocator.GetGeopositionAsync(
             maximumAge: TimeSpan.FromMinutes(5),
             timeout: TimeSpan.FromSeconds(10)
            );
        return new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);

      }
      catch (Exception ex)
      {
        if ((uint)ex.HResult == 0x80004004)
        {
          MessageBox.Show(AppResources.ServiceLocationDisabled);
        }
        return null;
      }
    }
  }
}