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
  public partial class LoadingControl : UserControl
  {
    public LoadingControl()
    {
      InitializeComponent();
      this.MinHeight = this.MaxHeight = this.Height = Application.Current.Host.Content.ActualHeight;
      this.MinWidth = this.MaxWidth = this.Width = Application.Current.Host.Content.ActualWidth;
    }
    
  }
}
