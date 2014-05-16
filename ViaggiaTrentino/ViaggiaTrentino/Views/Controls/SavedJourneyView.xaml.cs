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
  public partial class SavedJourneyView : UserControl
  {
    UserRouteLibrary urLib;
    public SavedJourneyView()
    {
      InitializeComponent();
      urLib = new UserRouteLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    private void DeleteJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      MessageBox.Show("dovrei cancellare il journey ma non lo farò");
      this.Visibility = System.Windows.Visibility.Collapsed;
      //urLib.DeleteSingleJourney(basITI.ClientId);
      //(this.Parent as ListBox).Items.Remove(this);
    }

    private void MonitorJourney_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {

    }
  }
}
