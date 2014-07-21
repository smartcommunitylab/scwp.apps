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
using System.Text.RegularExpressions;

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

    public void Handle(IEnumerable<KeyedList<string, TripData>> sms)
    {
      var message = sms.ToList();
      message.Sort(new CustomComparer());

      if (message.ToList().Count > 0)
        lls.ItemsSource = message.ToList();
      else
      {
        txtNoAvailable.Visibility = System.Windows.Visibility.Visible;
      }
    }
  }
  public class CustomComparer : IComparer<KeyedList<string, TripData>>
  {
    public int Compare(KeyedList<string, TripData> x, KeyedList<string, TripData> y)
    {
      int o1, o2;

      //if both are numbers
      if(Int32.TryParse(x.First().RouteShortName, out o1) && Int32.TryParse(y.First().RouteShortName, out o2))
        return o1.CompareTo(o2);
      
      // otherwise return as string comparison
      return x.First().RouteShortName.CompareTo(y.First().RouteShortName);
    }
  }
}