/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPISMARTUI.                                              **  
** Description:                                                                  **
** Utilities For HPISMARTUI.                                                     **
** Used For Communicate   Between UI and MicroController.                        **
** Created in sat 1403/03/20 16:20 PM By Hosein Pirani                           **
**                                                                               **
** Modified In sun 1403/03/20 16:00 PM To 19:05 by hosein pirani                 **
**                                                                               **
** TODO:                                                                         **
** TODO:                                                                         **
** ..                                                                            **
** ...                                                                           **
** And  LOT OF CODE @_@                                                          **
** .....                                                                         **  
** .......                                                                       **
** ..........                                                                    **
** .............                                                                 **
 *********************************************************************************/




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPISMARTUI
{
  public static  class Utilities
    {
        /// <summary>
        /// Maps Input Range To Another Range
        /// </summary>
        /// <param name="value">SetValue To Be Mapped</param>
        /// <param name="value_min">Minimum Input SetValue</param>
        /// <param name="value_max">Maximum Input SetValue</param>
        /// <param name="out_min">Minimum Output(Maped) SetValue</param>
        /// <param name="out_max">Maximum Output(Maped) SetValue</param>
        /// <returns></returns>
        public static float Map(float value, float value_min, float value_max, float out_min, float out_max)
        {
            return (value - value_min) * (out_max - out_min) / (value_max - value_min) + out_min;
        }

        /// <summary>
        /// Returns Engine's Battery Info, etc 12.3V 99% 
        /// </summary>
        /// <param name="battery_level"></param>
        /// <returns></returns>
        public static String FormatBatteryInfo(int battery_level)
        {
            var BatteryVoltage = Utilities.Map(battery_level, 0, 100, 11.7f, 12.7f);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(BatteryVoltage.ToString());
            stringBuilder.Append("V ");
            stringBuilder.Append(battery_level.ToString());
            stringBuilder.Append('%');
            return stringBuilder.ToString();
        }
    }
}
