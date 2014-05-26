using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using DBManager.DBModels;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService;
using Models.MobilityService.PublicTransport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectBusRouteDirectionViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private AgencyType agencyID;
    private string routeID, name, color;
    ObservableCollection<RouteName> routeDirections;

    public SelectBusRouteDirectionViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    #region Properties
    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }

    public string RouteID
    {
      get { return routeID; }
      set { routeID = value; }
    }

    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    public string Color
    {
      get { return color; }
      set { color = value; }
    }

    public ObservableCollection<RouteName> RouteDirections
    {
      get
      {
        return routeDirections;
      }
      set
      {
        routeDirections = value;
        NotifyOfPropertyChange(() => RouteDirections);
      }
    }
    #endregion

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      RouteDirections = new ObservableCollection<RouteName>(PhoneApplicationService.Current.State["routeNamesForDirections"] as List<RouteName>);
      PhoneApplicationService.Current.State.Remove("routeNamesForDirections");

    }
    public void ShowTimetable(object obj)
    {
      var a = obj as TextBlock;
      navigationService.UriFor<TimetablePageViewModel>()
        .WithParam(x => x.AgencyID, agencyID)
        .WithParam(x => x.RouteIDWitDirection, a.Tag)
        .WithParam(x => x.Description, a.Text)
        .WithParam(x => x.NameID, name)
        .WithParam(x => x.Color, color)
        .Navigate();
    }
  }
}