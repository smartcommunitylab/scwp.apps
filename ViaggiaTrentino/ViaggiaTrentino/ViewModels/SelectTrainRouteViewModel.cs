using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using System.Linq;
using DBManager.DBModels;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using TerritoryInformationServiceLibrary;
using Models.TerritoryInformationService;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Controls;
using ViaggiaTrentino.Views.Controls;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectTrainRouteViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private ObservableCollection<RouteName> routesName;

    public SelectTrainRouteViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    public ObservableCollection<RouteName> RoutesName
    {
      get
      {
        return routesName;
      }
      set
      {
        routesName = value;
        NotifyOfPropertyChange(() => RoutesName);
      }
    }
    protected override void OnActivate()
    {
      base.OnActivate();
      List<RouteName> results = new List<RouteName>();
      using (DBHelper dbh = new DBHelper())
      {
        results.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.BolzanoVeronaRailway)));
        results.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.TrentoBassanoDelGrappaRailway)));
        results.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.TrentoMaleRailway)));
      }
      RoutesName = new ObservableCollection<RouteName>(results);
    }
  }
}