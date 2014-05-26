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
using DBManager.DBModels;
using SQLite;
using System.IO;
using Windows.Storage;
using Models.MobilityService;
using Newtonsoft.Json;
using CommonHelpers;
using Models.MobilityService.PublicTransport;

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

    private async void btnStart_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (apptoken.Text.Trim() == "")
      {
        MessageBox.Show("inserisci il token di test_api");
        return;
      }


      xmlToSqlite.XmlToSqlite a = new xmlToSqlite.XmlToSqlite();
      progress.Text += "get routes info\n";
      List<RouteInfo> ri = a.ReadRoutesInfo();
      progress.Text += "get routes name\n";
      List<RouteName> rn = a.ReadRoutesName();

      string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "scdb.sqlite"));
      SQLiteXML2DB.SQLiteConnection sqlConn = new SQLiteXML2DB.SQLiteConnection(DB_PATH);

      progress.Text += "drop tables\n";

      sqlConn.DropTable<RouteName>();
      sqlConn.DropTable<RouteInfo>();
      sqlConn.DropTable<RouteCalendar>();
      sqlConn.DropTable<Calendar>();
      sqlConn.DropTable<DBManager.DBModels.Version>();

      progress.Text += "create new tables\n";
      sqlConn.CreateTable<RouteName>();
      sqlConn.CreateTable<RouteInfo>();
      sqlConn.CreateTable<RouteCalendar>();
      sqlConn.CreateTable<Calendar>();
      sqlConn.CreateTable<DBManager.DBModels.Version>();

      //fill routeinfo
      progress.Text += "fill routeinfo table\n";
      int b = sqlConn.InsertAll(ri, typeof(RouteInfo));
      //fill routename
      progress.Text += "fille routename table\n";
      int c = sqlConn.InsertAll(rn, typeof(RouteName));
      //fill version
      progress.Text += "fill version table\n";
      List<DBManager.DBModels.Version> versions = new List<DBManager.DBModels.Version>();
      versions.Add(new DBManager.DBModels.Version() { AgencyID = "12", VersionNumber = "0" });
      versions.Add(new DBManager.DBModels.Version() { AgencyID = "16", VersionNumber = "0" });
      versions.Add(new DBManager.DBModels.Version() { AgencyID = "10", VersionNumber = "0" });
      versions.Add(new DBManager.DBModels.Version() { AgencyID = "5", VersionNumber = "0" });
      versions.Add(new DBManager.DBModels.Version() { AgencyID = "6", VersionNumber = "0" });
      int d = sqlConn.InsertAll(versions, typeof(DBManager.DBModels.Version));


      Dictionary<AgencyType, string> dict = new Dictionary<AgencyType, string>();
      foreach (var item in versions)
      {
        dict.Add(EnumConverter.ToEnum<AgencyType>(item.AgencyID), item.VersionNumber.ToString());
      }

      string token = apptoken.Text.Trim();

      token = "b5372740-81e3-40dc-a11c-9d38cfe11121";
      progress.Text += "retrieve updates from sc server\n";

      MobilityServiceLibrary.PublicTransportLibrary ptl = new MobilityServiceLibrary.PublicTransportLibrary(token, "https://vas-dev.smartcampuslab.it/");
      var results = await ptl.GetReadTimetableCacheUpdates(dict);

      DBManager.DBHelper dbh = new DBManager.DBHelper();

      progress.Text += "inserting calendars into db";
      foreach (var item in results)
      {
        foreach (var file in item.Value.Added)
        {
          var res = await ptl.GetReadSingleTimetableCacheUpdates(EnumConverter.ToEnum<AgencyType>(item.Key), file);
          dbh.AddRouteCalendar(file.Split('_')[0], file, res);
        }
        foreach (var file in item.Value.Removed)
        {
          dbh.RemoveRouteCalendar(file);
        }
        dbh.AddCalendarsForAgency(item.Key, item.Value.Calendars);
        dbh.UpdateVersion(item.Key, item.Value.Version.ToString());
      }

      MessageBox.Show("EOI");
    }
  }
}