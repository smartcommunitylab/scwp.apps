using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using XML2DB.Resources;
using DBHelper.DBModels;

namespace XML2DB
{
  public partial class MainPage : PhoneApplicationPage
  {
    // Constructor
    public MainPage()
    {
      InitializeComponent();

      // Sample code to localize the ApplicationBar
      //BuildLocalizedApplicationBar();
    }

    private void btnStart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      xmlToSqlite.XmlToSqlite a = new xmlToSqlite.XmlToSqlite();
      List<RouteInfo> ri = a.ReadRoutesInfo();
      List<RouteName> rn = a.ReadRoutesName();

      DBHelper.DBHelper dbh = new DBHelper.DBHelper();
      
    }

  }
}