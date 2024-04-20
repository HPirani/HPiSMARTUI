/**********************************************************************************
**                             HPi Source File                                   **
**    Copyright (C) 2020-2024 HPiStudio. Allrights Reserved.                     **
** ********************************************************************************
** this code is part of HPiUIpro.                                                **  
** Description:                                                                  **
** Serial Input | Output pre-defined Commands.                                   **
** Used For Communicate   Between UI and MicroController.                        **
** Created in sat 1403/01/025 18:40 PM By Hosein Pirani                          **
**                                                                               **
** Modified In sun 1403/01/55 16:00 PM To 19:05 by hosein pirani                 **
**                                                                               **
** TODO: Add Input Serial Commands...                                            **
** TODO: Complete Serial Functions                                               **
** Serial functions                                                              **
** Event Handler For Them,State File writer ,GPS Speedometer,locator And sender  **
** And  LOT OF CODE @_@                                                          **
** ...                                                                           **  
**                                                                               **
**                                                                               **
**                                                                               **
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
    public class Serial_OUTCommands
    {


        public readonly String OutSerial_TurnOffENGINE_cmd = "EOF";
       public readonly String OutSerial_HeadLightOFF_cmd = "HOF";
        public readonly String OutSerial_HeadLightON_cmd = "HON";
        public readonly String OutSerial_HeadBlinkOFF_cmd = "HBF";
        public readonly String OutSerial_PoliceLightON_cmd = "PLO";
        public readonly String OutSerial_PoliceLightOFF_cmd = "PLF";
        public readonly String OutSerial_SetSirenSourceME_cmd = "SSM";
        public readonly String OutSerial_SetSirenSourceYOU_cmd = "SSU";
        public readonly String OutSerial_Serial_GetENGINEstate_cmd = "EST";
        public readonly String OutSerial_GetENGINEtemperature_cmd = "ETM";
        public readonly String OutSerial_GetENGINErpm_cmd = "ERP";
        public readonly String OutSerial_GetFuelLevel_cmd = "FUL";
        public readonly String OutSerial_GetBatteryVoltage = "BAT";
        public readonly String OutSerial_MultiBlinkON_cmd = "MBO";
        public readonly String OutSerial_MultiBlinkOFF_cmd = "MBC";
        public readonly String OutSerial_LeftBlinkON_cmd = "LBO";
               public readonly String OutSerial_LeftBlinkOFF_cmd = "LBC";
        public readonly String OutSerial_RightBlinkON_cmd = "RBO";
        public readonly String OutSerial_RightBlinkOFF_cmd = "RBC";
        public readonly String OutSerial_BlinkerDanceON_cmd = "BDO";
        public readonly String OutSerial_BlinkerDanceOFF_cmd = "BDC";
        public readonly String OutSerial_AllowENGINEstart_cmd = "AES";
        //Commands wich should combined With Numbers(Parameter).
        public readonly String OutSerial_HeadBlinkON_cmd = "HBO:";
        public readonly String OutSerial_SetBlinkInterval_cmd = "SBI:";
        public readonly String OutSerial_AUTOStart_cmd = "ASE:";
        public readonly String OutSerial_SetMinIdleRPM_cmd = "IDR:";
        public readonly String OutSerial_SetMinServoAngle_cmd = "SLA:";
        public readonly String OutSerial_SetMaxServoAngle_cmd = "SHA:";
        public readonly String OutSerial_SetHornMode_cmd = "HRN:";

    
    }

    public class Serial_InCommands
    {
        //NOT IMPLEMENTED
        public readonly String InSerial_STARTUP_cmd = "Im Alive^_^";
        public readonly String InSerial_ENGINEisOFF_cmd = "OFF";
        public readonly String InSerial_ENGINEisON_cmd = "ON";
        public readonly String InSerial_HeadLightIsON_cmd = "ONH";
        public readonly String InSerial_HeadLightIsOFF_cmd = "OFH";
        public readonly String InSerial_HeadBlinkIsON_cmd = "HBO";
        public readonly String InSerial_HeadBlinkIsOFF_cmd = "HBF";
        public readonly String InSerial_LeftTurnIsON_cmd = "LON";
        public readonly String InSerial_LeftTurnIsOFF_cmd = "LOF";
        public readonly String InSerial_RightTurnIsON_cmd = "RON";
        public readonly String InSerial_RightTurnIsOFF_cmd = "ROF";
        public readonly String InSerial_AllBlinkersIsON_cmd = "ABO";
        public readonly String InSerial_AllBlinkersIsOFF_cmd = "ABF";
        public readonly String InSerial_BlinkDanceIsON_cmd = "BDO";
        public readonly String InSerial_BlinkDanceIsOFF_cmd = "BDF";
        public readonly String InSerial_ALarmSourceIsMicro_cmd = "ASM";
        public readonly String InSerial_AlarmSourceIsUI_cmd = "ASU";
        public readonly String InSerial_SirenIsOn_cmd = "SON";
        public readonly String InSerial_SirenIsOFF_cmd = "SOF";
        public readonly String InSerial_PoliceLightsIsOn_cmd = "PON";
        public readonly String InSerial_PoliceLightsIsOFF_cmd = "POF";
        public readonly String InSerial_ShakeDetected_cmd = "WOW";
        public readonly String InSerial_AlarmSilenced_cmd = "NOP";
        //Commands wich should combined With Numbers(Parameter).
        public readonly String InSerial_BatteryVoltage_cmd = "VBT:";
        public readonly String InSerial_ENGINErpm_cmd = "RPM:";
        public readonly String InSerial_ENGINEtemperature_cmd = "TMP:";
        public readonly String InSerial_FuelLevel_cmd = "FuL:";
    }

}
