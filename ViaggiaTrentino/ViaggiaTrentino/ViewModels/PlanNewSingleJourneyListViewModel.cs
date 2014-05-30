using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  class PlanNewSingleJourneyListViewModel: Screen
  {
    private readonly INavigationService navigationService;
  
    public PlanNewSingleJourneyListViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }
  
  }
}
