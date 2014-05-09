using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class MainPageViewModel : Screen
  {
    private readonly INavigationService navigationService;

    public MainPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    #region Tiles

    public void PlanJourneyTile()
    {
      MessageBox.Show("Plan Journey");
    }

    public void PlanRecurrentJourneyTile()
    {
      MessageBox.Show("Plan Recurrent Journey");
    }

    public void SavedJourneysTile()
    {
      //MessageBox.Show("Saved Journeys");
      navigationService.UriFor<SavedJourneyPageViewModel>().Navigate();
    }

    public void ReadNotificationsTile()
    {
      MessageBox.Show("Read Notifications");
    }

    public void SubmitAlertTile()
    {
      MessageBox.Show("Submit Alert");
    }

    public void RealTimeInfoTile()
    {
      navigationService.UriFor<RealTimeInfoViewModel>().Navigate();
    }

    #endregion

    #region AppBar

    public void BarLogin()
    {
      MessageBox.Show("login");
    }

    public void BarTour()
    {
      MessageBox.Show("tour");
    }


    public void BarAbout()
    {
      MessageBox.Show("about");
    }

    public void BarSettings()
    {
      navigationService.UriFor<SettingsPageViewModel>().Navigate();
    }

    public void BarTest()
    {
      navigationService.UriFor<TestPageViewModel>().Navigate();
    }

    #endregion
  }
}