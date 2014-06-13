using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
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
using Windows.Phone.Devices.Notification;

namespace ViaggiaTrentino.ViewModels
{
  public class SettingsPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private ObservableCollection<Position> posFavourite;
    FileStorageHelper fsh;
    FavouritePlaceView fpv;

    public SettingsPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      posFavourite = new ObservableCollection<Position>();
      fsh = new FileStorageHelper();
    }

    public bool LocationConsent
    {
      get { return Settings.LocationConsent; }
      set
      {
        Settings.LocationConsent = value;
        NotifyOfPropertyChange(() => LocationConsent);
        if (value) { Settings.LaunchGPS(); }
      }
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
      string poses = fsh.ReadFile("favourites.pos");
      if(poses != null)
        FavPositions = new ObservableCollection<Position>( JsonConvert.DeserializeObject<List<Position>>(poses));
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
      FavPositions.Add(fpv.SelectedPosition);
      fsh.WriteFile("favourites.pos", JsonConvert.SerializeObject(FavPositions.ToArray()), true);
    }

   
  }
}