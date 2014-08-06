using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using MobilityServiceLibrary;
using Models.MobilityService.RealTime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class RoadInfoPageViewModel : Screen
  {
    private IEventAggregator eventAggregator;
    private INavigationService navigationService;
    private PublicTransportLibrary ptl;
    private ObservableCollection<AlertRoad> decrees;
    bool noResults;

    private string json;

    public RoadInfoPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.eventAggregator = eventAggregator;
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      NoResults = false;
    }

    private long DateTimeToEpoch(DateTime dt)
    {
      TimeSpan span = (dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
      return Convert.ToInt64(span.TotalMilliseconds);
    }

    public ObservableCollection<AlertRoad> Decrees
    {
      get { return decrees; }
      set
      {
        decrees = value;
        NotifyOfPropertyChange(() => Decrees);
      }
    }

    public bool NoResults
    {
      get { return noResults; }
      private set
      {
        noResults = value;
        NotifyOfPropertyChange(() => NoResults);
      }
    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      List<AlertRoad> results;
      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        results = await ptl.GetRoadInfoByAgency(Settings.ParkingAgencyId, DateTimeToEpoch(DateTime.Now), DateTimeToEpoch(DateTime.Now.AddDays(7)));
      }
      finally
      {
        App.LoadingPopup.Hide();
      }

      if (results.Count > 0)
        NoResults = false;
      else
        NoResults = true;

      eventAggregator.Publish(results);
      Decrees = new ObservableCollection<AlertRoad>(results);
    }

    public void OpenSingleDecreePopup(object data)
    {
      MessagePrompt mp = new MessagePrompt();
      mp.Style = Application.Current.Resources["mpNoTitleNoButtons"] as Style;
      mp.Body = new DecreePopupView(mp) { DataContext = (data as AlertRoad) };
      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.HorizontalAlignment = HorizontalAlignment.Center;
      mp.Show();
    }

  }
}
