using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ViaggiaTrentino.Views
{
  public partial class SettingsPageView : PhoneApplicationPage
  {
    public SettingsPageView()
    {
      InitializeComponent();
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if ((sender as Pivot).SelectedIndex == 1)
      {
        babAppbar.IsVisible = true;
      }
      else
      {
        babAppbar.IsVisible = false;
      }
    }
  }
}