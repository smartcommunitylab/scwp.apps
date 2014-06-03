using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Models.TerritoryInformationService;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using ViaggiaTrentino.Helpers;

namespace ViaggiaTrentino.Views
{
  public partial class StopTimesForStopView : PhoneApplicationPage, IHandle<IEnumerable<KeyedList<string, TripData>>>
  {
    private IEventAggregator eventAggregator;
    public StopTimesForStopView()
    {
      InitializeComponent();

      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }

    public void Handle(IEnumerable<KeyedList<string, TripData>> message)
    {
      lls.ItemsSource = message.ToList();
    }
  }
}