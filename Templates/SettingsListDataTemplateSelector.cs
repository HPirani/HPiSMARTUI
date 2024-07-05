using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HPISMARTUI.Model;
using HPISMARTUI.ViewModel;

namespace HPISMARTUI.Templates
{
   public class SettingsListDataTemplateSelector : DataTemplateSelector
    {
      //  private readonly SettingsViewModel settingsViewModel; 
        public DataTemplate ShouldHaveProgressBar
        {
            get; set;
        }
        public DataTemplate ShouldHavePicker
        {
            get; set;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((SettingItems)item).DisplayName.Equals("Default Active Horn"/*SettingsViewModel.CurrentHornModeName*/) ? ShouldHavePicker : ShouldHaveProgressBar;
        }
    }
}
