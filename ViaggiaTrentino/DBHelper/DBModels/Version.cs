﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper.DBModels
{
  public class Version
  {
    [PrimaryKey]
    public string AgencyID { get; set; }
    public string Version { get; set; }

  }
}
