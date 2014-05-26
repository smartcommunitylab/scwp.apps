using Caliburn.Micro;
using CommonHelpers;
using System.Linq;
using System.Collections.Generic;
using DBManager;
using MobilityServiceLibrary;
using Models.MobilityService;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System;
using Models.MobilityService.PublicTransport;
using System.Collections;
using System.Diagnostics;

namespace ViaggiaTrentino.ViewModels
{
  public class TimetablePageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private AgencyType agencyID;
    private string routeIDWitDirection, nameID, description, color;
    private ObservableCollection<DBManager.DBModels.RouteName> routeNames;
    DBManager.DBModels.RouteName selectedRouteName;


    public TimetablePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
    }

    #region Properties
    public ObservableCollection<DBManager.DBModels.RouteName> RouteNames
    {
      get { return routeNames; }
      set
      {
        routeNames = value;
        NotifyOfPropertyChange(() => RouteNames);
      }
    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }

    public string RouteIDWitDirection
    {
      get { return routeIDWitDirection; }
      set { routeIDWitDirection = value; }
    }

    public string NameID
    {
      get { return nameID; }
      set
      {
        nameID = value;
      }
    }

    public string Description
    {
      get { return description; }
      set
      {
        description = value;
      }
    }

    public string Color
    {
      get { return color; }
      set
      {
        color = value;
      }
    }

    public string RouteName
    {
      get { return string.Format("{0} - {1}", NameID, Description); }
    }

    public DBManager.DBModels.RouteName SelectedRouteName
    {
      get { return selectedRouteName; }
      set
      {
        selectedRouteName = value;
        NotifyOfPropertyChange(() => SelectedRouteName);
      }
    }

    #endregion

    private void GetTimetableFromDB()
    {
      using (DBHelper dbh = new DBHelper())
      {
        var calendar = dbh.GetCalendar(EnumConverter.ToEnumString<AgencyType>(agencyID), routeIDWitDirection).CalendarEntries;
        var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(calendar);

        string key = DateTime.Now.ToString("yyyyMMdd");
        if (results.ContainsKey(key) && results[key] != null)
        {
          string name = String.Format("{0}_{1}", routeIDWitDirection, results[key]);
          var timetable = dbh.GetRouteCalendar(name);
          eventAggregator.Publish(new CompressedTimetable()
          {
            CompressedTimes = timetable.Times,
            TripIds = JsonConvert.DeserializeObject<List<string>>(timetable.TripsIDs),
            Stops = JsonConvert.DeserializeObject<List<string>>(timetable.StopsNames),
            StopIds = JsonConvert.DeserializeObject<List<string>>(timetable.StopsIDs),
          });
        }
      }
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      GetTimetableFromDB();
    }
  }
}