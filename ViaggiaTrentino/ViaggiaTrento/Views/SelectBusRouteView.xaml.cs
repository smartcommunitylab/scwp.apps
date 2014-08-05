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
using Models.MobilityService.PublicTransport;

namespace ViaggiaTrentino.Views
{
  public partial class SelectBusRouteView : PhoneApplicationPage
  {
    public SelectBusRouteView()
    {
      InitializeComponent();
    }

    private void pivotRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // map pivot item
      if (pivotRoutes.SelectedIndex == 1)
      {
        double[] position = new double[2] { Settings.DefaultCityCoordinate.Latitude, Settings.DefaultCityCoordinate.Longitude };
        PopulateMap(position);
      }
    }

    private async void PopulateMap(double[] position)
    {
      //string[] agencyIds = new string[] {
      //    EnumConverter.ToEnumString<AgencyType>(((SelectBusRouteViewModel)(this.DataContext)).AgencyID)
      //  };

      GeoCoordinate q = StopsMap.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
      GeoCoordinate w = StopsMap.ConvertViewportPointToGeoCoordinate(new Point(StopsMap.ActualWidth, StopsMap.ActualHeight));
      double meters = q.GetDistanceTo(w) / 100000;

      //Debug.WriteLine(q.GetDistanceTo(w).ToString());

      List<Stop> stops = await ((SelectBusRouteViewModel)(this.DataContext)).RetrieveAllStops(position, meters);

      var pushPins = new List<Pushpin>();
      foreach (var stop in stops)
      {
        //stop.StopId = EnumConverter.ToEnumString<AgencyType>(((SelectBusRouteViewModel)(this.DataContext)).AgencyID) +"$$$"+stop.StopId;
        pushPins.Add(new Pushpin()
        {
          ContentTemplate = this.Resources["PushpinTemplate"] as DataTemplate,
          DataContext = stop,
          Tag = stop,
          GeoCoordinate = new GeoCoordinate(stop.Latitude, stop.Longitude),
          Content = stop.Name
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