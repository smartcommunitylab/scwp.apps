using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;
using System.Windows.Media;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class SavedJourneyView : UserControl
  {
    UserRouteLibrary urLib;
    BasicItinerary basIti;

    public SavedJourneyView()
    {
      InitializeComponent();
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      basIti = this.DataContext as BasicItinerary;
    }

    private async void DeleteJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if(await urLib.DeleteSingleJourney(basIti.ClientId))
        this.Visibility = System.Windows.Visibility.Collapsed;
    }

    private async void MonitorJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      basIti.Monitor = await urLib.SetMonitorSingleJourney(basIti.ClientId, !basIti.Monitor);
      this.DataContext = basIti;
      if (basIti.Monitor)
        retMonitor.Fill = new SolidColorBrush(Colors.Green);
      else retMonitor.Fill = new SolidColorBrush(Colors.Red);
    }    
  }
}
