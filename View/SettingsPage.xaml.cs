using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

using HPISMARTUI.Model;
using HPISMARTUI.Services;
using HPISMARTUI.ViewModel;

namespace HPISMARTUI.View;

public partial class SettingsPage : ContentPage
{
    SettingsViewModel vm;
    public SettingsPage(SettingsViewModel viewModel)
	{


        InitializeComponent();
      // BindingContext = new SettingsViewModel(service) ;
        BindingContext = vm =  viewModel;
    }


    protected override void OnAppearing()

    {
        base.OnAppearing();
    }


}