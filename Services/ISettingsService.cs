/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/03/026 13:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sat 1403/04/30 10:40 AM To 11:15 by me.                           **
** : RPMReadingInterval Added, Some Minor Fixes.                                 **
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

namespace HPISMARTUI.Services
{
    public interface ISettingsService
    {
        //ECU
        int BlinkersInterval{get; set;}
        int MinIdleRPM{get; set; }
        int MinimumServoAngle{get; set;}
        int MaximumServoAngle{get;set;}
        int CurrentHornMode{get; set;}
        int HeadBlinkInterval{get; set;}
        int HornKeyDebounceDelay{get; set;}
        int RPMreadingInterval{get;set;}
        //App
        int GPSUpdateInterval{get; set;}
        bool IS_ECU_ALive{ get;set;}
    }
}
