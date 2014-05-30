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
using ViaggiaTrentino.ViewModels;
using System.Diagnostics;

namespace ViaggiaTrentino.Views
{
  public partial class TimetablePageView : PhoneApplicationPage, IHandle<CompressedTimetable>
  {
    private readonly IEventAggregator eventAggregator;
    private BackgroundWorker bw;
    private StackPanel stackPanelCenter;

    public TimetablePageView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }

    private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
    {
      if (bw.IsBusy)
        e.Cancel = true;

    }

    public void Handle(CompressedTimetable ct)
    {
      scrollViewerTimetable.MaxHeight = ContentPanel.ActualHeight;
      columnNames.Width = new GridLength(Application.Current.Host.Content.ActualWidth * 0.4);
      var a = ContentPanel.Children.First(x => x.GetType() == typeof(TextBlock));

      if (ct.CompressedTimes == null)
      {
        stackPanelTimetable.Children.Clear();
        listBoxNames.Items.Clear();

        listBoxNames.Visibility = Visibility.Collapsed;
        stackPanelTimetable.Visibility = Visibility.Collapsed;
        txtNoAvailable.Padding = new Thickness(0, (ContentPanel.ActualHeight - txtNoAvailable.ActualHeight / 2 - bAppBar.ActualHeight) / 2, 0, 0);
        txtNoAvailable.Visibility = Visibility.Visible;

        ((TimetablePageViewModel)(this.DataContext)).DisableAppBar = true;
      }
      else
      {
        listBoxNames.Visibility = Visibility.Visible;
        stackPanelTimetable.Visibility = Visibility.Visible;
        txtNoAvailable.Visibility = Visibility.Collapsed;

        listBoxNames.Items.Clear();
        stackPanelTimetable.Children.Clear();
        (stackPanelTimetable.Parent as ScrollViewer).ScrollToHorizontalOffset(0);


        for (int i = 0; i < ct.StopIds.Count; i++)
        {
          listBoxNames.Items.Add(ct.Stops[i]);
        }
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
      ((TimetablePageViewModel)(this.DataContext)).DisableAppBar = true;

      if (txtNoAvailable.Visibility == Visibility.Visible)
        return;


      ScrollViewer scroll = stackPanelTimetable.Parent as ScrollViewer;
      StackPanel stackColumn = stackPanelCenter as StackPanel;
      var transform = stackColumn.TransformToVisual(scroll).Transform(new Point(0, 0));
      scroll.ScrollToHorizontalOffset(transform.X);

#if DEBUG
      Debug.WriteLine((stackColumn.Children[0] as TextBlock).Text);
#endif
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      CompressedTimetable ct = e.Argument as CompressedTimetable;
      stackPanelCenter = null;
      List<string> results = new List<string>();
      int i = 0;
      while (i < ct.CompressedTimes.Length)
      {
        if (results.Count == ct.Stops.Count)
        {
          worker.ReportProgress(i, results);
          Thread.Sleep(50);
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

      //select the closest trip time
      if (DateTime.Now.ToString("HH:mm").CompareTo(vari[0]) != -1 && vari[0] != "")
        stackPanelCenter = sp;

      for (int k = 0; k < vari.Count; k++)
      {
        sp.Children.Add(new TextBlock()
        {
          Margin = new Thickness(10, 3, 10, 3),
          Text = vari[k],
        });

      }
      stackPanelTimetable.Children.Add(sp);
    }
  }
}