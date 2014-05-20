using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DBModels
{
  public class RouteInfo
  {
    [PrimaryKey]
    public string AgencyID { get; set; }

    [PrimaryKey]
    public string RouteID { get; set; }
    
    public string Color { get; set; }
  }
}
