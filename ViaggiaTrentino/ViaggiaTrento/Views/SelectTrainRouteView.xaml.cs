using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Caliburn.Micro;
using TerritoryInformationServiceLibrary;
using Models.TerritoryInformationService;
using ViaggiaTrentino.ViewModels;
using Microsoft.Phone.Maps.Toolkit;
using System.Device.Location;
using ViaggiaTrentino.Model;
using CommonHelpers;
using Models.MobilityService;
using System.Diagnostics;
using Microsoft.Phone.Maps.Controls;

namespace ViaggiaTrentino.Views
{
  public partial class SelectTrainRouteView : PhoneApplicationPage
  {
    public SelectTrainRouteView()
    {
      InitializeComponent();
      
    }
  }
}