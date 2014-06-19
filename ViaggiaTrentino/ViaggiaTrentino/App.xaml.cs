using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViaggiaTrentino.Resources;
using ViaggiaTrentino.Views.Controls;
using System.Windows.Controls.Primitives;
using Coding4Fun.Toolkit.Controls;

namespace ViaggiaTrentino
{
  public partial class App : Application
  {
    public static PhoneApplicationFrame RootFrame { get; private set; }

    private static MessagePrompt loadingPopup;

    public App()
    {
      InitializeComponent();

     
      if (Debugger.IsAttached)
      {
        Application.Current.Host.Settings.EnableFrameRateCounter = true;
        PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
      }

    }
    
    public static class LoadingPopup
    {
      public static void Show()
      {
        loadingPopup = new MessagePrompt()
        {
          Style = Application.Current.Resources["mpNoBorders"] as Style,
          Body = new LoadingControl()
        };
        loadingPopup.Show();
      }
      public static void Hide()
      {
        loadingPopup.Hide();
      }
      public static bool IsShown()
      {
        return loadingPopup.IsOpen;
      }
    }

  }
}