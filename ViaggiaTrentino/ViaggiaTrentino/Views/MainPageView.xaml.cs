﻿using System;
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

namespace ViaggiaTrentino.Views
{
  public partial class MainPageView : PhoneApplicationPage, IHandle<bool>
  {
    private IEventAggregator eventAggregator;

    public MainPageView()
    {
      InitializeComponent();
      Bootstrapper bootstrapper = Application.Current.Resources["bootstrapper"] as Bootstrapper;
      IEventAggregator eventAggregator = bootstrapper.container.GetAllInstances(typeof(IEventAggregator)).FirstOrDefault() as IEventAggregator;
      this.eventAggregator = eventAggregator;
      eventAggregator.Subscribe(this);
    }
    public void Handle(bool message)
    {
      babMainPage.IsVisible = message;
      ManageTour(false);
    }


    void ManageTour(bool overrideCheck)
    {
      if (overrideCheck || (!Settings.IsTourAlreadyShown && Settings.IsLogged))
      {
        Tour tour = new Tour(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);

        foreach (HubTile ht in (ContentPanel.Children.OfType<Grid>().First() as Grid).Children.OfType<HubTile>())
          tour.Add(new TourElement(LayoutRoot, ht, (ht as HubTile).Tag, (ht as HubTile).Title));

        tour.TourStarted += tour_TourStarted;
        tour.TourCompleted += tour_TourCompleted;
        tour.TourProgressChanged += tour_TourProgressChanged;

        tour.Start();
      }
    }


    void tour_TourCompleted()
    {
      babMainPage.IsVisible = SystemTray.IsVisible = Settings.IsTourAlreadyShown = true;
    }

    //do not delete this function, even if its empty
    void tour_TourProgressChanged(int percentage)
    {
    }

    void tour_TourStarted()
    {
      babMainPage.IsVisible = SystemTray.IsVisible = false;
    }

    private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
    {
      eventAggregator.Unsubscribe(this);
    }

    private void BarTour_Click(object sender, EventArgs e)
    {
      ManageTour(true);
    }

  }
}