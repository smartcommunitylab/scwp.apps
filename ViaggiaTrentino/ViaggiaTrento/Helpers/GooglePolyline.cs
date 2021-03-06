﻿using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ViaggiaTrentino.Helpers
{
  /// <summary>
  /// Class that handles Google encoded polyline strings and displays them onto a Bing® Maps control
  /// 
  /// Additional reference:
  /// - Encoding algorithm theory https://developers.google.com/maps/documentation/utilities/polylinealgorithm
  /// - Decoding algorithm implementation http://www.codeproject.com/Tips/312248/Google-Maps-Direction-API-V-Polyline-Decoder
  /// </summary>
  public class GooglePolyline
  {
    LocationRectangle locRet;


    /// <summary>
    /// Converts a google polyline string of encoded points into a list of usable GeoCoordinate objects
    /// </summary>
    /// <param name="encodedPoints">an encoded polyline string</param>
    /// <returns>a list of GeoCoordinate objects</returns>
    private List<GeoCoordinate> DecodePolylinePoints(string encodedPoints)
    {
      if (encodedPoints == null || encodedPoints == "") return null;
      List<GeoCoordinate> poly = new List<GeoCoordinate>();
      char[] polylinechars = encodedPoints.ToCharArray();
      int index = 0;

      int currentLat = 0;
      int currentLng = 0;
      int next5bits;
      int sum;
      int shifter;

      try
      {
        while (index < polylinechars.Length)
        {
          // calculate next latitude
          sum = 0;
          shifter = 0;
          do
          {
            next5bits = (int)polylinechars[index++] - 63;
            sum |= (next5bits & 31) << shifter;
            shifter += 5;
          } while (next5bits >= 32 && index < polylinechars.Length);

          if (index >= polylinechars.Length)
            break;

          currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

          //calculate next longitude
          sum = 0;
          shifter = 0;
          do
          {
            next5bits = (int)polylinechars[index++] - 63;
            sum |= (next5bits & 31) << shifter;
            shifter += 5;
          } while (next5bits >= 32 && index < polylinechars.Length);

          if (index >= polylinechars.Length && next5bits >= 32)
            break;

          currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
          GeoCoordinate p = new GeoCoordinate();
          p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
          p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
          poly.Add(p);
        }
      }
      catch (Exception)
      {
        // log it
      }
      return poly;
    }

    /// <summary>
    /// Shows a full screen popup with a map control and a polyline of the provided path.
    /// The map is centered on the start of the path ant the polyline is of the phone accent color
    /// </summary>
    /// <param name="googlePath">LegGeometryInfo.Points</param>
    public void ShowMapWithPath(string googlePath)
    {
      List<GeoCoordinate> lp = DecodePolylinePoints(googlePath);
      MapPolyline mpl = new MapPolyline();
      foreach (GeoCoordinate p in lp)
        mpl.Path.Add(p);
      mpl.StrokeColor = (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color;
      mpl.StrokeThickness = 3;
      Map ggm = new Map();

      ggm.MapElements.Add(mpl);
      ggm.ZoomLevel = 15;
      ggm.Center = lp[0];
      ggm.Height = Application.Current.Host.Content.ActualHeight;
      ggm.Width = Application.Current.Host.Content.ActualWidth;

      MessagePrompt hugeMap = new MessagePrompt();
      hugeMap.Style = Application.Current.Resources["mpNoBorders"] as Style;
      hugeMap.Body = ggm;
      hugeMap.Show();
    }

    /// <summary>
    ///  Shows a full screen popup with a map control and a polyline of the provided path.
    ///  The path is extracted from the collection of legs and is drawn using a cyan brush.
    ///  To highlight the selected leg, it is drawn using a red brush    ///  
    /// </summary>
    /// <param name="legs">a list of Leg items, specifically an ItemCollection taken from the Items property of a Listbox</param>
    /// <param name="selectedLeg">the leg that should be highlighted</param>
    public void ShowMapWithFullPath(ItemCollection legs, Leg selectedLeg)
    {
      // map setup
      Map ggm = new Map();
      ggm.ZoomLevel = 15;
      ggm.Height = Application.Current.Host.Content.ActualHeight;
      ggm.Width = Application.Current.Host.Content.ActualWidth;
      ggm.Loaded += ggm_Loaded;
      MapOverlay mapOverS = new MapOverlay();
      MapOverlay mapOverE = new MapOverlay();
      MapLayer mapLay = new MapLayer();
      Pushpin startPin, endPin;

      foreach (var gamba in legs)
      {
        List<GeoCoordinate> lp = DecodePolylinePoints((gamba as Leg).LegGeometryInfo.Points);

        // check if first leg of the whole path in order to set the appropriate flag
        if (gamba == legs.First())
        {
          startPin = new Pushpin()
          {
            Style = Application.Current.Resources["startPushPin"] as Style,
            GeoCoordinate = new GeoCoordinate(lp.First().Latitude, lp.First().Longitude),
            //Content = new Image() { Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/Assets/Miscs/ic_start.png") }
          };
          mapOverS.PositionOrigin = new Point(0.5, 1);
          mapOverS.Content = startPin;
          mapOverS.GeoCoordinate = startPin.GeoCoordinate;
        }

        // check if last leg of the whole path in order to set the appropriate flag
        // can't use 'else' because one-legged journeys are possible
        if (gamba == legs.Last())
        {
          endPin = new Pushpin()
          {
            Style = Application.Current.Resources["endPushPin"] as Style,
            GeoCoordinate = new GeoCoordinate(lp.Last().Latitude, lp.Last().Longitude),
            //Content = new Image() { Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/Assets/Miscs/ic_stop.png") }
          };
          mapOverE.PositionOrigin = new Point(0.5,1);
          mapOverE.Content = endPin;
          mapOverE.GeoCoordinate = endPin.GeoCoordinate;
        }

        MapPolyline mpl = new MapPolyline();
        mpl.StrokeThickness = 5;

        // add the many lines that compose this leg's polyline
        foreach (GeoCoordinate p in lp)
          mpl.Path.Add(p);

        // if selected leg, colour it red, cyan otherwise
        if (gamba == selectedLeg)
        {
          locRet = LocationRectangle.CreateBoundingRectangle(lp);
          mpl.StrokeColor = Colors.Red;
          ggm.Center = new GeoCoordinate((lp[0].Latitude + lp[lp.Count - 1].Latitude) / 2,
                                (lp[0].Longitude + lp[lp.Count - 1].Longitude) / 2);
        }
        else
          mpl.StrokeColor = Colors.Cyan;

        // add polyline to map
        ggm.MapElements.Add(mpl);
      }

      // add start/end pushpins to map layer
      mapLay.Add(mapOverS);
      mapLay.Add(mapOverE);

      // add layer to map
      ggm.Layers.Add(mapLay);

      // finally displays the map
      MessagePrompt hugeMap = new MessagePrompt();
      hugeMap.Style = Application.Current.Resources["mpNoBorders"] as Style;
      hugeMap.Body = ggm;
      hugeMap.Show();
    }

    // SetView function needs to be called after the map has been added to the visual tree
    // and drawn onscreen. So here it goes
    void ggm_Loaded(object sender, RoutedEventArgs e)
    {
      (sender as Map).SetView(locRet);
    }
  }
}
