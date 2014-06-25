using Coding4Fun.Toolkit.Controls;
using Models.MobilityService.Journeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.Helpers
{
  public class FavouriteLocationHelper
  {
    public delegate void FavouriteSelectionCompletedHandler(object sender, Position results);
    public event FavouriteSelectionCompletedHandler FavouriteSelectionCompleted;

    MessagePrompt favouritePrompt;

    public void ShowFavouriteSelector()
    {
      favouritePrompt = new MessagePrompt();
      favouritePrompt.ActionPopUpButtons.Clear();
      favouritePrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      favouritePrompt.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      favouritePrompt.Margin = new System.Windows.Thickness(10);
      favouritePrompt.Title = null;
      favouritePrompt.Body = new ChooseFavouritePlaceControl(favouritePrompt);
      favouritePrompt.Completed += favouritePrompt_Completed;
      favouritePrompt.IsAppBarVisible = false;
      favouritePrompt.Show();
    }

    void favouritePrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
    {
      FavouriteSelectionCompleted(sender, (sender as MessagePrompt).Tag as Position);
    }
  }
}
