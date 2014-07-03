using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViaggiaTrentino.Views.Controls;

namespace ViaggiaTrentino.Views
{
  public partial class SavedJourneyPageView : PhoneApplicationPage
  {
    public SavedJourneyPageView()
    {
      InitializeComponent();
    }

    private void Saved_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (!(bool)e.NewValue)
      {
        if (sender is SavedJourneyView)
          lonelySingle.Visibility = System.Windows.Visibility.Visible;
        else
          lonelyRecurrent.Visibility = System.Windows.Visibility.Visible;
      }
    }
  }
}