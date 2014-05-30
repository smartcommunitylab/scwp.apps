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
        til = new TerritoryInformationLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);

        string[] agencyIds = {"12","16"};

        Dictionary<string,object> criteria = new Dictionary<string,object>();
        criteria.Add("source", "smartplanner-transitstops");
        criteria.Add("customData.agencyId", agencyIds);
        var results = await til.ReadPlaces(new FilterObject()
        {
          SkipFirstElements = 0,
          NumberOfResults = -1,
          Categories = new List<string>() {"Mobility"},
          MongoFilters = criteria,
          Coordinates = new double[2] {Settings.GPSPosition.Latitude, Settings.GPSPosition.Longitude},
        });
      }
    }
  }
}