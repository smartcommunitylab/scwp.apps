using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using DBManager;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using ViaggiaTrentino.ViewModels;
using ViaggiaTrentino.ViewModels.Controls;
using Windows.Storage;


namespace ViaggiaTrentino
{
  public class Bootstrapper : PhoneBootstrapperBase
  {
    private PhoneApplicationFrame rootFrame;
    private bool reset;
    public PhoneContainer container { get; set; }

    public Bootstrapper()
    {
      Start();
    }

    protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
    {
      rootFrame = new PhoneApplicationFrame();
      return rootFrame;
    }

    protected override void OnActivate(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
    {
      base.OnActivate(sender, e);
    }

    protected override void OnLaunch(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
    {
      base.OnLaunch(sender, e);
    }

    protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
    {
      base.OnStartup(sender, e);
      Settings.Initialize();
      LaunchGPS();
      DBManagement();
    }

    private async void DBManagement()
    {
      StorageFile dbFile = null;
      try
      {
        // Try to get the 
        dbFile = await StorageFile.GetFileFromPathAsync(DBHelper.DB_PATH);
        //dbFile.DeleteAsync();
      }
      catch (FileNotFoundException)
      {
        if (dbFile == null)
        {
          // Copy file from installation folder to local folder.
          // Obtain the virtual store for the application.
          IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

          // Create a stream for the file in the installation folder.
          using (Stream input = System.Windows.Application.GetResourceStream(new Uri("scdb.sqlite", UriKind.Relative)).Stream)
          {
            // Create a stream for the new file in the local folder.
            using (IsolatedStorageFileStream output = iso.CreateFile(DBHelper.DB_PATH))
            {
              // Initialize the buffer.
              byte[] readBuffer = new byte[4096];
              int bytesRead = -1;

              // Copy the file from the installation folder to the local folder. 
              while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
              {
                output.Write(readBuffer, 0, bytesRead);
              }
            }
          }
        }
      }
    }

    private void LaunchGPS()
    {
      if (!Settings.LocationConsent)
        return;
      GeoCoordinateWatcher geolocator = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
      geolocator.StatusChanged += geolocator_StatusChanged;
      geolocator.Start();
    }

    void geolocator_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
    {
      if (e.Status == GeoPositionStatus.Ready)
      {
        Settings.GPSPosition = (sender as GeoCoordinateWatcher).Position.Location;
        (sender as GeoCoordinateWatcher).Stop();
      }
    }

    protected override void Configure()
    {
      container = new PhoneContainer();
      if (!Execute.InDesignMode)
        container.RegisterPhoneServices(RootFrame);

      container.PerRequest<TestPageViewModel>();


      //Pages
      container.PerRequest<MainPageViewModel>();
      container.PerRequest<SavedJourneyPageViewModel>();
      container.PerRequest<SavedJourneyDetailsViewModel>();
      container.PerRequest<RealTimeInfoViewModel>();
      container.PerRequest<SettingsPageViewModel>();
      container.PerRequest<ParkingsPageViewModel>();
      container.PerRequest<PlanNewSingleJourneyViewModel>();
      container.PerRequest<MonitorJourneyViewModel>();
      container.PerRequest<SelectAlertPageViewModel>();
      container.PerRequest<SubmitAlertPageViewModel>();
      container.PerRequest<SelectBusRouteViewModel>();
      container.PerRequest<TimetablePageViewModel>();

      //User controls
      container.PerRequest<SavedRecurrentJourneyViewModel>();
      container.PerRequest<SavedJourneyViewModel>();
      container.PerRequest<UserSettingsViewModel>();
      container.PerRequest<SingleParkingViewModel>();

      AddCustomConventions();

      rootFrame.Navigated += rootFrame_Navigated;
      rootFrame.Navigating += rootFrame_Navigating;
    }

    void rootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
    {
      if (reset && e.IsCancelable && e.Uri.OriginalString == "/Views/MainPage.xaml")
      {
        e.Cancel = true;
        reset = false;
      }
    }

    void rootFrame_Navigated(object sender, NavigationEventArgs e)
    {
      reset = e.NavigationMode == NavigationMode.Reset;
    }

    static void AddCustomConventions()
    {
      ConventionManager.AddElementConvention<BindableAppBarMenuItem>(Control.IsEnabledProperty, "DataContext", "Click");
      ConventionManager.AddElementConvention<BindableAppBarButton>(Control.IsEnabledProperty, "DataContext", "Click");
      ConventionManager.AddElementConvention<HubTile>(Control.IsEnabledProperty, "DataContext", "Tap");
      ConventionManager.AddElementConvention<TextBlock>(Control.IsEnabledProperty, "DataContext", "Tap");
      ConventionManager.AddElementConvention<MenuItem>(Control.IsEnabledProperty, "DataContext", "Tap");

      ConventionManager.AddElementConvention<ListPicker>(Control.IsEnabledProperty, "DataContext", "SelectionChanged");
      ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged")
                .ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                  if (ConventionManager
                      .GetElementConvention(typeof(ItemsControl))
                      .ApplyBinding(viewModelType, path, property, element, convention))
                  {
                    ConventionManager
                        .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                    ConventionManager
                        .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                    return true;
                  }

                  return false;
                };

      ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem",
          "SelectionChanged").ApplyBinding =
          (viewModelType, path, property, element, convention) =>
          {
            if (ConventionManager
                .GetElementConvention(typeof(ItemsControl))
                .ApplyBinding(viewModelType, path, property, element, convention))
            {
              ConventionManager
                  .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
              ConventionManager
                  .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
              return true;
            }

            return false;
          };
    }

    protected override object GetInstance(Type service, string key)
    {
      var instance = container.GetInstance(service, key);
      if (instance != null)
        return instance;

      throw new InvalidOperationException("Could not locate any instances.");
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
      return container.GetAllInstances(service);
    }

    protected override void BuildUp(object instance)
    {
      container.BuildUp(instance);
    }
  }
}
