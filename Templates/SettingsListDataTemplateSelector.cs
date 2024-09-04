/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/02/25 6:40 PM By Hosein Pirani                            **
**                                                                               **
** Modified In Wed 1403/05/31 02:45 PM To  7:15 by me.                           **
** :                            Minor Fixes.                                     **
** TODO: Test All Methods.                                                       **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And CODE                                                                      **
** ..... More Code                                                               **  
** ........ Code                                                                 **
** ...........  #_#                                                              **
** ...............                                                               **
 *********************************************************************************/














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
