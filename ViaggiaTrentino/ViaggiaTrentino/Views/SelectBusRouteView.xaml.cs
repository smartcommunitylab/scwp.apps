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
using CommonHelpers;
using Models.MobilityService;
using System.Diagnostics;
using Microsoft.Phone.Maps.Controls;

namespace ViaggiaTrentino.Views
{
  public partial class SelectBusRouteView : PhoneApplicationPage
  {
    private IEventAggregator eventAggregator;
    TerritoryInformationLibrary til;

    public SelectBusRouteView()
    {
      InitializeComponent();
      //Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      //IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      //this.eventAggregator = eventAggregator;
      //eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      //eventAggregator.Unsubscribe(this);
    }

    private void pivotRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // map pivot item
      if (pivotRoutes.SelectedIndex == 1)
      {
        double[] position = new double[2] { Settings.GPSPosition.Latitude, Settings.GPSPosition.Longitude };
        PopulateMap(position);
      }
    }

    private async void PopulateMap(double[] position)
    {
      string[] agencyIds = new string[] {
          EnumConverter.ToEnumString<AgencyType>(((SelectBusRouteViewModel)(this.DataContext)).AgencyID)
        };

      GeoCoordinate q = StopsMap.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
      GeoCoordinate w = StopsMap.ConvertViewportPointToGeoCoordinate(new Point(StopsMap.ActualWidth, StopsMap.ActualHeight));
      double meters = q.GetDistanceTo(w) / 100000;

      //Debug.WriteLine(q.GetDistanceTo(w).ToString());

      List<POIObject> stops = await ((SelectBusRouteViewModel)(this.DataContext)).RetrieveAllStops(position, meters, agencyIds);

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

    private void StopsMap_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
    {
      double[] position = new double[2] { StopsMap.Center.Latitude, StopsMap.Center.Longitude };
      PopulateMap(position);
    }
  }
}