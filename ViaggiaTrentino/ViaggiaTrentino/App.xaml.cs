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

namespace ViaggiaTrentino
{
  public partial class App : Application
  {
    public static PhoneApplicationFrame RootFrame { get; private set; }

    private static Popup loadingPopup;

    public App()
    {
      InitializeComponent();

      loadingPopup = new Popup()
      {
        Child = new LoadingControl()
      };

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
        loadingPopup.IsOpen = true;
      }
      public static void Hide()
      {
        loadingPopup.IsOpen = false;
      }
      public static bool IsShown()
      {
        return loadingPopup.IsOpen;
      }
    }

  }
}