using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DandyDino.Modulate
{
    public partial class Icons
    {
        public GUIContent Locked(string tooltip = "")
        {
            GUIContent item =  DefaultUnityIcon("IN LockButton on@2x");
            item.tooltip = tooltip;
            return item;
        }
        
        public GUIContent Unlocked(string tooltip = "")
        {
            GUIContent item =  DefaultUnityIcon("IN LockButton@2x");
            item.tooltip = tooltip;
            return item;
        }
        
        public GUIContent Warning(string tooltip = "")
        {
            GUIContent item =  DefaultUnityIcon("console.warnicon");
            item.tooltip = tooltip;
            return item;
        }
    }
}