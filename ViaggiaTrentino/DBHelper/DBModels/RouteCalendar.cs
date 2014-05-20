using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DBModels
{
  public class RouteCalendar
  {
    [PrimaryKey]
    public string LineHash { get; set; }

    public string StopsIDs { get; set; }
    
    public string StopsNames { get; set; }
    
    public string TripsIDs { get; set; }
    
    public string Times { get; set; }
  }
}
