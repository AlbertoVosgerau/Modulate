using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace DandyDino.Modulate
{
    public partial class Icons
    {
        private bool _isInsidePackage;
        private string _assemblyName;
        public Icons()
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            if (packageInfo != null)
            {
                _isInsidePackage = true;
                _assemblyName = packageInfo.name;
            }
            else
            {
                _isInsidePackage = false;
            }
        }

        public string RootPath
        {
            get
            {
                if (_isInsidePackage)
                {
                    return $"Packages/{_assemblyName}/Icons";
                }

                return "Assets/DDElements/Editor/Resources/Icons";
            }
        }

        public GUIContent GetIcon(GUIContent icon, string tooltip)
        {
            GUIContent newGuiContent = icon;
            newGuiContent.tooltip = tooltip;
            return newGuiContent;
        }
        
        public GUIContent ArrowDown(string tooltip = "")
        {
            GUIContent item = DefaultUnityIcon("d_ProfilerTimelineDigDownArrow");
            item.tooltip = tooltip;
            return item;
        }
        
        

        private GUIContent DefaultUnityIcon(string icon)
        {
            return EditorGUIUtility.IconContent(icon);
        }
        private GUIContent CustomIcon(string icon, string directory)
        {
            return new GUIContent((Texture2D)EditorGUIUtility.Load($"{directory}/{icon}.png"));
        }
    }
}