using System;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using System.Collections.ObjectModel;
using System.Windows;
using MobilityServiceLibrary;
using System.Windows.Controls.Primitives;
using ViaggiaTrentino.Views.Controls;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Phone.Maps.Toolkit;

namespace ViaggiaTrentino.ViewModels
{
  public class ParkingsPageViewModel : Screen
  {
    private readonly IEventAggregator eventAggregator;
    private readonly INavigationService navigationService;
    ObservableCollection<Parking> parkings;
    PublicTransportLibrary publicTransLib;
    

    public ParkingsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;      
      publicTransLib = new PublicTransportLibrary(Settings.AppToken.AccessToken, Settings.ServerUrl);
    }


    List<Parking> parchi;

    protected async override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      await Settings.RefreshToken();
      parchi = await publicTransLib.GetParkingsByAgency(Models.MobilityService.AgencyType.ComuneDiTrento);
      Parkings = new ObservableCollection<Parking>();
      BackgroundWorker bw = new BackgroundWorker();
      bw.RunWorkerCompleted += bw_RunWorkerCompleted;
      bw.DoWork += bw_DoWork;
      bw.ProgressChanged += bw_ProgressChanged;
      bw.WorkerReportsProgress = true;
      bw.WorkerSupportsCancellation = true;
      bw.RunWorkerAsync();
    }

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

    public ObservableCollection<Parking> Parkings
    {
      get { return parkings; }
      set
      {
        parkings = value;
        NotifyOfPropertyChange(() => Parkings);
      }
    }

    public void TappedPushPin(Parking pp)
    {

      MessagePrompt mp = new MessagePrompt();
      mp.Title = null;

      mp.Body = new ParkingPopupView(mp, navigationService) { DataContext = pp };
      mp.ActionPopUpButtons.Clear();

      mp.VerticalAlignment = VerticalAlignment.Center;
      mp.HorizontalAlignment = HorizontalAlignment.Center;
      mp.Show();
    }

  }
}
