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
using Android.Content;
namespace HPISMARTUI.Services
{
    public static class AndroidServiceManager
    {
        public static MainActivity MainActivity
        {
            get; set;
        }

        public static bool IsRunning
        {
            get; set;
        }

        public static void StartMyService()
        {
            if (MainActivity == null)
                return;
            MainActivity.StartService();
        }

        public static void StopMyService()
        {
            if (MainActivity == null)
                return;
            MainActivity.StopService();
            IsRunning = false;
        }
    }
}
