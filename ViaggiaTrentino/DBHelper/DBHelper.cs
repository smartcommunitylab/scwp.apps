using DBHelper.DBModels;
using Models.MobilityService.PublicTransport;
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

    public Calendar GetCalendar(string agencyID, string routeID)
    {
      return sqlConn.Get<Calendar>(x => x.AgencyID == agencyID && x.Route == routeID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="routeID">the route ID for the specified calendar. It is the KEY field in the 'Calendars' property of the TimetableCacheUpdate model</param>
    /// <param name="routeCalendar">the fully mapped calendar. It is the VALUE field in the 'Calendars' property of the TimetableCacheUpdate model</param>
    /// <returns></returns>
    public bool AddCalendar(string routeID,   routeCalendar)
    {
      
    }

    #endregion

    #region RouteCalendar

    #endregion

    #region RotueInfo

    #endregion

    #region RouteName

    #endregion

    #region Version

    #endregion
  }
}
