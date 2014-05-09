using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class PlanNewSingleJourneyViewModel: Screen
  {
    private readonly INavigationService navigationService;
    bool isSettingsShown;
    DateTime departureDate;

    
    public PlanNewSingleJourneyViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      departureDate = DateTime.Now;
    }

    public DateTime DepartureDateTime
    {
      get { return departureDate; }
      set
      {
        departureDate = value;
        NotifyOfPropertyChange(() => DepartureDateTime);
      }
    }

    public bool IsSettingsShown
    {
      get { return isSettingsShown; }
      set 
      {
        isSettingsShown = value;
        NotifyOfPropertyChange(() => IsSettingsShown);

      }
    }
  }
}
