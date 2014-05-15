using Caliburn.Micro;
using Models.MobilityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SubmitAlertPageViewModel: Screen
  {

    private readonly INavigationService navigationService;

    public SubmitAlertPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    public AgencyType Agency { get; set; }

  }
}
