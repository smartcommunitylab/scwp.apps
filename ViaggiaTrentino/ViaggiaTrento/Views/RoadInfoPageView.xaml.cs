using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Models.MobilityService.PublicTransport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using ViaggiaTrentino.Model;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.Views
{
  public partial class RoadInfoPageView : PhoneApplicationPage
  {
    private IEventAggregator eventAggregator;

    public RoadInfoPageView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      this.eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      eventAggregator.Subscribe(this);
    }
  }
}