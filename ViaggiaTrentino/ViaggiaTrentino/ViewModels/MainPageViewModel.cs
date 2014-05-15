using AuthenticationLibrary;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Models.AuthorizationService;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ViaggiaTrentino.ViewModels
{
  public class MainPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    AuthLibrary authLib;
    Popup loginPopup;

    public MainPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;

      Settings.Initialize();
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      if (!Settings.IsLogged)
      {
        MessageBox.Show("Loggati");
        BarLogin();
      }
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
      authLib = new AuthLibrary(Settings.ClientId, Settings.ClientSecret, Settings.RedirectUrl, Settings.ServerUrl);

      if (Settings.AppToken == null)
      {
        WebBrowser wb = new WebBrowser();
        wb.Navigating += wb_Navigating;
        wb.Height = Application.Current.Host.Content.ActualHeight;
        wb.Width = Application.Current.Host.Content.ActualWidth;
        loginPopup = new Popup();
        loginPopup.Child = wb;
        loginPopup.IsOpen = true;
        wb.Navigate(AuthUriHelper.GetCodeUri(Settings.ClientId, Settings.RedirectUrl));
      }
    }

    async void wb_Navigating(object sender, NavigatingEventArgs e)
    {
      if (e.Uri.ToString().StartsWith(Settings.RedirectUrl))
      {
        string code = e.Uri.Query.Split('=')[1];
        Settings.AppToken = await authLib.GetAccessToken(code);
        loginPopup.IsOpen = false;
      }
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