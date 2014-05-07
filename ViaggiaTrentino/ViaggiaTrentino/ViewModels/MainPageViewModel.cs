using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViaggiaTrentino.ViewModels
{
  public class MainPageViewModel : PropertyChangedBase
  {
    private string name;
    private bool isEnabled;


    public MainPageViewModel()
    {
      name = "COOOOOOOP";
      //MessageBox.Show("I'm the MainView!");
    }
    public bool IsEnabled
    {
      get { return isEnabled; }
      set
        {
            isEnabled = value;
            NotifyOfPropertyChange(() => IsEnabled);
            NotifyOfPropertyChange(() => CanShowPower);
        }
    }
    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        NotifyOfPropertyChange(() => Name);
      }
    }
    
    public bool CanShowPower
    {
      get { return IsEnabled; }
    }
    public void ShowPower()
    {
      MessageBox.Show("moar power");
    }



  }
}
