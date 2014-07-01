using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Models.MobilityService.Journeys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Phone.Devices.Notification;

namespace ViaggiaTrentino.ViewModels
{
  public class SettingsPageViewModel : Screen, IHandle<Position>, IHandle<Pushpin>
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private ObservableCollection<Position> posFavourite;
    Popup pu;
    FileStorageHelper fsh;
    FavouritePlaceView fpv;

    public SettingsPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      posFavourite = new ObservableCollection<Position>();
      fsh = new FileStorageHelper();
      pu = new Popup();
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

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
      eventAggregator.Unsubscribe(this);
    }

    public ObservableCollection<Position> FavPositions
    {
      get { return posFavourite; }
      set
      {
        posFavourite = value;
        NotifyOfPropertyChange(() => FavPositions);
      }
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      eventAggregator.Subscribe(this);
      string poses = fsh.ReadFile("favourites.pos");
      if (poses != null)
        FavPositions = new ObservableCollection<Position>(JsonConvert.DeserializeObject<List<Position>>(poses));
    }

    
    public void BarAdd()
    {
      fpv = new FavouritePlaceView();
      MessagePrompt mp = new MessagePrompt();
      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.HorizontalAlignment = HorizontalAlignment.Stretch;
      mp.Title = AppResources.ChooseTitle;
      mp.Body = fpv;
      mp.Completed += mpSmall_Completed;
      mp.Show();
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

    //override 

    public void Handle(Position message)
    {
      FavPositions.Remove(message);
      fsh.WriteFile("favourites.pos", JsonConvert.SerializeObject(FavPositions.ToArray()), true);
    }

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
  }
}