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
    
    public PlanNewSingleJourneyViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
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
