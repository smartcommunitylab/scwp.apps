using Caliburn.Micro;
using Caliburn.Micro.BindableAppBar;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ViaggiaTrentino.ViewModels;
using ViaggiaTrentino.ViewModels.Controls;


namespace ViaggiaTrentino
{
  public class Bootstrapper : PhoneBootstrapper
  {
    PhoneContainer container;

    protected override void Configure()
    {
      container = new PhoneContainer();

      container.RegisterPhoneServices(RootFrame);
      container.PerRequest<MainPageViewModel>();
      container.PerRequest<TestPageViewModel>();
      container.PerRequest<SavedJourneyViewModel>();
      container.PerRequest<RealTimeInfoViewModel>();

      AddCustomConventions();
    }

    static void AddCustomConventions()
    {
      ConventionManager.AddElementConvention<BindableAppBarMenuItem>(Control.IsEnabledProperty, "DataContext", "Click");
      ConventionManager.AddElementConvention<BindableAppBarButton>(Control.IsEnabledProperty, "DataContext", "Click");
      ConventionManager.AddElementConvention<HubTile>(Control.IsEnabledProperty, "DataContext", "Tap");

    }

    protected override object GetInstance(Type service, string key)
    {
      return container.GetInstance(service, key);
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
