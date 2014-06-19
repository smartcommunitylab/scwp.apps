using DBManager.DBModels;
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

namespace DBManager
{
  public class DBHelper : IDisposable
  {
    public readonly static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "scdb.sqlite"));

    private SQLiteConnection sqlConn;

    public DBHelper()
    {
      sqlConn = new SQLiteConnection(DB_PATH);
    }

    /// <summary>
    /// Destroys the object, allows for the usage of the Helper in the
    /// (using DBHelper dbhName ...) format
    /// </summary>
    public void Dispose()
    {
      if (sqlConn != null)
      {
        sqlConn.Close();
        sqlConn = null;
      }
    }

    /*
     * Database operations that interact with the Calendar table
     */

    #region Calendar

    /// <summary>
    /// Adds a row in the Calendar table
    /// </summary>
    /// <param name="agencyID">the agency ID for the specified calendar.</param>
    /// <param name="routeID">the route ID for the specified calendar. It is the KEY field in the 'Calendars' property of the TimetableCacheUpdate model</param>
    /// <param name="calendarEntries">the fully mapped calendar. It is the VALUE field in the 'Entries' field in the VALUE property of the TimetableCacheUpdate model</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
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

    /// <summary>
    /// Adds multiple rows (and therefore possibly routes) in the Calendar table for a specific Agency
    /// </summary>
    /// <param name="agencyID">the agency ID for the specified calendar.</param>
    /// <param name="calendars">A dictionary having the routeID as key in the format "calendar_{routeID}" and a 
    /// TimeTableCachUpdateCalendar object, with the Entries field containing the actual calendar reference entries</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool AddCalendarsForAgency(string agencyID, Dictionary<string, TimeTableCacheUpdateCalendar> calendars)
    {
      foreach (var item in calendars)
      {
        if (!AddCalendar(agencyID, item.Key.Replace("calendar_", ""), item.Value.Entries))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Retrieves a specific calendar from the database
    /// </summary>
    /// <param name="agencyID">the agency ID for the specified calendar.</param>
    /// <param name="routeID">the route ID for the specified calendar.</param>    
    /// <returns>The requested Calendar object matching given agency and route</returns>
    public Calendar GetCalendar(string agencyID, string routeID)
    {
      return sqlConn.Get<Calendar>(x => x.AgencyID == agencyID && x.RouteID == routeID);
    }

    /// <summary>
    /// Removes a specific calendar from the database
    /// </summary>
    /// <param name="agencyID">the agency ID for the specified calendar.</param>
    /// <param name="routeID">the route ID for the specified calendar.</param>    
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool RemoveCalendar(string agencyID, string routeID)
    {
      SQLiteCommand sCmd = sqlConn.CreateCommand("DELETE FROM Calendar WHERE AgencyID = ? AND RouteID = ?",
                                                agencyID, routeID);
      return sCmd.ExecuteNonQuery() != 0;
      //return sqlConn.Delete<Calendar>(new Calendar() { AgencyID = agencyID, RouteID = routeID }) != 0;
    }

    #endregion

    /*
     * Database operations that interact with the RouteCalendar table 
     * (that is, the table containing the actual timetables, stored as JSON arrays)
     */

    #region RouteCalendar

    /// <summary>
    /// Adds a RouteCalendar to the database
    /// </summary>
    /// <param name="routeID">the unique identifier for a specific route</param>
    /// <param name="lineHash">the unique identifier for a specidic timetable (the one found in the Calendar table)</param>
    /// <param name="ct">an instance of a ComressedTimetable object, containinga list of stops, times and other extra data</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool AddRouteCalendar(string routeID, string fileHash, CompressedTimetable ct)
    {
      try
      {
        sqlConn.InsertOrReplace(new RouteCalendar()
        {
          LineHash = fileHash,
          StopsIDs = JsonConvert.SerializeObject(ct.StopIds),
          StopsNames = JsonConvert.SerializeObject(ct.Stops),
          TripsIDs = JsonConvert.SerializeObject(ct.TripIds),
          Times = ct.CompressedTimes
        });
      }
      catch
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Retrieves a specific RouteCalendar
    /// </summary>
    /// <param name="lineHash">the unique identifier for a specidic timetable (the one found in the Calendar table)</param>
    /// <returns>a RouteCalendar object, containing a timetable, stops list and extra data (if available)</returns>
    public RouteCalendar GetRouteCalendar(string lineHash)
    {
      return sqlConn.Get<RouteCalendar>(x => x.LineHash == lineHash);
    }

    /// <summary>
    /// Removes a specific timetable from the database
    /// </summary>
    /// <param name="lineHash">the unique identifier for a specidic timetable (the one found in the Calendar table)</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool RemoveRouteCalendar(string lineHash)
    {
      SQLiteCommand sCmd = sqlConn.CreateCommand("DELETE FROM RouteCalendar WHERE LineHash = ?",
                                                 lineHash);
      return sCmd.ExecuteNonQuery() != 0;
    }

    #endregion


    #region RouteInfo

    public bool AddRouteInformation(string agencyID, string routeID, string color)
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
      return sqlConn.Table<RouteInfo>().Where(x=> x.AgencyID == agencyID).ToList();
    }

    #endregion

    /*
     * Database operations that interact with the RouteName table
     * (the one with the proper names for each route)
     */

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
      return sqlConn.Table<RouteName>().Where(x => x.AgencyID == agencyID).ToList();
    }

    public RouteName GetRouteName(string agencyID, string routeID)
    {
      return sqlConn.Get<RouteName>(x => x.AgencyID == agencyID && x.RouteID == routeID);
    }

    public bool RemoveRouteName(string agencyID, string routeID)
    {
      SQLiteCommand sCmd = sqlConn.CreateCommand("DELETE FROM RouteName WHERE AgencyID = ? AND RouteID = ?",
                                                agencyID, routeID);
      return sCmd.ExecuteNonQuery() != 0;
    }

    #endregion

    /*
     * Database operations that interact with the Version table, used for sync and update purposes
     */

    #region Version

    /// <summary>
    /// Retrieves the version number of the stored data for all available agencies
    /// </summary>
    /// <returns>a list of Version objects, containing Agency and version number of each stored agency</returns>
    public List<DBModels.Version> GetAllVersions()
    {
      return sqlConn.Table<DBModels.Version>().ToList();
    }

    /// <summary>
    /// Retrieves the version object fir a specific agency
    /// </summary>
    /// <param name="agencyID">the unique identifier of the desired agency</param>
    /// <returns>a Version object indicating the currently stored version number</returns>
    public DBModels.Version GetVersion(string agencyID)
    {
      return sqlConn.Get<DBModels.Version>(x => x.AgencyID == agencyID);
    }

    /// <summary>
    /// Removes a specific Version object from the database
    /// </summary>
    /// <param name="agencyID">the unique identifier of the desired agency</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool RemoveVersion(string agencyID)
    {
      SQLiteCommand sCmd = sqlConn.CreateCommand("DELETE FROM Version WHERE AgencyID = ?", agencyID);
      return sCmd.ExecuteNonQuery() != 0;
    }

    /// <summary>
    /// Updates the stored version number for a specific agency
    /// </summary>
    /// <param name="agencyID">the unique identifier of the desired agency</param>
    /// <param name="version">a string indicating the new version for a specific agency</param>
    /// <returns>a boolean value indicating the success of the operation</returns>
    public bool UpdateVersion(string agencyID, string version)
    {
      SQLiteCommand sCmd = sqlConn.CreateCommand("UPDATE Version SET VersionNumber = ? WHERE AgencyID = ?", version, agencyID);
      return sCmd.ExecuteNonQuery() != 0;
    }
    #endregion
  }
}
