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
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace ViaggiaTrentino.Views
{
  public partial class TimetablePageView : PhoneApplicationPage, IHandle<CompressedTimetable>, IHandle<List<Delay>>
  {
    private readonly IEventAggregator eventAggregator;
    private BackgroundWorker bw;
    private StackPanel stackPanelCenter;
    List<Delay> listDelay;
    bool hasType;

    public TimetablePageView()
    {
      InitializeComponent();
      listDelay = null;
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
    {    
      txtNoAvailable.Padding = new Thickness(0, (ContentPanel.ActualHeight - txtNoAvailable.ActualHeight / 2 - bAppBar.ActualHeight) / 2, 0, 0);      
    }



    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }

    private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
    {
      if (bw!= null && bw.IsBusy)
        e.Cancel = true;
    }

    /*
     * Handle for the message sent by the TimeTablePageViewModel after the appropriate timetable is loaded from DB
     * 
     * if the TripIDs collection is present, add "Line" (or its translation) to the listbox of stop names
     * Populates listbox containing stop names with available stop names from the database
     * 
     * Prepare and start background worker, which parses the compressed timetable times and regenerates the proper
     * table (from the four-charachter-or-pipe string to a list of strings). 
     * Each list is the full timetable of a single, unique vehicle.
     * 
     * if the TripIDs collection is present, an additional string indicating the type of transport 
     * (i.e. Regionale, Regionale Veloce, EuroCity, etc) is included at the beginning of each list
     * 
     * After a list is completed, the ProgressChanged event is fired by the backgroundWorker
     * In the function handling this, the freshly created list is converted into a StackPanel and added 
     * to the horizontal listbox in the page, containing all timetable stackpanels.
     * 
     * Time of first departure for the new StackPanel is checked against current time, in order to know which stackpanel is 
     * the closest to the current hour, and is stored in a separate pointer.
     * 
     * After this last population is completed, the horizontal listbox is automatically scrolled to the previously
     * stored closest-time stackpanel
     * 
     */

    public void Handle(CompressedTimetable ct)
    {
      scrollViewerTimetable.MaxHeight = ContentPanel.ActualHeight;
      columnNames.Width = new GridLength(Application.Current.Host.Content.ActualWidth * 0.4);
    
      stackPanelTimetable.Children.Clear();
      listBoxNames.Items.Clear();
      
      
      if (ct.CompressedTimes == null)
      {
        ((TimetablePageViewModel)(this.DataContext)).DisableAppBar = true;
      }
      else
      {
        (stackPanelTimetable.Parent as ScrollViewer).ScrollToHorizontalOffset(0);

        listBoxNames.Items.Add(new TextBlock()
        {
          Foreground = new SolidColorBrush(Colors.Red),
          FontWeight = FontWeights.SemiBold,
          Text = AppResources.SubAlertDelay,
          Margin = new Thickness(0, 3, 0, 5)
        });

        if (ct.TripIds != null)
          hasType = true;

        if (hasType)
          listBoxNames.Items.Add(new TextBlock()
          {
            Foreground = new SolidColorBrush(Colors.Red),
            FontWeight = FontWeights.SemiBold,
            Text = AppResources.TimeTablePageLineType,
            Margin = new Thickness(0, 3, 0, 5)
          });

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

    #region Background Worker

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;
      CompressedTimetable ct = e.Argument as CompressedTimetable;
      stackPanelCenter = null;
      List<string> results = new List<string>();
      int i = 0;
      int j = 0;
      /* 
       * Looks like we always ignored the last column. Here's what was happening:
       * - only 8 charachters (2 times)
       * first step: i = 0 => if = true, read first 4 chars, set i = 4, results.Count = 1
       * second step: i = 4 => if = true, read other 4 chars, set i = i + 4, results.Count = 2
       * third step (here should publish but wait) i = 8 => if false => quit
       * 
       * so I changed the while to allow one more last iteration
       */
      while (i <= ct.CompressedTimes.Length)
      {
        if (results.Count == ct.Stops.Count)
        {
          if (hasType)
          {
            var regex = new Regex("^[a-zA-Z]*");
            var xRegexResult = regex.Match(ct.TripIds[j]).Value;
            List<string> newRess = new List<string>();
            newRess.Add(xRegexResult);
            newRess.AddRange(results);
            results = newRess;
            j++;
          }

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
      
      //shifting index to be used as beginning of actual times
      int indexToUse = 0;

      sp.Children.Add(new TextBlock()
      {
        Margin = new Thickness(10, 3, 10, 5),
        Text = "",
        FontWeight = FontWeights.SemiBold,
        HorizontalAlignment = System.Windows.HorizontalAlignment.Center
      });

      //check if TripIDs is present (usually trains) and adds an extra line in the beginning
      if (hasType)
      {
        sp.Children.Add(new TextBlock()
        {
          Margin = new Thickness(10, 3, 10, 5),
          Text = vari[0],
          FontWeight = FontWeights.SemiBold,
          HorizontalAlignment = System.Windows.HorizontalAlignment.Center
        });
        indexToUse++;
      }

      //select the closest trip time
      if (DateTime.Now.ToString("HH:mm").CompareTo(vari[indexToUse]) != -1 && vari[indexToUse] != "")
        stackPanelCenter = sp;

      //adds stop times to the stackpanel
      for (int k = indexToUse; k < vari.Count; k++)
      {
        sp.Children.Add(new TextBlock()
        {
          Margin = new Thickness(10, 3, 10, 3),
          Text = vari[k],
        });
      }

      //renders the stackpanel
      stackPanelTimetable.Children.Add(sp);
    }

    void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      ((TimetablePageViewModel)(this.DataContext)).DisableAppBar = true;

      if (((TimetablePageViewModel)(this.DataContext)).NoResults)
        return;

      // scrolls to selected position
      ScrollViewer scroll = stackPanelTimetable.Parent as ScrollViewer;
      StackPanel stackColumn = stackPanelCenter as StackPanel;
      var transform = stackColumn.TransformToVisual(scroll).Transform(new Point(0, 0));
      scroll.ScrollToHorizontalOffset(transform.X);

#if DEBUG
      //Debug.WriteLine((stackColumn.Children[0] as TextBlock).Text);
#endif

      if (listDelay != null)
        UpdateTimeTableWithDelays();
    }

    #endregion

    public void Handle(List<Delay> message)
    {
      listDelay = message;
      if (bw != null && !bw.IsBusy)
        UpdateTimeTableWithDelays();
    }

    public void UpdateTimeTableWithDelays()
    {
      for (int spColumn = 0; spColumn < listDelay.Count(); spColumn++)
      {
        TextBlock txBlk = (stackPanelTimetable.Children[spColumn] as StackPanel).Children[0] as TextBlock;
        if(listDelay[spColumn].delayFromService != null)
          txBlk.Inlines.Add(new Run() { Text = listDelay[spColumn].delayFromService, Foreground = new SolidColorBrush(Colors.Red) });
        if(listDelay[spColumn].delayFromUser != null)
          txBlk.Inlines.Add(new Run() { Text = listDelay[spColumn].delayFromUser, Foreground = new SolidColorBrush(Colors.Blue) });

      }
    }

    private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      txtNoAvailable.Padding = new Thickness(0, (ContentPanel.ActualHeight - txtNoAvailable.ActualHeight / 2 - bAppBar.ActualHeight) / 2, 0, 0);      
    }
  }
}