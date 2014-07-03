using AuthenticationLibrary;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using ProfileServiceLibrary;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Helpers;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class MainPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    ExceptionLoggerHelper elh;

    AuthLibrary authLib;
    ProfileLibrary pll;
    Popup loginPopup;

    public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      elh = new ExceptionLoggerHelper();
    }

    public bool IsLogged
    {
      get { return Settings.IsLogged; }
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      if (!Settings.IsLogged)
      {
       BarLogin();
      }
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);

      string oldEx = elh.RetrieveLoggedException(ExceptionType.Unhandled);
      if (oldEx != null)
      {
        string extraEx = elh.RetrieveLoggedException(ExceptionType.Handled);
        oldEx = extraEx != null ? string.Format("{0}{1}{2}", oldEx, Environment.NewLine, extraEx) : oldEx;
        if (MessageBox.Show(AppResources.ErrorReportMessage, AppResources.ErrorReportCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
          SendErrorEmail(oldEx);
        elh.DeleteLoggedException(ExceptionType.Unhandled);
        elh.DeleteLoggedException(ExceptionType.Handled);
      }
      else
      {
        string extraEx = elh.RetrieveLoggedException(ExceptionType.Handled);

        if (extraEx != null)
        {
          if (MessageBox.Show(AppResources.CatchedErrorsReportMessage, AppResources.ErrorReportCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            SendErrorEmail(extraEx);
          elh.DeleteLoggedException(ExceptionType.Handled);
        }
      }
    }

    private static void SendErrorEmail(string exceptionMessage)
    {
      EmailComposeTask ect = new EmailComposeTask();
      ect.To = "smartcampuslab@outlook.com";
      ect.Subject = AppResources.ErrorReportCaption;
      ect.Body = string.Format("{1}{0}{2}{0}{3}{0}{4}", Environment.NewLine, Newtonsoft.Json.JsonConvert.SerializeObject(Environment.OSVersion),
        Environment.Version.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(Microsoft.Phone.Info.DeviceStatus.DeviceName), exceptionMessage);
      ect.Show();
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
      navigationService.UriFor<SavedJourneyPageViewModel>().Navigate();
    }

    public void ReadNotificationsTile()
    {
      MessageBox.Show("Crash Test", "Application crash.", MessageBoxButton.OK);
      throw new DivideByZeroException("Application crash self-test");
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
        eventAggregator.Publish(false);
        WebBrowser wb = new WebBrowser();
        wb.ClearCookiesAsync();
        wb.ClearInternetCacheAsync();
        wb.IsScriptEnabled = true;
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
      //User accept Google permissions
      if (e.Uri.ToString().StartsWith(Settings.RedirectUrl))
      {
        //User refuse SC permissions
        if (e.Uri.ToString().Contains("error=access_denied"))
        {
          MessageBox.Show(AppResources.MessageBoxSmartCampusPermsMessage, AppResources.MessageBoxSmartCampusPermsTitle, MessageBoxButton.OK);
          BarLogin();
        }
        else
        {
          string code = e.Uri.Query.Split('=')[1];
          Settings.AppToken = await authLib.GetAccessToken(code);
          NotifyOfPropertyChange(() => IsLogged);
          loginPopup.IsOpen = false;
          pll = new ProfileLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
          Settings.UserID = (await pll.GetBasicProfile()).UserId;
          eventAggregator.Publish(true);
        }
      }
      //User refuse Google permissions
      else if (e.Uri.ToString().Contains("&openid.mode=cancel&"))
      {
        MessageBox.Show(AppResources.MessageBoxGooglePermsMessage, AppResources.MessageBoxGooglePermsTitle, MessageBoxButton.OK);
        BarLogin();
      }
    }

    public void BarLogout()
    {
      Settings.AppToken = null;
      NotifyOfPropertyChange(() => IsLogged);
      if (!Settings.IsLogged)
      {
        BarLogin();
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

    #endregion
  }
}