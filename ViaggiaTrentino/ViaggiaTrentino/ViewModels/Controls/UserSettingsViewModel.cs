using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels.Controls
{
  public class UserSettingsViewModel :Screen
  {
    private readonly INavigationService navigationService;

    public UserSettingsViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }
  }
}
