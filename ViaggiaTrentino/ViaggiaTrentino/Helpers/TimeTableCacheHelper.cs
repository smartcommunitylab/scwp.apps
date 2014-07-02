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
