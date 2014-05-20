using DBHelper.DBModels;
using Models.MobilityService.PublicTransport;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DBHelper
{
  public class DBHelper
  {
    private readonly static string DB_PATH= Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "scdb.sqlite"));

    private SQLiteConnection sqlConn;

    public DBHelper()
    {
      sqlConn = new SQLiteConnection(DB_PATH);
    }

    #region Calendar    

    /// <summary>
    /// Adds a row in the Calendar table
    /// </summary>
    /// <param name="routeID">the agency ID for the specified calendar.</param>
    /// <param name="routeID">the route ID for the specified calendar. It is the KEY field in the 'Calendars' property of the TimetableCacheUpdate model</param>
    /// <param name="calendarEntries">the fully mapped calendar. It is the VALUE field in the 'Entries' field in the VALUE property of the TimetableCacheUpdate model</param>
    /// <returns></returns>
    public bool AddCalendar(string agencyID, string routeID, Dictionary<string, string> calendarEntries)
    {
      try
      {
        sqlConn.InsertOrReplace(new Calendar()
        {
          AgencyID = agencyID,
          RouteID = routeID,
          CalendarEntries = JsonConvert.SerializeObject(calendarEntries)
        });
      }
      catch
      {
        return false;
      }
      return true;
    }

    public bool AddCalendarsForAgency(string agencyID, Dictionary<string, TimeTableCacheUpdateCalendar> calendars)
    {      
        foreach (var item in calendars)
        {
          if (!AddCalendar(agencyID, item.Key.Replace("calendar_", ""), item.Value.Entries))
            return false;
        }
        return true;
    }

    public Calendar GetCalendar(string agencyID, string routeID)
    {
      return sqlConn.Get<Calendar>(x => x.AgencyID == agencyID && x.RouteID == routeID);
    }

    public bool RemoveCalendar(string agencyID, string routeID)
    {
      return sqlConn.Delete<Calendar>(new Calendar() { AgencyID = agencyID, RouteID = routeID }) != 0;
    }

    #endregion

    #region RouteCalendar

    public bool AddRouteCalendar(string routeID, string fileHash, CompressedTimetable ct)
    {
      try
      {
        sqlConn.InsertOrReplace(new RouteCalendar()
        {
          LineHash = fileHash,
          StopsIDs = JsonConvert.SerializeObject(ct.StopIds),
          StopsNames = JsonConvert.SerializeObject(ct.Stops),
          TripsIDs = JsonConvert.SerializeObject(ct.TripIds)
        });
      }
      catch
      {
        return false;
      }
      return true;
    }

    public RouteCalendar GetRouteCalendar(string lineHash)
    {
      return sqlConn.Get<RouteCalendar>(x => x.LineHash == lineHash);
    }

    public bool RemoveRouteCalendar(string lineHash)
    {
      return sqlConn.Delete<RouteCalendar>(lineHash) != 0;
    }

    #endregion

    
    #region RouteInfo

    public bool AddRouteInformation(string agencyID, string routeID, string color )
    {
      try
      {
        sqlConn.InsertOrReplace(new RouteInfo()
        {
          AgencyID = agencyID,
          RouteID = routeID,
          Color = color
        });
      }
      catch
      {
        return false;
      }
      return true;
    }

    public List<RouteInfo> GetRouteInfo(string agencyID)
    {
      return sqlConn.Table<RouteInfo>().ToList();
    }

    #endregion

    #region RouteName

    public bool AddRouteName(string agencyID, string routeID, string name)
    {
      try
      {
        sqlConn.InsertOrReplace(new RouteName()
        {
          AgencyID = agencyID,
          RouteID = routeID,
          Name = name
        });
      }
      catch
      {
        return false;
      }
      return true;
    }

    public List<RouteName> GetRoutesNames(string agencyID)
    {
      return sqlConn.Table<RouteName>().ToList();
    }

    public RouteName GetRouteName(string agencyID, string routeID)
    {
      return sqlConn.Get<RouteName>(x => x.AgencyID == agencyID && x.RouteID == routeID);
    }

    #endregion

    #region Version

    public List<DBModels.Version> GetAllVersions()
    {
      return sqlConn.Table<DBModels.Version>().ToList();
    }

    public DBModels.Version GetVersion(string agencyID)
    {
      return sqlConn.Get<DBModels.Version>(x => x.AgencyID == agencyID);
    }

    #endregion
  }
}
