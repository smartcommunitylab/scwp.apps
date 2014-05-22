using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectBusRouteViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private AgencyType agencyID;

    ObservableCollection<DBManager.DBModels.RouteInfo> routesName;
    PublicTransportLibrary ptl;

    public SelectBusRouteViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);

    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }
    public ObservableCollection<DBManager.DBModels.RouteInfo> RoutesName
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
      using (DBHelper dbh = new DBHelper())
      {
        RoutesName = new ObservableCollection<DBManager.DBModels.RouteInfo>(dbh.GetRouteInfo(EnumConverter.ToEnumString<AgencyType>(AgencyID)));
      }
    }


    public void OpenTimetableView(DBManager.DBModels.RouteInfo obj)
    {
      navigationService.UriFor<TimetablePageViewModel>().WithParam(x => x.AgencyID, agencyID).WithParam(x => x.RouteID, obj.RouteID).Navigate();
    }
  }
}