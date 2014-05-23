using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class AboutPageViewModel: Screen
  {
     private readonly INavigationService navigationService;

    public AboutPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;

    }
  }
}
