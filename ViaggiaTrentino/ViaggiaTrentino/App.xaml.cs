using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino
{
  public partial class App : Application
  {
    public static PhoneApplicationFrame RootFrame { get; private set; }

    public App()
    {
      InitializeComponent();

      if (Debugger.IsAttached)
      {
        Application.Current.Host.Settings.EnableFrameRateCounter = true;
        PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
      }
    }
  }
}