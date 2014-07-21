using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Text;
using ViaggiaTrentino.Resources;

namespace ViaggiaTrentino.ViewModels
{
  public class AboutPageViewModel: Screen
  {
     private readonly INavigationService navigationService;

    public AboutPageViewModel(INavigationService navigationService)
    {
      this.navigationService = navigationService;
    }

    public void ThirdAbout()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("• Microsoft HttpClient");
      sb.AppendLine("• Microsoft RateMyApp");
      sb.AppendLine("• Newtonsoft JSON.NET");
      sb.AppendLine("• Caliburn Micro framework");
      sb.AppendLine("• Coding4Fun toolkit");
      sb.AppendLine("• SQLite for Windows Phone");
      sb.AppendLine("• Andrea Panizza GuidedTour");

      CustomMessageBox cmb = new CustomMessageBox()
      {
        Caption = AppResources.AboutPageThird,
        Message = AppResources.AboutPageThirdParty,
        Content = sb.ToString(),
        LeftButtonContent = AppResources.ValidationBtnOk
      };
      cmb.Show();
    }

    public void RateApp()
    {
      MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
      marketplaceReviewTask.Show();
    }

    public void sclabLink()
    {
      WebBrowserTask wbt = new WebBrowserTask();
      wbt.Uri = new System.Uri("http://www.smartcampuslab.eu/");
      wbt.Show();
    }

    public void comuneLink()
    {
      WebBrowserTask wbt = new WebBrowserTask();
      wbt.Uri = new System.Uri("http://www.comune.trento.it/");
      wbt.Show();
    }
    
    public void innovazioneLink()
    {
      WebBrowserTask wbt = new WebBrowserTask();
      wbt.Uri = new System.Uri("http://www.innovazione.comunitrentini.tn.it/");
      wbt.Show();
    }
  }
}
