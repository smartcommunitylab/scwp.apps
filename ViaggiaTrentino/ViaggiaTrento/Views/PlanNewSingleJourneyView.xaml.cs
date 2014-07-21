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

namespace ViaggiaTrentino.Views
{
  public partial class PlanNewSingleJourneyView : PhoneApplicationPage, IHandle<KeyValuePair<string, string>>
  {
    IEventAggregator eventAggregator;

    public PlanNewSingleJourneyView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      this.eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator; 
      eventAggregator.Subscribe(this);
    }

    public void Handle(KeyValuePair<string, string> message)
    {
      if (message.Key == "from")
        FromBox.Text = message.Value;
      else
        ToBox.Text = message.Value;
    }

  
  }
}