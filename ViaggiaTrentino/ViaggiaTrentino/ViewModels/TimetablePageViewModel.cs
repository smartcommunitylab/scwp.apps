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
    private string routeID;
    private ObservableCollection<DBManager.DBModels.RouteName> routeNames;
    //private CompressedTimetable timetable;

    DBManager.DBModels.RouteName selectedRouteName;

    public TimetablePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
    }

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

    public string RouteID
    {
      get { return routeID; }
      set { routeID = value; }
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
    //public CompressedTimetable Timetable
    //{
    //  get { return timetable; }
    //  set
    //  {
    //    timetable = value;
    //    NotifyOfPropertyChange(() => Timetable);
    //  }
    //}

    private void GetTimetableFromDB(string routeID)
    {
      using (DBHelper dbh = new DBHelper())
      {      
        var calendar = dbh.GetCalendar(EnumConverter.ToEnumString<AgencyType>(agencyID),routeID ).CalendarEntries;
        var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(calendar);

        string key = DateTime.Now.ToString("yyyyMMdd");
        if (results.ContainsKey(key) && results[key] != null)
        {
          string name = String.Format("{0}_{1}", routeID, results[key]);
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
      GetTimetableFromDB("05A");

      //TODO: move to the previous page
      //using (DBHelper dbh = new DBHelper())
      //{
      //  RouteNames = new ObservableCollection<DBManager.DBModels.RouteName>(
      //    dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(agencyID)).Where(
      //      x => (x.RouteID.StartsWith(routeID) || x.RouteID.StartsWith("0" + routeID))
      //    ));

      //  SelectedRouteName = RouteNames.FirstOrDefault();
      //}
    }
  }
}