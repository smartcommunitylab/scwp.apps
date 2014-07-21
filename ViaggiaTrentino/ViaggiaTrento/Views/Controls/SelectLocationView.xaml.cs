using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;

namespace ViaggiaTrentino.Views.Controls
{
  public partial class SelectLocationView : UserControl
  {
    MessagePrompt msgPrompt;
    public SelectLocationView(MessagePrompt mp)
    {
      InitializeComponent();
      msgPrompt = mp;
    }
    
    private void mapPoint_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      msgPrompt.Value = "openMap";
      msgPrompt.Hide();
    }

    private void currentStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      msgPrompt.Value = "current";
      msgPrompt.Hide();
    }
  }
}
