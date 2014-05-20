using DBHelper.DBModels;
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
      return null;
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
