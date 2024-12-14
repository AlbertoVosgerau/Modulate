using System.IO;
using UnityEngine;


namespace DandyDino.Modulate
{
    public partial class Icons
    {
        private string Path => Directory.Exists("Assets/PackageProjects/Modulate/Editor/ModulateDDElements/Icons")? "Assets/PackageProjects/Modulate/Editor/ModulateDDElements/Icons" : "Packages/com.dandydino.elements/Icons";
        
        public GUIContent BaseSize(string tooltip = "")
        {
            GUIContent item = CustomIcon("base-size", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent BluishPlus(string tooltip = "")
        {
            GUIContent item = CustomIcon("bluish-plus", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent CSharp(string tooltip = "")
        {
            GUIContent item = CustomIcon("c-sharp", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent CheckMark(string tooltip = "")
        {
            GUIContent item = CustomIcon("check-mark", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Check(string tooltip = "")
        {
            GUIContent item = CustomIcon("check", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Circle(string tooltip = "")
        {
            GUIContent item = CustomIcon("circle", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent CogWheel(string tooltip = "")
        {
            GUIContent item = CustomIcon("cog-wheel", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent ColorEnvelope(string tooltip = "")
        {
            GUIContent item = CustomIcon("color-envelope", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent ColorGear(string tooltip = "")
        {
            GUIContent item = CustomIcon("color-gear", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent ColorScript(string tooltip = "")
        {
            GUIContent item = CustomIcon("color-script", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Comment(string tooltip = "")
        {
            GUIContent item = CustomIcon("comment", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Copy(string tooltip = "")
        {
            GUIContent item = CustomIcon("copy", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Cubes(string tooltip = "")
        {
            GUIContent item = CustomIcon("cubes", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Dash(string tooltip = "")
        {
            GUIContent item = CustomIcon("dash", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Delete(string tooltip = "")
        {
            GUIContent item = CustomIcon("delete", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Dinosaur(string tooltip = "")
        {
            GUIContent item = CustomIcon("dinosaur", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Dna(string tooltip = "")
        {
            GUIContent item = CustomIcon("dna", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent EmptyCheck(string tooltip = "")
        {
            GUIContent item = CustomIcon("empty-check", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Enterprise(string tooltip = "")
        {
            GUIContent item = CustomIcon("enterprise", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Enumerate(string tooltip = "")
        {
            GUIContent item = CustomIcon("enumerate", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Envelope(string tooltip = "")
        {
            GUIContent item = CustomIcon("envelope", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Focus(string tooltip = "")
        {
            GUIContent item = CustomIcon("focus", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Folder(string tooltip = "")
        {
            GUIContent item = CustomIcon("folder", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent GreenAdd(string tooltip = "")
        {
            GUIContent item = CustomIcon("green-add", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent GreenSquareAdd(string tooltip = "")
        {
            GUIContent item = CustomIcon("green-square-add", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent HomeDefault(string tooltip = "")
        {
            GUIContent item = CustomIcon("home-default", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Joystick(string tooltip = "")
        {
            GUIContent item = CustomIcon("joystick", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Layers(string tooltip = "")
        {
            GUIContent item = CustomIcon("layers", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Logout(string tooltip = "")
        {
            GUIContent item = CustomIcon("logout", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Magic(string tooltip = "")
        {
            GUIContent item = CustomIcon("magic", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Monkey(string tooltip = "")
        {
            GUIContent item = CustomIcon("monkey", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Picture(string tooltip = "")
        {
            GUIContent item = CustomIcon("picture", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Process(string tooltip = "")
        {
            GUIContent item = CustomIcon("process", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Remove1(string tooltip = "")
        {
            GUIContent item = CustomIcon("remove 1", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Remove(string tooltip = "")
        {
            GUIContent item = CustomIcon("remove", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent RocketColor(string tooltip = "")
        {
            GUIContent item = CustomIcon("rocket-color", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent ScriptGray(string tooltip = "")
        {
            GUIContent item = CustomIcon("script-gray", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Script(string tooltip = "")
        {
            GUIContent item = CustomIcon("script", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Search(string tooltip = "")
        {
            GUIContent item = CustomIcon("search", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Selection(string tooltip = "")
        {
            GUIContent item = CustomIcon("selection", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent StylizedArrowDownGreen(string tooltip = "")
        {
            GUIContent item = CustomIcon("stylized-arrow-down-green", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent StylizedArrowRightGreen(string tooltip = "")
        {
            GUIContent item = CustomIcon("stylized-arrow-right-green", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent StylizedArrowUpGreen(string tooltip = "")
        {
            GUIContent item = CustomIcon("stylized-arrow-up-green", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent SwitchOff(string tooltip = "")
        {
            GUIContent item = CustomIcon("switchOff", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent SwitchOn(string tooltip = "")
        {
            GUIContent item = CustomIcon("switchOn", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Trash(string tooltip = "")
        {
            GUIContent item = CustomIcon("trash", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Unity(string tooltip = "")
        {
            GUIContent item = CustomIcon("unity", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent Window(string tooltip = "")
        {
            GUIContent item = CustomIcon("window", Path);
            item.tooltip = tooltip;
            return item;
        }
        public GUIContent XButton(string tooltip = "")
        {
            GUIContent item = CustomIcon("x-button", Path);
            item.tooltip = tooltip;
            return item;
        }
    }
}