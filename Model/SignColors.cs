using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using HPISMARTUI.View;
using HPISMARTUI.ViewModel;
namespace HPISMARTUI.Model
{

    
    public partial class SignColors : ObservableObject
    {
        [ObservableProperty]
       public  ENGINEstate _enginestate = new ENGINEstate();
        public SignColors()
        {
        }

        
        
        public Color SmallLightColor{ get { return Enginestate.IsSmallLight_Enabled ? Colors.Blue : Colors.DarkBlue; } }
        public Color HeadLightLightColor{ get { return Enginestate.IsSmallLight_Enabled ? Colors.Blue : Colors.DarkBlue; } }
        public Color LeftTurnColor{ get { return Enginestate.IsLeftTurn_Enabled ? Colors.Blue : Colors.DarkBlue; } }
        public Color RightTurnColor{ get { return Enginestate.IsRightTurn_Enabled ? Colors.Blue : Colors.DarkBlue; } }
        public Color PoliceLightColor{ get { return Enginestate.IsPOliceLight_Enabled ? Colors.Blue : Colors.DarkBlue; } }



    }
}
