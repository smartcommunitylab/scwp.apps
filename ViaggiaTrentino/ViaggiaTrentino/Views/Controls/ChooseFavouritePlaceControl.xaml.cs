using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViaggiaTrentino.Helpers;
using Newtonsoft.Json;
using Models.MobilityService.Journeys;
using Coding4Fun.Toolkit.Controls;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class ChooseFavouritePlaceControl : UserControl
  {
    FileStorageHelper fsh;
    MessagePrompt sourceMp;

    public ChooseFavouritePlaceControl(MessagePrompt mp)
    {
      InitializeComponent();
      sourceMp = mp;
      fsh = new FileStorageHelper();
    }

    private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      sourceMp.Tag = lstBxPos.SelectedItem;
      sourceMp.Hide();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      string poses = fsh.ReadFile("favourites.pos");
      if (poses != null)
        lstBxPos.ItemsSource = JsonConvert.DeserializeObject<List<Position>>(poses);
    }
  }
}
