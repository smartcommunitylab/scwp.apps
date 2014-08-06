using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using Caliburn.Micro;
using ViaggiaTrentino.ViewModels;
using Models.MobilityService.PublicTransport;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class DecreePopupView : UserControl
  {
    MessagePrompt mp;

    public DecreePopupView(MessagePrompt container)
    {
      InitializeComponent();
      mp = container;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      mp.Hide();
    }
  }
}
