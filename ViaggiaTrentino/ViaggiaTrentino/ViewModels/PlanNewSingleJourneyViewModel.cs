using Caliburn.Micro;
using Models.MobilityService.Journeys;
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
    SingleJourney journey;
    DateTime departureDate;
    string fromText;
    string toText;


    
    public PlanNewSingleJourneyViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      departureDate = DateTime.Now;
      journey = new SingleJourney();
      
    }

    public string FromText
    {
      get { return fromText; }
      set
      {
        fromText = value;
        NotifyOfPropertyChange(() => FromText);
      }
    }

    public string ToText
    {
      get { return toText; }
      set
      {
        toText = value;
        NotifyOfPropertyChange(() => ToText);
      }
    }

    public SingleJourney Journey
    {
      get { return journey; }
      set
      {
        journey = value;
        NotifyOfPropertyChange(() => Journey);
      }
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

    public void PlanNewJourney()
    {
      //finalize SingleJourneyObject and proceed to post
    }
  }
}
