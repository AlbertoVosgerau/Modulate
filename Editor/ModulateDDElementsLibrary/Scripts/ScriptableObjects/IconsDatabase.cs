using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DandyDino.Modulate;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CreateAssetMenu(fileName = "IconsDatabase", menuName = StringLibrary.MENU_ITEM_ICONS_DATABASE)]
    public class IconsDatabase : ScriptableObject
    {
        [SerializeField] private Texture[] textures;
        private string templateClass = "using System.IO;\nusing UnityEngine;\n\n\nnamespace DandyDino.Elements\n{\n    public partial class Icons\n    {\n        private string Path => Directory.Exists(#path#)? #path# : \"Packages/com.dandydino.elements/Icons\";\n        \n#content#    }\n}";

        public void Refresh()
        {
            string path = DDElements.Assets.GetAssetFolder(this);
            textures = DDElements.Assets.GetAssetsInDirectory<Texture>(path).ToArray();
            ProcessTemplate();
            DDElements.Assets.SetDirtyAndSave(this);
            Debug.Log($"Refreshed Icons Database");
        }

        private void ProcessTemplate()
        {
            string directory = DDElements.Assets.GetAssetFolder(this);
            directory = directory.Replace("\\", "/");
            string codeBlocks = string.Empty;
            
            for (int i = 0; i < textures.Length; i++)
            {
                if (textures[i] == null)
                {
                    continue;
                }

                codeBlocks += GetCodeBlock(textures[i].name);
            }

            string classContent = templateClass.Replace("#content#", codeBlocks);
            classContent = classContent.Replace("#path#", $"\"{directory}\"");
            
            string fileName = $"{this.name}";
            string path = Path.Combine(directory, fileName);
            
            DDElements.Assets.CreateClass(directory, fileName, classContent);
            AssetDatabase.ImportAsset($"{path}.cs");
        }

        private string GetCodeBlock(string textureName)
        {
            return
                $"        public GUIContent {textureName.ToPascalCase()}(string tooltip = \"\")\n" +
                "        {\n" +
                $"            GUIContent item = CustomIcon(\"{textureName}\", Path);\n" +
                "            item.tooltip = tooltip;\n" +
                "            return item;\n" +
                "        }\n";
        }
        
        public Texture GetTexture(string name)
        {
            return textures.FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }
    }
}
