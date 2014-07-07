using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Models.MobilityService.Journeys;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class SettingsPageViewModel : Screen, IHandle<Position>, IHandle<Pushpin>
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private ObservableCollection<Position> posFavourite;
    FileStorageHelper fsh;
    FavouritePlaceView fpv;
    Popup pu;

    public SettingsPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      posFavourite = new ObservableCollection<Position>();
      fsh = new FileStorageHelper();
      pu = new Popup();
    }

    #region Properties

    public ObservableCollection<Position> FavPositions
    {
      get { return posFavourite; }
      set
      {
        posFavourite = value;
        NotifyOfPropertyChange(() => FavPositions);
      }
    }

    public bool LocationConsent
    {
      get { return Settings.LocationConsent; }
      set
      {
        Settings.LocationConsent = value;
        NotifyOfPropertyChange(() => LocationConsent);
        Settings.LaunchGPS();
      }
    }

    public bool FeedbackEnabled
    {
      get { return Settings.FeedbackEnabled; }
      set
      {
        Settings.FeedbackEnabled = value;
        NotifyOfPropertyChange(() => LocationConsent);        
      }
    }

    #endregion

    #region Page overrides

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      eventAggregator.Unsubscribe(this);
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      eventAggregator.Subscribe(this);
      string poses = fsh.ReadFile("favourites.pos");
      if (poses != null)
        FavPositions = new ObservableCollection<Position>(JsonConvert.DeserializeObject<List<Position>>(poses));
    }

    #endregion

    #region Appbar

    public void BarAdd()
    {
      fpv = new FavouritePlaceView();
      MessagePrompt mpSmall = new MessagePrompt();
      mpSmall.VerticalAlignment = VerticalAlignment.Center;
      mpSmall.HorizontalAlignment = HorizontalAlignment.Stretch;
      mpSmall.Title = AppResources.ChooseTitle;
      mpSmall.Body = fpv;
      mpSmall.Completed += mpSmall_Completed;
      mpSmall.Show();
    }

    void mpSmall_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
    {
      //do stuff with fpv.SelectedPosition
      if (e.PopUpResult == PopUpResult.Ok && fpv.SelectedPosition != null)
      {
        FavPositions.Add(fpv.SelectedPosition);
        fsh.WriteFile("favourites.pos", JsonConvert.SerializeObject(FavPositions.ToArray()), true);
      }
    }

    #endregion

    #region Handlers from /Vires/Controls/FavouritePlaceControl

    // removes a position from stored list of favourites
    public void Handle(Position message)
    {
      FavPositions.Remove(message);
      fsh.WriteFile("favourites.pos", JsonConvert.SerializeObject(FavPositions.ToArray()), true);
    }

    // shows map with centered Pushpin to selected location
    public void Handle(Pushpin message)
    {
      Map favMap = new Map();
      MapLayer favMapLayer = new MapLayer();
      MapOverlay favMapOverlay = new MapOverlay();

      favMapOverlay.Content = message;
      favMapOverlay.GeoCoordinate = message.GeoCoordinate;

      favMapLayer.Add(favMapOverlay);
      favMap.Layers.Add(favMapLayer);

      favMap.Height = Application.Current.Host.Content.ActualHeight;
      favMap.Width = Application.Current.Host.Content.ActualWidth;
      favMap.Center = message.GeoCoordinate;
      favMap.ZoomLevel = 15;

      MessagePrompt mp = new MessagePrompt();
      mp.Style = Application.Current.Resources["mpNoBorders"] as Style;
      mp.Body = favMap;
      //mp.ActionPopUpButtons.Clear();
      mp.Show();
    }   

    #endregion
  }
}