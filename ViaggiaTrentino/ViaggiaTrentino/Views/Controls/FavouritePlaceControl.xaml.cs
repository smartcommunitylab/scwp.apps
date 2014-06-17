using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Caliburn.Micro;
using Models.MobilityService.Journeys;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Device.Location;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class FavouritePlaceControl : UserControl
  {
    private IEventAggregator eventAggregator;

    public FavouritePlaceControl()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
    }


    private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      Position toShowPos = this.DataContext as Position;
   
      GeoCoordinate favMapGeoCoord = new GeoCoordinate(Convert.ToDouble(toShowPos.Latitude), Convert.ToDouble(toShowPos.Longitude));


      Pushpin toPush = new Pushpin()
      {
        Content = toShowPos.Name,
        GeoCoordinate = favMapGeoCoord
      };

      eventAggregator.Publish(toPush);
    }

    private void DeleteFavourite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      eventAggregator.Publish(this.DataContext as Position);
    }
  }
}
