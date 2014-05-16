using Caliburn.Micro;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class MonitorJourneyViewModel: Screen
  {
    private readonly INavigationService navigationService;
    ObservableCollection<object> selDays;
    bool isSettingsShown;
    bool isAlways;
    RecurrentJourney journey;
    DateTime beginDate;
    DateTime endDate;
    string fromText;
    string toText;    

    public MonitorJourneyViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
      beginDate = DateTime.Now;
      endDate = DateTime.Now;
      journey = new RecurrentJourney();
      selDays = new ObservableCollection<object>();
      GiorniScelti.Add(DateTime.Now.ToString("dddd"));
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

    public RecurrentJourney Journey
    {
      get { return journey; }
      set
      {
        journey = value;
        NotifyOfPropertyChange(() => Journey);
      }
    }

    
    public ObservableCollection<object> GiorniScelti
    {
      get { return selDays; }
      set
      {
        selDays = value;
        NotifyOfPropertyChange(() => GiorniScelti);
      }
    }

    public DateTime BeginDateTime
    {
      get { return beginDate; }
      set
      {
        beginDate = value;
        NotifyOfPropertyChange(() => BeginDateTime);
      }
    }

    public DateTime EndDateTime
    {
      get { return endDate; }
      set
      {
        endDate = value;
        NotifyOfPropertyChange(() => EndDateTime);
      }
    }

    //big cheat
    public bool CanChoose
    {
      get { return !IsAlways; }
    }

    public bool IsAlways
    {
      get { return isAlways; }
      set
      {
        isAlways = value;
        NotifyOfPropertyChange(() => IsAlways);
        NotifyOfPropertyChange(() => CanChoose);
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
