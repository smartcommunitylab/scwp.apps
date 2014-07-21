using Caliburn.Micro;
using Coding4Fun.Toolkit.Controls;
using MobilityServiceLibrary;
using Models.MobilityService.PublicTransport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.ViewModels
{
  public class ParkingsPageViewModel : Screen
  {
    private readonly IEventAggregator eventAggregator;
    private readonly INavigationService navigationService;
    ObservableCollection<Parking> parkings;
    PublicTransportLibrary publicTransLib;
    List<Parking> parchi;

    public ParkingsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;      
      publicTransLib = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }

    public ObservableCollection<Parking> Parkings
    {
      get { return parkings; }
      set
      {
        parkings = value;
        NotifyOfPropertyChange(() => Parkings);
      }
    }

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      try
      {
        App.LoadingPopup.Show();
        await Settings.RefreshToken();
        parchi = await publicTransLib.GetParkingsByAgency(Settings.ParkingAgencyId);
      }
      finally
      {
        App.LoadingPopup.Hide();
      }
     
      Parkings = new ObservableCollection<Parking>();
      BackgroundWorker bw = new BackgroundWorker();
      bw.RunWorkerCompleted += bw_RunWorkerCompleted;
      bw.DoWork += bw_DoWork;
      bw.ProgressChanged += bw_ProgressChanged;
      bw.WorkerReportsProgress = true;
      bw.WorkerSupportsCancellation = true;
      bw.RunWorkerAsync();
    }

    #region Parking retrieval

    void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {  
      eventAggregator.Publish(Parkings);
    }

    void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      Parkings.Add(e.UserState as Parking);
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      
      for (int i = 0; i < parchi.Count; i++)
      {

        (sender as BackgroundWorker).ReportProgress((i / parchi.Count) * 100, parchi[i]);
        Thread.Sleep(50);
      }
    }

    #endregion

    #region Map pushpins

    public void TappedPushPin(Parking pp)
    {

      MessagePrompt mp = new MessagePrompt();
      mp.Style = Application.Current.Resources["mpNoTitleNoButtons"] as Style;
      mp.Body = new ParkingPopupView(mp, navigationService) { DataContext = pp };
      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.HorizontalAlignment = HorizontalAlignment.Center;
      mp.Show();
    }

    #endregion
  }
}
