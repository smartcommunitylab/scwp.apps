﻿using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectBusRouteViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private AgencyType agencyID;

    ObservableCollection<DBManager.DBModels.RouteInfo> routesName;
    PublicTransportLibrary ptl;

    public SelectBusRouteViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);

    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }
    public ObservableCollection<DBManager.DBModels.RouteInfo> RoutesName
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

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      using (DBHelper dbh = new DBHelper())
      {
        RoutesName = new ObservableCollection<DBManager.DBModels.RouteInfo>(dbh.GetRouteInfo(EnumConverter.ToEnumString<AgencyType>(AgencyID)));
      }
    }
  }
}