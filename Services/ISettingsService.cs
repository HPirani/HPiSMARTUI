using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPISMARTUI.Services
{
    public interface ISettingsService
    {
        int BlinkersInterval{get; set;}
        int MinIdleRPM{get; set; }
        int MinimumServoAngle{get; set;}
        int MaximumServoAngle{get;set;}
        int CurrentHornMode{get; set;}
        int HeadBlinkInterval{get; set;}
        int HornKeyDebounceDelay{get; set;}
    }
}
