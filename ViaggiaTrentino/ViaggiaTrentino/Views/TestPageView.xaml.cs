using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.Windows.Controls.Primitives;

namespace ViaggiaTrentino.Views
{
  public partial class TestPageView : PhoneApplicationPage
  {
    public TestPageView()
    {
      InitializeComponent();
    }

    private void show_Click(object sender, RoutedEventArgs e)
    {
      App.LoadingPopup.Show();
    }

    protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
    {
      if (App.LoadingPopup.IsShown())
        e.Cancel = true;
      base.OnBackKeyPress(e);
    }
  }
}