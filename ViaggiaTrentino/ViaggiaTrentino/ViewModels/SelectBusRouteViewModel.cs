using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using CommonHelpers;
using DBManager;
using DBManager.DBModels;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.TerritoryInformationService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TerritoryInformationServiceLibrary;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;

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

    #region Properties

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

    #endregion

    #region Page overrides

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
        bw.WorkerReportsProgress = true;
        bw.WorkerSupportsCancellation = true;
        bw.RunWorkerAsync();
      }
    }

    #endregion

    #region Database loading

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
        {
          navigationService.UriFor<TimetablePageViewModel>()
            .WithParam(x => x.AgencyID, agencyID)
            .WithParam(x => x.RouteIDWitDirection, routenames.First().RouteID)
            .WithParam(x => x.Description, routenames.First().Name)
            .WithParam(x => x.NameID, obj.Name)
            .WithParam(x => x.Color, obj.Color)
            .Navigate();
        }
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

    #endregion

    // this function is cheatously used in the associated View for this ViewModel
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
      mp.Body = new StopPopupView(mp, navigationService) { DataContext = stop };
      mp.Style = Application.Current.Resources["mpNoTitleNoButtons"] as Style;
      mp.ActionPopUpButtons.Clear();
      mp.HorizontalAlignment = HorizontalAlignment.Center;
      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.Show();
    }
  }
}