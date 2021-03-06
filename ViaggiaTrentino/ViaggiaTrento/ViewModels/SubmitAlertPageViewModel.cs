﻿using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using DBManager.DBModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.Journeys;
using Models.MobilityService.PublicTransport;
using Models.MobilityService.RealTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class SubmitAlertPageViewModel : Screen
  {
    PublicTransportLibrary ptl;
    RealTimeUpdateLibrary rtuLib;
    private Route selRoute;
    private bool isLoaded;
    private Stop selStop;
    private StopTime selST;
    private AgencyType agencyID;
    private string delay;
    private string lineName;
    private readonly INavigationService navigationService;
    private ObservableCollection<Route> routes;
    private ObservableCollection<Stop> stops;
    private ObservableCollection<StopTime> stopTimes;

    public SubmitAlertPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      rtuLib = new RealTimeUpdateLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      IsLoaded = true;
    }

    #region Properties

    public string LineName
    {
      get { return lineName; }
      set
      {
        lineName = value;
        NotifyOfPropertyChange(() => LineName);
      }
    }

    public ObservableCollection<Route> Routes
    {
      get
      {
        return routes;
      }
      set
      {
        routes = value;
        NotifyOfPropertyChange(() => Routes);
      }
    }

    public ObservableCollection<Stop> Stops
    {
      get
      {
        return stops;
      }
      set
      {
        stops = value;
        NotifyOfPropertyChange(() => Stops);
      }
    }

    public ObservableCollection<StopTime> StopTimes
    {
      get { return stopTimes; }
      set
      {
        stopTimes = value;
        NotifyOfPropertyChange(() => StopTimes);
      }
    }

    public Route SelectedRoute
    {
      get { return selRoute; }
      set
      {
        selRoute = value;
        NotifyOfPropertyChange(() => SelectedRoute);
        GetStopsForRoute(value);
      }
    }

    public Stop SelectedStop
    {
      get { return selStop; }
      set
      {
        selStop = value;
        NotifyOfPropertyChange(() => SelectedStop);

        GetStopTimesForStop(value);
      }
    }

    public StopTime SelectedStopTime
    {
      get { return selST; }
      set
      {
        selST = value;

        NotifyOfPropertyChange(() => SelectedStopTime);

        //GetStopTimesForStop(value);
      }
    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }

    public bool IsLoaded
    {
      get { return isLoaded; }
      set
      {
        isLoaded = value;
        NotifyOfPropertyChange(() => IsLoaded);
      }
    }

    #endregion

    
    protected override async void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      using (DBHelper dbh = new DBHelper())
      {
        List<RouteName> availableRoutes = dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(agencyID));
        if(agencyID == AgencyType.BolzanoVeronaRailway)
        {
          availableRoutes.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.TrentoMaleRailway)));
          availableRoutes.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.TrentoBassanoDelGrappaRailway)));
        }
        PhoneApplicationService.Current.State["routeNames"] = availableRoutes;
      }

      try
      {
        IsLoaded = false; App.LoadingPopup.Show(); 
        await Settings.RefreshToken();
        
        var results = await ptl.GetRoutes(agencyID);
        if (agencyID == AgencyType.BolzanoVeronaRailway)
        {
          results.AddRange(await ptl.GetRoutes(AgencyType.TrentoMaleRailway));
          results.AddRange(await ptl.GetRoutes(AgencyType.TrentoBassanoDelGrappaRailway));
        }
        else
          results.Sort();

        Routes = new ObservableCollection<Route>(results);
        SelectedRoute = Routes.FirstOrDefault();
      }
      finally
      {
        App.LoadingPopup.Hide(); IsLoaded = true;
      }
    }

    #region Times and stops loading

    public async void GetStopsForRoute(Route r)
    {
      try
      {
        IsLoaded = false; App.LoadingPopup.Show();
        await Settings.RefreshToken();
        Stops = new ObservableCollection<Stop>(await ptl.GetStops(r.RouteId.AgencyId, r.RouteId.Id));
        SelectedStop = Stops.FirstOrDefault();
      }
      finally
      {
        App.LoadingPopup.Hide(); IsLoaded = true;
      }
    }

    private async void GetStopTimesForStop(Stop value)
    {
      try
      {
        IsLoaded = false; App.LoadingPopup.Show(); 
        await Settings.RefreshToken();
        StopTimes = new ObservableCollection<StopTime>(await ptl.GetTimetable(selRoute.RouteId.AgencyId, selRoute.RouteId.Id, value.StopId));
        SelectedStopTime = StopTimes.FirstOrDefault();
      }
      finally
      {
        App.LoadingPopup.Hide(); IsLoaded = true;
      }
    }

    public void DelayTimeChanged(TextBox obj)
    {
      delay = obj.Text;
    }

    #endregion 

    #region Appbar

    private bool ValidateDelay()
    {
      int useless;
      StringBuilder sb = new StringBuilder();
      if (!Int32.TryParse(delay, out useless))
      {
        sb.AppendLine(string.Format("• {0}", AppResources.ValidationDelay));
      }
      string errors = sb.ToString();

      if (errors.Length > 0)
      {
        CustomMessageBox cmb = new CustomMessageBox()
        {
          Caption = AppResources.ValidationCaption,
          Message = AppResources.ValidationMessage,
          Content = sb.ToString(),
          LeftButtonContent = AppResources.ValidationBtnOk
        };
        cmb.Show();
        return false;
      }
      return true;
    }

    public async void SubmitDelay()
    {
      if (ValidateDelay())
      {
        try
        {
          long a = (DateTime.Now.Ticks - 621355968000000000) / 10000000;
          AlertDelay ad = new AlertDelay()
          {
            CreatorId = Settings.UserID,
            CreatorType = CreatorType.User,
            Note = "",
            PositionInfo = new Models.MobilityService.Journeys.Position()
            {
              Latitude = SelectedStop.Latitude.ToString(),
              Longitude = SelectedStop.Longitude.ToString(),
              Name = SelectedStop.Name,
              Stop = new StopId { Agency = agencyID, Id = SelectedStop.StopId },
              StopCode = SelectedStop.StopId
            },
            Delay = Convert.ToInt32(delay),
            Type = AlertType.Delay,
            ValidFrom = (DateTime.Now.Ticks - 621355968000000000) / 10000000
          };
          App.LoadingPopup.Show();

          await Settings.RefreshToken();
          rtuLib.SignalAlert<AlertDelay>(ad);

        }
        finally
        {
          App.LoadingPopup.Hide(); IsLoaded = true;
        }
        navigationService.UriFor<MainPageViewModel>().Navigate();
      }
    }

    #endregion
  }
}
