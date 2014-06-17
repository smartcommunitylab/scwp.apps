using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using Models.MobilityService.Journeys;
using System.Threading.Tasks;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Controls;
using Windows.Phone.Devices.Notification;
using ViaggiaTrentino.Resources;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Helpers;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class FavouritePlaceView : UserControl
  {
    LocationChooserHelper lch;

    public Position SelectedPosition
    {
      get { return smartCampusACB.Tag as Position; }
    }

    public FavouritePlaceView()
    {      
      InitializeComponent();
      lch = new LocationChooserHelper();
      lch.PositionObtained += lch_PositionObtained;
    }

    void lch_PositionObtained(object sender, Position results)
    {
      smartCampusACB.Tag = results;
      smartCampusACB.Text = results.Name;
    }

  
    private void GpsLocTo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      lch.ShowLocationSelectorHelper(); 
    }
  }
}
