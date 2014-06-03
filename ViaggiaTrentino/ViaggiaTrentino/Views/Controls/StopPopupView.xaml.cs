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
using Models.TerritoryInformationService;
using CommonHelpers;
using Models.MobilityService;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class StopPopupView : UserControl
  {
    private MessagePrompt mp;
    private readonly INavigationService navigationService;

    public StopPopupView(MessagePrompt container, INavigationService navService)
    {
      InitializeComponent();
      mp = container;
      navigationService = navService;
    }

    private void btnRetrieveStopTimes_Click(object sender, RoutedEventArgs e)
    {
      POIObject poi = this.DataContext as POIObject;
      navigationService.UriFor<StopTimesForStopViewModel>()
        .WithParam(x => x.AgencyID, EnumConverter.ToEnum<AgencyType>(poi.CustomData["agencyId"] as string))
        .WithParam(x => x.StopID, poi.CustomData["id"] as string)
        .Navigate();
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      mp.Hide();
    }
  }
}
