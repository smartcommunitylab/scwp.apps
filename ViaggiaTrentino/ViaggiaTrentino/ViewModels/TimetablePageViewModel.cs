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

namespace ViaggiaTrentino.ViewModels
{
  public class TimetablePageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private AgencyType agencyID;
    private string routeID;
    private ObservableCollection<DBManager.DBModels.RouteName> routeNames;

    DBManager.DBModels.RouteName selectedRouteName;

    public TimetablePageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;    
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
        GetTimetableFromDB();
      }
    }

    private void GetTimetableFromDB()
    {
      using (DBHelper dbh = new DBHelper())
      {
        
        var calendar = dbh.GetCalendar(EnumConverter.ToEnumString<AgencyType>(agencyID), SelectedRouteName.RouteID).CalendarEntries;
        var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(calendar);

        string key = DateTime.Now.ToString("yyyyMMdd");
        if (results.ContainsKey(key) && results[key] != null)
        {
          string name = String.Format("{0}_{1}", SelectedRouteName.RouteID, results[key]);
          var timetable = dbh.GetRouteCalendar(name);
        }
      }
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      using (DBHelper dbh = new DBHelper())
      {
        RouteNames = new ObservableCollection<DBManager.DBModels.RouteName>(
          dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(agencyID)).Where(
            x=>(x.RouteID.StartsWith(routeID) || x.RouteID.StartsWith("0"+routeID))
          ));

        SelectedRouteName = RouteNames.FirstOrDefault();
      }
    }
  }
}