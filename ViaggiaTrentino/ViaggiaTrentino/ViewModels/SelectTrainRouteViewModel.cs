using Caliburn.Micro;
using CommonHelpers;
using DBManager;
using DBManager.DBModels;
using Models.MobilityService;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class SelectTrainRouteViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private ObservableCollection<RouteName> routesName;

    public SelectTrainRouteViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    public ObservableCollection<RouteName> RoutesName
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

    protected override void OnActivate()
    {
      base.OnActivate();
      List<RouteName> results = new List<RouteName>();
      using (DBHelper dbh = new DBHelper())
      {
        results.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.BolzanoVeronaRailway)));
        results.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.TrentoBassanoDelGrappaRailway)));
        results.AddRange(dbh.GetRoutesNames(EnumConverter.ToEnumString<AgencyType>(AgencyType.TrentoMaleRailway)));
      }
      RoutesName = new ObservableCollection<RouteName>(results);
    }
    
    public void ShowTimetable(object obj)
    {
      var routeName = (obj as TextBlock).Tag as RouteName;
      navigationService.UriFor<TimetablePageViewModel>()
        .WithParam(x => x.AgencyID, EnumConverter.ToEnum<AgencyType>(routeName.AgencyID))
        .WithParam(x => x.RouteIDWitDirection, routeName.RouteID)
        .WithParam(x => x.Description, routeName.Name)
        .WithParam(x => x.NameID, "")
        .WithParam(x => x.Color, "#555555")
        .Navigate();
    }
  }
}