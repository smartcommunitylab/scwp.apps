using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using System.Linq;
using DBManager.DBModels;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using TerritoryInformationServiceLibrary;
using Models.TerritoryInformationService;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Controls;
using ViaggiaTrentino.Views.Controls;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectBusRouteViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private AgencyType agencyID;
    private MessagePrompt mp;
    private ObservableCollection<RouteInfo> routesName;
    private TerritoryInformationLibrary til;
    private PublicTransportLibrary ptl;
    BackgroundWorker bw;
    bool isLoadCompleted;

    public SelectBusRouteViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      isLoadCompleted = false;
    }

    public string SelectedAgencyTitle
    {
      get
      {
        switch (agencyID)
        {
          case AgencyType.TrentoCityBus: return AppResources.TileTrentoBusMessage;
          case AgencyType.RoveretoCityBus: return AppResources.TileRoveretoBusMessage;
          default: return AppResources.ApplicationTitle;
        }
      }
    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; NotifyOfPropertyChange(() => SelectedAgencyTitle); }
    }

    public ObservableCollection<RouteInfo> RoutesName
    {
      get
      {
        return routesName;
      }
      set
      {
        routesName = value;
        NotifyOfPropertyChange(() => RoutesName);
      }
    }

    protected override void OnInitialize()
    {
      base.OnInitialize();
      RoutesName = new ObservableCollection<RouteInfo>();
    }

    protected override void OnViewReady(object view)
    {
      base.OnViewReady(view);
      if (!isLoadCompleted)
      {
        RoutesName.Clear();
        bw = new BackgroundWorker();
        bw.DoWork += bw_DoWork;
        bw.ProgressChanged += bw_ProgressChanged;
        bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        bw.WorkerReportsProgress = true;
        bw.WorkerSupportsCancellation = true;
        bw.RunWorkerAsync();
      }
    }

    void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
     // if(!e.Cancelled)
    }


    void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      RoutesName.Add(e.UserState as RouteInfo);
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {

      using (DBHelper dbh = new DBHelper())
      {
        List<RouteInfo> rName = dbh.GetRouteInfo(EnumConverter.ToEnumString<AgencyType>(AgencyID));
        foreach (var item in rName)
        {
          if ((sender as BackgroundWorker).CancellationPending)
            return;
          (sender as BackgroundWorker).ReportProgress(0, item);
          Thread.Sleep(50);
          
        }
      }
      isLoadCompleted = true;

    }

    public void OpenTimetableView(DBManager.DBModels.RouteInfo obj)
    {
      if (bw.IsBusy)
        bw.CancelAsync();
      using (DBHelper dbh = new DBHelper())
      {
        var routenames = dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(agencyID));
        int result;
        if(agencyID == AgencyType.RoveretoCityBus && Int32.TryParse(obj.RouteID.ToLower(), out result))
            routenames = routenames.Where(x => x.RouteID.ToLower().StartsWith("0" + obj.RouteID.ToLower()) || x.RouteID.ToLower().StartsWith(("N" + obj.RouteID).ToLower())).ToList();
        else
          routenames = routenames.Where(x => x.RouteID.ToLower().StartsWith(obj.RouteID.ToLower())).ToList();

        if (routenames.Count == 1)
          navigationService.UriFor<TimetablePageViewModel>()
            .WithParam(x => x.AgencyID, agencyID)
            .WithParam(x => x.RouteIDWitDirection, routenames.First().RouteID)
            .WithParam(x => x.Description, routenames.First().Name)
            .WithParam(x => x.NameID, obj.Name)
            .WithParam(x => x.Color, obj.Color)
            .Navigate();
        else
        {
          PhoneApplicationService.Current.State["routeNamesForDirections"] = routenames;
          navigationService.UriFor<SelectBusRouteDirectionViewModel>()
            .WithParam(x => x.AgencyID, agencyID)
            .WithParam(x => x.RouteID, obj.RouteID)
            .WithParam(x => x.Name, obj.Name)
            .WithParam(x => x.Color, obj.Color)
            .Navigate();
        }
      }
    }

    public async Task<List<POIObject>> RetrieveAllStops(double[] coordinates, double radius, string[] agencyIds)
    {
      til = new TerritoryInformationLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      List<POIObject> results = null;
      Dictionary<string, object> criteria = new Dictionary<string, object>();
      criteria.Add("source", "smartplanner-transitstops");
      criteria.Add("customData.agencyId", agencyIds);

      await Settings.RefreshToken();
      results = await til.ReadPlaces(new FilterObject()
      {
        SkipFirstElements = 0,
        NumberOfResults = -1,
        Categories = new List<string>() { "Mobility" },
        MongoFilters = criteria,
        Coordinates = new double[2] { coordinates[0], coordinates[1] },
        Radius = radius
      });

      results = (from place in results
                 group place by new { place.Poi.Latitude, place.Poi.Longitude }
                   into mygroup
                   select mygroup.First()).ToList();

      return results;
    }

    public void TappedPushPin(POIObject stop)
    {
      mp = new MessagePrompt();
      mp.Title = Resources.AppResources.SubAlertStop;
      mp.Body = new StopPopupView(mp, navigationService) { DataContext = stop };
      mp.ActionPopUpButtons.Clear();
      mp.HorizontalAlignment = HorizontalAlignment.Center;
      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.Show();
    }
  }
}