using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.DBModels
{
  public class Version
  {
    [PrimaryKey]
    public string AgencyID { get; set; }
    public string VersionNumber { get; set; }

  }
}
