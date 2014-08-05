using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class SingleDecreesView : UserControl
  {
    public SingleDecreesView()
    {
      InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.MaxHeight = Application.Current.Host.Content.ActualHeight - 10;
      this.MaxWidth = Application.Current.Host.Content.ActualWidth - 10;
    }
  }
}
