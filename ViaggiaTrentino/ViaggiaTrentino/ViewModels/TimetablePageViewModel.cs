using Caliburn.Micro;
using CommonHelpers;
using System.Linq;
using System.Collections.Generic;
using DBManager;
using MobilityServiceLibrary;
using Models.MobilityService;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System;
using Models.MobilityService.PublicTransport;
using System.Collections;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media;
using ViaggiaTrentino.Converters;
using System.Windows.Controls;
using Windows.Storage;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using System.Net;
using ViaggiaTrentino.Views;
using ViaggiaTrentino.Resources;


namespace ViaggiaTrentino.ViewModels
{
  public class TimetablePageViewModel : Screen
  {
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    private AgencyType agencyID;
    private DateTime currentDate;
    private bool enableAppBar, noResults;
    private string routeIDWitDirection, nameID, description, color;
    private ObservableCollection<DBManager.DBModels.RouteName> routeNames;
    DBManager.DBModels.RouteName selectedRouteName;

    public TimetablePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
    {
      this.navigationService = navigationService;
      this.eventAggregator = eventAggregator;
      NoResults = false;
    }

    protected override void OnInitialize()
    {
      base.OnInitialize();
      CurrentDate = DateTime.Now;
    }

    protected override void OnViewLoaded(object view)
    {
      base.OnViewLoaded(view);
      var qs = (navigationService.CurrentContent as TimetablePageView).NavigationContext.QueryString;
      if (qs.ContainsKey("ft"))
      {
        AgencyID = EnumConverter.ToEnum<AgencyType>(qs["AgencyID"]);
        RouteIDWitDirection = qs["RouteIDWitDirection"];
        Description = qs["Description"];
        NameID = qs["NameID"];
        Color = qs["Color"];
      }
      GetTimetableFromDB();
    }

    #region Properties
    public ObservableCollection<DBManager.DBModels.RouteName> RouteNames
    {
      get { return routeNames; }
      set
      {
        routeNames = value;
        NotifyOfPropertyChange(() => RouteNames);
      }
    }

    public AgencyType AgencyID
    {
      get { return agencyID; }
      set { agencyID = value; }
    }

    public DateTime CurrentDate
    {
      get { return currentDate; }
      set
      {
        currentDate = value;
        NotifyOfPropertyChange(() => CurrentDate);
      }
    }

    public string RouteIDWitDirection
    {
      get { return routeIDWitDirection; }
      set { routeIDWitDirection = value; }
    }

    public string NameID
    {
      get { return nameID; }
      set
      {
        nameID = value;
      }
    }

    public string Description
    {
      get { return description; }
      set
      {
        description = value;
      }
    }

    public string Color
    {
      get { return color; }
      set
      {
        color = value;
      }
    }

    public string RouteName
    {
      get { return NameID.Trim() != "" ? string.Format("{0} - {1}", NameID, Description) : Description; }
    }

    public DBManager.DBModels.RouteName SelectedRouteName
    {
      get { return selectedRouteName; }
      set
      {
        selectedRouteName = value;
        NotifyOfPropertyChange(() => SelectedRouteName);
      }
    }

    public bool DisableAppBar
    {
      get { return enableAppBar; }
      set
      {
        enableAppBar = value;
        NotifyOfPropertyChange(() => DisableAppBar);
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
    #endregion

    private void GetTimetableFromDB()
    {
      using (DBHelper dbh = new DBHelper())
      {
        DisableAppBar = false;
        var calendar = dbh.GetCalendar(EnumConverter.ToEnumString<AgencyType>(agencyID), routeIDWitDirection).CalendarEntries;
        var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(calendar);

        string key = CurrentDate.ToString("yyyyMMdd");

        if (results.ContainsKey(key) && results[key] != "null")
        {
          string name = String.Format("{0}_{1}", routeIDWitDirection, results[key]);
          DBManager.DBModels.RouteCalendar timetable;
          try
          {
            timetable = dbh.GetRouteCalendar(name);
            eventAggregator.Publish(new CompressedTimetable()
            {
              CompressedTimes = timetable.Times,
              TripIds = JsonConvert.DeserializeObject<List<string>>(timetable.TripsIDs),
              Stops = JsonConvert.DeserializeObject<List<string>>(timetable.StopsNames),
              StopIds = JsonConvert.DeserializeObject<List<string>>(timetable.StopsIDs),
            });
            NoResults = false;
          }
          catch
          {
            NoResults = true;
            eventAggregator.Publish(new CompressedTimetable()
            {
              CompressedTimes = null
            });
          }
        }
        else
        {
          NoResults = true;
          eventAggregator.Publish(new CompressedTimetable()
          {
            CompressedTimes = null
          });
        }
      }
    }

    #region AppBar

    public void Next()
    {
      CurrentDate = CurrentDate.AddDays(1);
      GetTimetableFromDB();
    }

    public void Current()
    {
      CurrentDate = DateTime.Now;
      GetTimetableFromDB();
    }

    public void Previous()
    {
      CurrentDate = CurrentDate.AddDays(-1);
      GetTimetableFromDB();
    }
    public async void PinToStart()
    {

      string tileUri = String.Format("/Views/TimetablePageView.xaml?AgencyID={0}&RouteIDWitDirection={1}&Description={2}&NameID={3}&Color={4}&ft=t",
        EnumConverter.ToEnumString<AgencyType>(AgencyID),
        RouteIDWitDirection,
        Description,
        NameID,
        Color);

#if DEBUG
      Random rr = new Random();
      tileUri += "&random=" + rr.Next(1, 100);
#endif

      if (ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Equals(tileUri)) != null)
      {
        MessageBox.Show(AppResources.AlreadyPinnedTileError, AppResources.GenericErrorTitle, MessageBoxButton.OK);
        return;
      }

      WriteableBitmap wb = new WriteableBitmap(336, 336);

      #region TimetableTileTemplate
      RouteBackgroungColorToForegroundColorConverter rbctfcc = new RouteBackgroungColorToForegroundColorConverter();
      StringHexColorToColorsConverter hexToColor = new StringHexColorToColorsConverter();
      Color colorConverted = (Color)hexToColor.Convert(color, null, null, System.Globalization.CultureInfo.CurrentUICulture);

      Canvas bg = new Canvas
      {
        Background = new SolidColorBrush(colorConverted),
        Width = wb.PixelWidth,
        Height = wb.PixelHeight
      };

      var foregroundString = rbctfcc.Convert(Color, null, null, System.Globalization.CultureInfo.CurrentUICulture) as string;
      Color foregroundColor = (Color)hexToColor.Convert(foregroundString, null, null, System.Globalization.CultureInfo.CurrentUICulture);
      
      TextBlock lineNumber = new TextBlock
      {
        Foreground = new SolidColorBrush(foregroundColor),
        FontSize = (NameID.Length == 3 ? 150 : 170),
        Text = NameID
      };
      TextBlock mainStops = new TextBlock
      {
        Foreground = new SolidColorBrush(foregroundColor),
        TextWrapping = System.Windows.TextWrapping.Wrap,
        Width = bg.Width - 20,
        FontSize = 35,
        Text = Description
      };

      bg.Children.Add(mainStops);
      bg.Children.Add(lineNumber);

      Canvas.SetLeft(mainStops, 10);
      Canvas.SetTop(mainStops, 10);

      Canvas.SetLeft(lineNumber, bg.Width - lineNumber.ActualWidth - 15);
      Canvas.SetTop(lineNumber, bg.Height - lineNumber.ActualHeight + (NameID.Length == 3 ? 10 : 20 ));

      #endregion

      wb.Render(bg, null);
      wb.Invalidate();

      #region StorageOps
      StorageFolder sharedFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Shared", CreationCollisionOption.OpenIfExists);
      StorageFolder shellContentFolder = await sharedFolder.CreateFolderAsync("ShellContent", CreationCollisionOption.OpenIfExists);

      StorageFile storageFile = await shellContentFolder.CreateFileAsync(
        String.Format("{0}_{1}.jpg", EnumConverter.ToEnumString<AgencyType>(AgencyID), nameID),
        CreationCollisionOption.ReplaceExisting
      );
      string filep = String.Format(@"isostore:\{0}", System.IO.Path.Combine(sharedFolder.Name, shellContentFolder.Name, storageFile.Name));

      #endregion

      using (Stream sw = await storageFile.OpenStreamForWriteAsync())
      {
        wb.SaveJpeg(sw, wb.PixelWidth, wb.PixelHeight, 0, 100);
      };

      StandardTileData NewTileData = new StandardTileData
      {
        BackgroundImage = new Uri(filep, UriKind.Absolute),
      };

      ShellTile.Create(new Uri(tileUri, UriKind.Relative), NewTileData, false);
    }

    #endregion
  }
}