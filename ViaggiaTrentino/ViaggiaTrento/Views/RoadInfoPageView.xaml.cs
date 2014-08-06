using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using ViaggiaTrentino.Model;
using ViaggiaTrentino.ViewModels;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.Views
{
  public partial class RoadInfoPageView : PhoneApplicationPage, IHandle<List<AlertRoad>>
  {
    private IEventAggregator eventAggregator;

    public RoadInfoPageView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      this.eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      this.eventAggregator.Unsubscribe(this);
    }

    public void Handle(List<AlertRoad> message)
    {
      var pushPins = new List<Pushpin>();
      foreach (var decree in message)
      {
        pushPins.Add(new Pushpin()
        {
          ContentTemplate = this.Resources["PushpinTemplate"] as DataTemplate,
          DataContext = decree,
          Tag = decree,
          GeoCoordinate = new GeoCoordinate(Convert.ToDouble(decree.RoadInfo.Latitude.Replace('.', ',')), Convert.ToDouble(decree.RoadInfo.Longitude.Replace('.', ','))),
          Content = decree.RoadInfo.Street
        }
        );
      }   
      var clusterer = new ClustersGenerator(DecreesMap, pushPins, this.Resources["ClusterTemplate"] as DataTemplate);
    }

    private void SingleDecreesView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      AlertRoad p = (sender as SingleDecreesView).DataContext as AlertRoad;
      DecreesMap.Center = new GeoCoordinate(Convert.ToDouble(p.RoadInfo.Latitude.Replace('.', ',')), Convert.ToDouble(p.RoadInfo.Longitude.Replace('.', ',')));
      DecreesMap.ZoomLevel = 17;
      pivotContainer.SelectedItem = pivotMap;
    }
  }
}