using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViaggiaTrentino.ViewModels.Controls;
using MobilityServiceLibrary;
using Models.MobilityService.Journeys;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class SavedRecurrentJourneyView : UserControl
  {
    UserRouteLibrary urLib;
    BasicRecurrentJourney basIti;

    public SavedRecurrentJourneyView()
    {
      InitializeComponent();
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      basIti = this.DataContext as BasicRecurrentJourney;
    }

    private async void DeleteJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if(await urLib.DeleteRecurrentJourney(basIti.ClientId))
        this.Visibility = System.Windows.Visibility.Collapsed;
    }

    private void MonitorJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      urLib.SetMonitorSingleJourney(basIti.ClientId, !basIti.Monitor);
    }    
  }
}
