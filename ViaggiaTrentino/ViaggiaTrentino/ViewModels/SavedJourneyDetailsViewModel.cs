using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class SavedJourneyDetailsViewModel : Screen
  {
    private readonly INavigationService navigationService;
    public SavedJourneyDetailsViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }
  }
}
