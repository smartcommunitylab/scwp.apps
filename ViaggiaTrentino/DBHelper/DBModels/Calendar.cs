using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DBModels
{
  public class Calendar
  {
    [PrimaryKey]
    public string AgencyID { get; set; }
    
    [PrimaryKey]
    public string RouteID { get; set; }
       
    public string CalendarEntries { get; set; }
    
  }
}
