/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUI                                               **  
** Description:                                                                  **
** Setting ListView Model (Template).                                            **
**                                                                               **
** Created in mon 1403/03/028  3:30 PM By Hosein Pirani                          **
**                                                                               **
** Modified In mon 1403/03/30  3:00 PM To  8:05 PM by me.                        **
** :                                                                             **
** TODO:                                                                         **
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
