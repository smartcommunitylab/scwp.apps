using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;

namespace ViaggiaTrentino.Views
{
  public partial class MainPageView : PhoneApplicationPage, IHandle<bool>
  {
    private IEventAggregator eventAggregator;

    public MainPageView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);

    }
    public void Handle(bool message)
    {
      babMainPage.IsVisible = message;
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }
  }
}