using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using GuidedTour;
using RateMyApp.Controls;
using System.Windows.Media;

namespace ViaggiaTrentino.Views
{
  public partial class MainPageView : PhoneApplicationPage, IHandle<bool>
  {
    private IEventAggregator eventAggregator;
    bool isSubscribed;
    Tour tour;

    public MainPageView()
    {
      isSubscribed = false;
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
      isSubscribed = true;

#if TRENTO
      HubTileService.FreezeGroup("RoveretoApp");
      RealTimeInfoTile.Visibility = System.Windows.Visibility.Collapsed;
#else
      HubTileService.FreezeGroup("TrentoApp");
      TrentoBusTile.Visibility = System.Windows.Visibility.Collapsed;
      TrainTile.Visibility = System.Windows.Visibility.Collapsed;
      ParkingTile.Visibility = System.Windows.Visibility.Collapsed;
#endif
    }

    #region Tutorial

    public void Handle(bool message)
    {
      babMainPage.IsVisible = message;
      ManageTour(false);
    }


    void ManageTour(bool overrideCheck)
    {
      if (overrideCheck || (!Settings.IsTourAlreadyShown && Settings.IsLogged))
      {
        tour = new Tour(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
        tour.Clear();

        foreach (HubTile ht in HubTilePanel.Children.OfType<HubTile>())
          if(ht.Visibility == System.Windows.Visibility.Visible)
            tour.Add(new TourElement(LayoutRoot, ht, (ht as HubTile).Tag as string, (ht as HubTile).Title, new Point(0,32)));

        tour.TourStarted += tour_TourStarted;
        tour.TourCompleted += tour_TourCompleted;
        tour.TourProgressChanged += tour_TourProgressChanged;

        tour.Start();
      }
    }


    void tour_TourCompleted()
    {
      babMainPage.IsVisible = Settings.IsTourAlreadyShown = true;
      ContentPanel.Children.OfType<ScrollViewer>().First().ScrollToVerticalOffset(0);
    }

    //do not delete this function, even if its empty
    void tour_TourProgressChanged(int percentage)
    {
      ScrollViewer sv = ContentPanel.Children.OfType<ScrollViewer>().First();
      if (tour.CurrentElement.Position.Y + tour.CurrentElement.Height > Application.Current.Host.Content.ActualHeight)
      {
        if (tour.CurrentElement.Position.Y + tour.CurrentElement.Height > Application.Current.Host.Content.ActualHeight + sv.VerticalOffset)
          sv.ScrollToVerticalOffset(tour.CurrentElement.Position.Y - tour.CurrentElement.Height - 32);

        tour.CurrentElement.Position = new Point(tour.CurrentElement.Position.X, tour.CurrentElement.Position.Y - sv.VerticalOffset);
      }
    }

    void tour_TourStarted()
    {
      babMainPage.IsVisible = false;
    }

    #endregion

    private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
    {
      if (!isSubscribed)
        eventAggregator.Subscribe(this);
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
      isSubscribed = false;
    }

    private void BarTour_Click(object sender, EventArgs e)
    {
      ManageTour(true);
    }

    private void feedbackOverlay_VisibilityChanged(object sender, EventArgs e)
    {
      if ((sender as FeedbackOverlay).Visibility == System.Windows.Visibility.Visible)
        SystemTray.BackgroundColor = (Color)Application.Current.Resources["PhoneChromeColor"];
      else
        SystemTray.BackgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
    }


  }
}