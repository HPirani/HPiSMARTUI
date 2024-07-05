/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Serial Input | Output pre-defined Commands.                                   **
**                                                                               **
** Created in sat 1403/01/025 18:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sun 1403/01/26 16:00 PM To 20:05 by me.                           **
** : Splash Video Added.                                                         **
** TODO: Complete Serial Functions.                                              **
** TODO:  & Fix It's Position                                                    **
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
    /// <summary>
    /// Serial Commands From UI To MCU
    /// </summary>
    public static class Serial_OutCommands 
    {


        public static string OutSerial_TurnOffENGINE_cmd => "EOF";
        public static string OutSerial_HeadLightOFF_cmd => "HOF";
        public static string OutSerial_HeadLightON_cmd => "HON";
        public static string OutSerial_HeadBlinkOFF_cmd => "HBF";
        public static string OutSerial_PoliceLightON_cmd => "PLO";
        public static string OutSerial_PoliceLightOFF_cmd => "PLF";
        public static string OutSerial_SetSirenSourceME_cmd => "SSM";
        public static string OutSerial_SetSirenSourceYOU_cmd => "SSU";
        public static string OutSerial_Serial_GetENGINEstate_cmd => "EST";
        public static string OutSerial_GetENGINEtemperature_cmd => "ETM";
        public static string OutSerial_GetENGINErpm_cmd => "ERP";
        public static string OutSerial_GetFuelLevel_cmd => "FUL";
        public static string OutSerial_GetBatteryVoltage => "BAT";
        public static string OutSerial_MultiBlinkON_cmd => "MBO";
        public static string OutSerial_MultiBlinkOFF_cmd => "MBC";
        public static string OutSerial_LeftBlinkON_cmd => "LBO";
        public static string OutSerial_LeftBlinkOFF_cmd => "LBC";
        public static string OutSerial_RightBlinkON_cmd => "RBO";
        public static string OutSerial_RightBlinkOFF_cmd => "RBC";
        public static string OutSerial_BlinkerDanceON_cmd => "BDO";
        public static string OutSerial_BlinkerDanceOFF_cmd => "BDC";
        public static string OutSerial_AllowENGINEstart_cmd => "AES";
        //Commands contains Numbers(Parameter).
        public static string OutSerial_HeadBlinkON_cmd => "HBO:";
        public static string OutSerial_SetBlinkInterval_cmd => "SBI:";
        public static string OutSerial_AUTOStart_cmd => "ASE:"; //Enter Delay For AutoStart Or 100 For Start Until On. -> TODO: REMOVE Manual Start!!!.
        public static string OutSerial_SetMinIdleRPM_cmd => "IDR:";
        public static string OutSerial_SetMinServoAngle_cmd => "SLA:";
        public static string OutSerial_SetMaxServoAngle_cmd => "SHA:";
        public static string OutSerial_SetHornMode_cmd => "HRN:";
        public static string OutSerial_SetHeadBlinkFreq_cmd => "HBD:";//
        public static string OutSerial_SetHornKeyDebounceDelay_cmd => "SHD:";

    }

    public static class Serial_InCommands
    {
        //NOT IMPLEMENTED
        public static string InSerial_STARTUP_cmd => "Im Alive^_^";
        public static string InSerial_ENGINEisOFF_cmd => "OFF";
        public static string InSerial_ENGINEisON_cmd => "ON";
        public static string InSerial_HeadLightIsON_cmd => "ONH";
        public static string InSerial_HeadLightIsOFF_cmd => "OFH";
        public static string InSerial_HeadBlinkIsON_cmd => "HBO";
        public static string InSerial_HeadBlinkIsOFF_cmd => "HBF";
        public static string InSerial_LeftTurnIsON_cmd => "LON";
        public static string InSerial_LeftTurnIsOFF_cmd => "LOF";
        public static string InSerial_RightTurnIsON_cmd => "RON";
        public static string InSerial_RightTurnIsOFF_cmd => "ROF";
        public static string InSerial_AllBlinkersIsON_cmd => "ABO";
        public static string InSerial_AllBlinkersIsOFF_cmd => "ABF";
        public static string InSerial_BlinkDanceIsON_cmd => "BDO";
        public static string InSerial_BlinkDanceIsOFF_cmd => "BDF";
        public static string InSerial_ALarmSourceIsMicro_cmd =>  "ASM";
        public static string InSerial_AlarmSourceIsUI_cmd => "ASU";
        public static string InSerial_SirenIsOn_cmd => "SON";
        public static string InSerial_SirenIsOFF_cmd => "SOF";
        public static string InSerial_PoliceLightsIsOn_cmd => "PON";
        public static string InSerial_PoliceLightsIsOFF_cmd => "POF";
        public static string InSerial_ShakeDetected_cmd => "WOW";
        public static  string InSerial_AlarmSilenced_cmd => "NOP";
        //Commands wich should combined With Numbers(Parameter).
        public static string InSerial_BatteryVoltage_cmd => "VBT:";
        public static string InSerial_ENGINErpm_cmd => "RPM:";
        public static string InSerial_ENGINEtemperature_cmd => "TMP:";
        public static string InSerial_FuelLevel_cmd => "FuL:";
    }

}
