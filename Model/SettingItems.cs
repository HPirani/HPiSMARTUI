/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUI                                               **  
** Description:                                                                  **
** Setting ListView Model (Template).                                            **
**                                                                               **
** Created in mon 1403/03/028 15:30 PM By Hosein Pirani                          **
**                                                                               **
** Modified In mon 1403/03/30 15:00 PM To 20:05 by me.                           **
** :                                                                             **
** TODO:                                                                         **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And  LOT OF CODE @_@                                                          **
** .....                                                                         **  
** ........                                                                      **
** ...........                                                                   **
** ...............                                                               **
 *********************************************************************************/




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPISMARTUI.Model
{
    public class SettingItems
    {
        public string DisplayName
        {
            get; set;
        }
        public string Description
        {
        get; set; }
        public int SetValue //(Value).Renamed to Prevent From Mistaking. 
        {
            get;
            set;
        }
        public int MinimumAllowed //For UI Slider
        {
            get; 
            set;
        }
        public int MaximumAllowed//For UI Slider
        {
            get;
            set;
        }
    }
}
