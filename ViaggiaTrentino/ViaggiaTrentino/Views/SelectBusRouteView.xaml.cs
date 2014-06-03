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
using TerritoryInformationServiceLibrary;
using Models.TerritoryInformationService;
using ViaggiaTrentino.ViewModels;
using Microsoft.Phone.Maps.Toolkit;
using System.Device.Location;
using ViaggiaTrentino.Model;

namespace ViaggiaTrentino.Views
{
  public partial class SelectBusRouteView : PhoneApplicationPage
  {
    private IEventAggregator eventAggregator;
    TerritoryInformationLibrary til;

    public SelectBusRouteView()
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

    private async void pivotRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // map pivot item
      if (pivotRoutes.SelectedIndex == 1)
      {
        List<POIObject> stops = await ((SelectBusRouteViewModel)(this.DataContext)).RetrieveAllStops();

        var pushPins = new List<Pushpin>();        
        foreach (var stop in stops)
        {
          pushPins.Add(new Pushpin()
          {
            ContentTemplate = this.Resources["PushpinTemplate"] as DataTemplate,
            DataContext = stop,
            Tag = stop,
            GeoCoordinate = new GeoCoordinate(stop.Location[0], stop.Location[1]),
            Content = stop.Title
          });
        }

        var clusterer = new ClustersGenerator(StopsMap, pushPins, this.Resources["ClusterTemplate"] as DataTemplate);
      }
    }
  }
}