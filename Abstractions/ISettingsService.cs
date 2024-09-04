/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiSMARTUi                                               **  
** Description:                                                                  **
**                                                                               **
**                                                                               **
** Created in sat 1403/03/026 1:40 PM By Hosein Pirani                           **
**                                                                               **
** Modified In sat 1403/05/31 2:45 PM To 3:00 PM by me.                          **
** : TimerResetInterval Added, .                                                 **
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
        double Trip{get;set;}
        int GPSUpdateInterval{get; set;}
        int TimerResetInterval{get; set;}
        int GPSLocationAccuracy{get; set;}
        int GPSLocationRequestInterval{get; set;}
        bool IS_ECU_ALive{ get;set;}
    }
}
