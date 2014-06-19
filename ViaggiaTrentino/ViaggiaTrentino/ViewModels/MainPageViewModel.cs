﻿using AuthenticationLibrary;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Models.AuthorizationService;
using ProfileServiceLibrary;
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
    ProfileLibrary pll;
    Popup loginPopup;

    public MainPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    protected override async void OnActivate()
    {
      base.OnActivate();
      if (!Settings.IsLogged)
      {
        BarLogin();
      }
      else await Settings.RefreshToken(true);
    }
    

    #region Tiles

    public void PlanJourneyTile()
    {
      navigationService.UriFor<PlanNewSingleJourneyViewModel>().Navigate();
    }

    public void PlanRecurrentJourneyTile()
    {
      navigationService.UriFor<MonitorJourneyViewModel>().Navigate();

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
      navigationService.UriFor<SelectAlertPageViewModel>().Navigate();
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
        pll = new ProfileLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
        Settings.UserID = (await pll.GetBasicProfile()).UserId;
      }
    }

    public void BarTour()
    {
      MessageBox.Show("tour");
    }


    public void BarAbout()
    {
      navigationService.UriFor<AboutPageViewModel>().Navigate();

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