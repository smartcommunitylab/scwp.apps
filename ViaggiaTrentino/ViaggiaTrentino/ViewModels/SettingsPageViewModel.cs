using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SettingsPageViewModel : Screen
  {
    private readonly INavigationService navigationService;

    public SettingsPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }
  }
}
