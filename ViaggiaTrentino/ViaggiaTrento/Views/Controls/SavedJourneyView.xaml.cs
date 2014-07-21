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
using ViaggiaTrentino.Resources;

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
      if(MessageBox.Show(AppResources.SureDelete, AppResources.Warn, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
      {
        try
        {
          App.LoadingPopup.Show();
          await Settings.RefreshToken();
          bool delRes = await urLib.DeleteSingleJourney(basIti.ClientId);
          if (delRes)
          {
            this.Visibility = System.Windows.Visibility.Collapsed;
            this.IsEnabled = false;
          }
        }
        finally
        {
          App.LoadingPopup.Hide();
        }
        
      }
    }

    private async void MonitorJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        basIti.Monitor = await urLib.SetMonitorSingleJourney(basIti.ClientId, !basIti.Monitor);
        this.DataContext = basIti;
        if (basIti.Monitor)
          retMonitor.Fill = new SolidColorBrush(Colors.Green);
        else retMonitor.Fill = new SolidColorBrush(Colors.Red);
      }
      finally
      {
        App.LoadingPopup.Hide();
      }

    }    
  }
}
