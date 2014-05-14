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

    private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
    {
      GeoCoordinateWatcher geolocator = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
      geolocator.StatusChanged += geolocator_StatusChanged;
      geolocator.Start();
    }

    public void Handle(IEnumerable<Parking> parkings)
    {
      ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(ParkingsMap);
      var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
      obj.ItemsSource = parkings;
    }

    void geolocator_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
    {
      if (e.Status == GeoPositionStatus.Ready)
      {
        ParkingsMap.Center = (sender as GeoCoordinateWatcher).Position.Location;
        (sender as GeoCoordinateWatcher).Stop();
      }
    }
  }
}