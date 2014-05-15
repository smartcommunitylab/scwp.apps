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
  public partial class ParkingPopupView : UserControl
  {
    MessagePrompt mp;
    private readonly INavigationService navigationService;

    public ParkingPopupView(MessagePrompt container, INavigationService navService)
    {
      InitializeComponent();
      mp = container;
      navigationService = navService;
    }

    private void btnDirections_Click(object sender, RoutedEventArgs e)
    {
      Parking park = this.DataContext as Parking;
      navigationService.UriFor<PlanNewSingleJourneyViewModel>().WithParam(x => x.Position, park.Position).Navigate();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      mp.Hide();
    }
  }
}
