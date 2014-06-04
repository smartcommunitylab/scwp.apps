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

    public void Handle(IEnumerable<KeyedList<string, TripData>> message)
    {
      message.ToList().Sort(new CustomComparer());


      lls.ItemsSource = message.ToList();
    }
  }
  public class CustomComparer : IComparer<KeyedList<string, TripData>>
  {
    public int Compare(KeyedList<string, TripData> x, KeyedList<string, TripData> y)
    {
      var regex = new Regex("^(d+)");

      // run the regex on both strings
      var xRegexResult = regex.Match(x.First().RouteShortName);
      var yRegexResult = regex.Match(y.First().RouteShortName);
      int o1, o2;
      if(Int16.TryParse(x.First().RouteShortName, o1))
      // check if they are both numbers
      if (xRegexResult.Success && yRegexResult.Success)
      {
        return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
      }

      // otherwise return as string comparison
      return x.First().RouteShortName.CompareTo(y.First().RouteShortName);
    }
  }
}