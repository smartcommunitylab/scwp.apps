using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Maps.Controls;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ViaggiaTrentino.Helpers
{
  public class GooglePolyline
  {
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
      catch (Exception ex)
      {
        // log it
      }
      return poly;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="googlePath">LegGeometryInfo.Points</param>
    public void ShowMapWithPath(string googlePath)
    {
      List<GeoCoordinate> lp = DecodePolylinePoints(googlePath);
      MapPolyline mpl = new MapPolyline();
      foreach (GeoCoordinate p in lp)
        mpl.Path.Add(p);
      mpl.StrokeColor =   (App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color;
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

    public void ShowMapWithFullPath(ItemCollection legs, Leg selectedLeg)
    {
      Map ggm = new Map();
      LocationRectangle locRet = new LocationRectangle();
      ggm.ZoomLevel = 15;
      ggm.Height = Application.Current.Host.Content.ActualHeight;
      ggm.Width = Application.Current.Host.Content.ActualWidth;

      foreach (var gamba in legs)
      {
        List<GeoCoordinate> lp = DecodePolylinePoints((gamba as Leg).LegGeometryInfo.Points);
        MapPolyline mpl = new MapPolyline();
        mpl.StrokeThickness = 3;
        
        foreach (GeoCoordinate p in lp)
          mpl.Path.Add(p);
        if (gamba == selectedLeg)
        {
          mpl.StrokeColor = Colors.Red;
          GeoCoordinate NorhWest = new GeoCoordinate( Math.Max(lp[0].Latitude, lp[lp.Count - 1].Latitude), Math.Min(lp[0].Longitude, lp[lp.Count - 1].Longitude) );
          GeoCoordinate SouthEast = new GeoCoordinate( Math.Min(lp[0].Latitude, lp[lp.Count - 1].Latitude), Math.Max(lp[0].Longitude, lp[lp.Count - 1].Longitude) );
          locRet = new LocationRectangle(NorhWest, SouthEast);
          ggm.Center = new GeoCoordinate((lp[0].Latitude + lp[lp.Count - 1].Latitude) / 2,
                                (lp[0].Longitude + lp[lp.Count - 1].Longitude) / 2);
        }
        else
          mpl.StrokeColor = Colors.Cyan;

        ggm.MapElements.Add(mpl);
      }     

      MessagePrompt hugeMap = new MessagePrompt();
      hugeMap.Style = Application.Current.Resources["mpNoBorders"] as Style;
      hugeMap.Body = ggm;
      hugeMap.Show();
      ggm.SetView(locRet);
    }
  }
}
