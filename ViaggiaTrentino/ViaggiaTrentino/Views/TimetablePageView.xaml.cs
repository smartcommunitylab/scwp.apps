using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Caliburn.Micro;
using Models.MobilityService.PublicTransport;
using ViaggiaTrentino.Resources;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading;

namespace ViaggiaTrentino.Views
{
  public partial class TimetablePageView : PhoneApplicationPage, IHandle<CompressedTimetable>
  {
    private readonly IEventAggregator eventAggregator;
    private BackgroundWorker bw;

    public TimetablePageView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
      Previous.IsEnabled = false;
      Next.IsEnabled = false;
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }

    public void Handle(CompressedTimetable ct)
    {
      scrollViewerTimetable.MaxHeight = ContentPanel.ActualHeight;
      columnNames.Width = new GridLength(Application.Current.Host.Content.ActualWidth * 0.4);

      if (ct.CompressedTimes == null)
      {
        stackPanelTimetable.Children.Clear();
      }
      else
      {
        Previous.IsEnabled = false;
        Next.IsEnabled = false;
        for (int i = 0; i < ct.StopIds.Count; i++)
        {
          listBoxNames.Items.Add(ct.Stops[i]);
        }
        stackPanelTimetable.Children.Clear();
        bw = new BackgroundWorker();
        bw.WorkerSupportsCancellation = true;
        bw.WorkerReportsProgress = true;
        bw.DoWork += bw_DoWork;
        bw.ProgressChanged += bw_ProgressChanged;
        bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        bw.RunWorkerAsync(ct);
      }
    }

    void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Previous.IsEnabled = true;
      Next.IsEnabled = true;
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      CompressedTimetable ct = e.Argument as CompressedTimetable;

      List<string> results = new List<string>();
      int i = 0;
      while (i < ct.CompressedTimes.Length)
      {
        if (results.Count == ct.Stops.Count)
        {
          worker.ReportProgress(i, results);
          Thread.Sleep(100);
          results = new List<string>();
        }
        if (ct.CompressedTimes[i] == '|')
        {
          results.Add("");
          i++;
        }
        else
        {
          string s = String.Format("{0}:{1}", ct.CompressedTimes.Substring(i, 2), ct.CompressedTimes.Substring(i + 2, 2));
          results.Add(s);
          i += 4;
        }
      }
    }

    void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      StackPanel sp = new StackPanel();
      List<string> vari = e.UserState as List<string>;
      for (int k = 0; k < vari.Count; k++)
      {
        sp.Children.Add(new TextBlock()
        {
          Margin = new Thickness(10, 3, 10, 3),
          Text = (e.UserState as List<string>)[k],
        });

      }
      stackPanelTimetable.Children.Add(sp);
    }
  }
}