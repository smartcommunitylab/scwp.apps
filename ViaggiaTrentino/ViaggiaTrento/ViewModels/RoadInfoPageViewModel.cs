using Caliburn.Micro;
using MobilityServiceLibrary;
using Models.MobilityService.RealTime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViaggiaTrentino.ViewModels
{
  public class RoadInfoPageViewModel : Screen
  {
    private IEventAggregator eventAggregator;
    private INavigationService navigationService;
    private PublicTransportLibrary ptl;
    private ObservableCollection<AlertRoad> decrees;
    bool noResults;

    private string json;

    public RoadInfoPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
    {
      this.eventAggregator = eventAggregator;
      this.navigationService = navigationService;
      ptl = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
      NoResults = false;
      string j1, j2, j3, j4;
      j1 = "{ \"agencyId\":\"COMUNE_DI_ROVERETO\", \"road\":{\"note\":\"\",\"lat\":\"45.894037\",\"lon\":\"11.043587\",\"streetCode\":\"290\",\"street\":\"CORSO BETTINI A.\",\"fromNumber\":\"\",\"toNumber\":\"\",\"fromIntersection\":\"\",\"toIntersection\":\"\"},\"changeTypes\":[ \"ROAD_BLOCK\"],\"id\":\"612005_290\", \"type\":null, \"entity\":null,  \"description\":\"UFFICIO ATTIVITA' PRODUTTIVE: DIVIETO DI TRANSITO E DI SOSTA CON RIMOZIONE COATTA IN CORSO BETTINI, IN VIALE TRENTO E NELLE STRADE LIMITROFE A ROVERETO PER LO SVOLGIMENTO DEL MERCATO SETTIMANALE DEL MARTEDI'.\",  \"from\":1372543200000,\"to\":1378764000000,  \"creatorId\":\"default\", \"creatorType\":\"SERVICE\", \"effect\":\"Temporanea\", \"note\":null},";
      j2 = "{ \"agencyId\":\"COMUNE_DI_ROVERETO\", \"road\":{\"note\":\"\",\"lat\":\"45.894037\",\"lon\":\"11.043587\",\"streetCode\":\"290\",\"street\":\"CORSO BETTINI A.\",\"fromNumber\":\"\",\"toNumber\":\"\",\"fromIntersection\":\"\",\"toIntersection\":\"\"},\"changeTypes\":[ \"PARKING_BLOCK\",\"ROAD_BLOCK\"],\"id\":\"612005_290\", \"type\":null, \"entity\":null,  \"description\":\"UFFICIO ATTIVITA' PRODUTTIVE: DIVIETO DI TRANSITO E DI SOSTA CON RIMOZIONE COATTA IN CORSO BETTINI'.\",  \"from\":1372543200000,\"to\":1378764000000,  \"creatorId\":\"default\", \"creatorType\":\"SERVICE\", \"effect\":\"Temporanea\", \"note\":null},";
      j3 = "{ \"agencyId\":\"COMUNE_DI_ROVERETO\", \"road\":{\"note\":\"\",\"lat\":\"45.894037\",\"lon\":\"11.043587\",\"streetCode\":\"290\",\"street\":\"CORSO BETTINI A.\",\"fromNumber\":\"\",\"toNumber\":\"\",\"fromIntersection\":\"\",\"toIntersection\":\"\"},\"changeTypes\":[ \"PARKING_BLOCK\",\"ROAD_BLOCK\",\"DRIVE_CHANGE\"],\"id\":\"612005_290\", \"type\":null, \"entity\":null,  \"description\":\"UFFICIO ATTIVITA' PRODUTTIVE'.\",  \"from\":1372543200000,\"to\":1378764000000,  \"creatorId\":\"default\", \"creatorType\":\"SERVICE\", \"effect\":\"Temporanea\", \"note\":null},";
      j4 = "{ \"agencyId\":\"COMUNE_DI_ROVERETO\", \"road\":{\"note\":\"\",\"lat\":\"45.894037\",\"lon\":\"11.043587\",\"streetCode\":\"290\",\"street\":\"CORSO BETTINI A.\",\"fromNumber\":\"\",\"toNumber\":\"\",\"fromIntersection\":\"\",\"toIntersection\":\"\"},\"changeTypes\":[ \"PARKING_BLOCK\",\"ROAD_BLOCK\",\"DRIVE_CHANGE\",\"OTHER\"],\"id\":\"612005_290\", \"type\":null, \"entity\":null,  \"description\":\"UFFICIO ATTIVITA' PRODUTTIVE: DIVIETO DI TRANSITO E DI SOSTA CON RIMOZIONE COATTA IN CORSO BETTINI, IN VIALE TRENTO E NELLE STRADE LIMITROFE A ROVERETO PER LO SVOLGIMENTO DEL MERCATO SETTIMANALE DEL MARTEDI'.\",  \"from\":1372543200000,\"to\":1378764000000,  \"creatorId\":\"default\", \"creatorType\":\"SERVICE\", \"effect\":\"Temporanea\", \"note\":null},";
      json = "[" + j1 + j2 + j3 + j4 + "]";
    }

    private long DateTimeToEpoch(DateTime dt)
    {
      TimeSpan span = (dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
      return Convert.ToInt64(span.TotalSeconds);
    }

    public ObservableCollection<AlertRoad> Decrees
    {
      get { return decrees; }
      set
      {
        decrees = value;
        NotifyOfPropertyChange(() => Decrees);
      }
    }

    public bool NoResults
    {
      get { return noResults; }
      private set
      {
        noResults = value;
        NotifyOfPropertyChange(() => NoResults);
      }
    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      List<AlertRoad> results;
      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        results = await ptl.GetRoadInfoByAgency(Settings.ParkingAgencyId, DateTimeToEpoch(DateTime.Now.AddMonths(-12)), DateTimeToEpoch(DateTime.Now));
        results = JsonConvert.DeserializeObject<List<AlertRoad>>(json);
      }
      finally
      {
        App.LoadingPopup.Hide();
      }

      if (results.Count > 0)
        NoResults = false;
      else
        NoResults = true;

      eventAggregator.Publish(results);
      Decrees = new ObservableCollection<AlertRoad>(results);
    }

    public void OpenSingleDecree(object data)
    {
      
    }

  }
}
