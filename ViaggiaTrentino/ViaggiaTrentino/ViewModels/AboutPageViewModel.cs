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
  }
}
