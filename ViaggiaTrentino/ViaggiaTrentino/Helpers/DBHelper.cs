using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.Helpers
{
  public class DBHelper
  {
    string dbName;

    public DBHelper(string DBName)
    {
      dbName = DBName;
    }

    public DBHelper()
    {
      dbName = "SCDB.sqlite";
    }

    private bool OpenConnection()
    {
      return true;
    }

    private bool CloseConnection()
    {
      return true;
    }

    #region Calendar

    //public bool AddCalendarEntry(Models.MobilityService.PublicTransport.)
    //{
    //  return true;
    //}

    #endregion

    #region Route_calendat

    #endregion

    #region Route_names

    #endregion

    #region Urbaninfo

    #endregion

    #region Version

    #endregion

    //public void 

  }
}
