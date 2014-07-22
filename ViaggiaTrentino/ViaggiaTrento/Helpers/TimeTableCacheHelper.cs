using CommonHelpers;
using DBManager;
using MobilityServiceLibrary;
using Models.MobilityService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ViaggiaTrentino.Helpers
{
  public class TimeTableCacheHelper
  {
    PublicTransportLibrary ptLib;

    /*
     * this function updates the stored timetable
     * 
     * 1) loads from database the Version table, which has all the cache versions associated with various agencyIDs
     * 2) retrieves from the server a list of all the updates that were published since the last update.
     *    this provides two lists, containing one the new timetables and the other the revoked ones 
     * 3) retrieves and processess the full timetable for each item in the new timetable list, then stores them 
     *    in the database
     * 4) removes all timetables found in the list of revoked ones from the database
     * 5) adds the references to the new timetable to the database
     * 6) updates the Version table with the latest info
     * 
     */
    public async Task<bool> UpdateCachedCalendars()
    {
      bool result = false;
      try
      {
        ptLib = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);

        using (DBHelper dbHelp = new DBHelper())
        {
          Dictionary<AgencyType, string> listVers = dbHelp.GetAllVersions().ToDictionary(x => EnumConverter.ToEnum<AgencyType>(x.AgencyID), t => t.VersionNumber);
          var results = await ptLib.GetReadTimetableCacheUpdates(listVers);

          foreach (var item in results)
          {
            foreach (var file in item.Value.Added)
            {
              var res = await ptLib.GetReadSingleTimetableCacheUpdates(EnumConverter.ToEnum<AgencyType>(item.Key), file);
              dbHelp.AddRouteCalendar(file.Split('_')[0], file, res);
            }
            foreach (var file in item.Value.Removed)
            {
              dbHelp.RemoveRouteCalendar(file);
            }
            dbHelp.AddCalendarsForAgency(item.Key, item.Value.Calendars);
            dbHelp.UpdateVersion(item.Key, item.Value.Version.ToString());
          }
        }
        result = true;
      }
      finally
      {
      }

      return result;


    }
  }
}
