using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class SettingsPageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;

    public SettingsPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
    }

    public bool LocationConsent
    {
      get { return Settings.LocationConsent; }
      set
      {
        Settings.LocationConsent = value;
        NotifyOfPropertyChange(() => LocationConsent);
      }
    }

    

  }
}