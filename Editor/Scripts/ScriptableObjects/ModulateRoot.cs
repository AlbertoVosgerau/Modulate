using System.Collections.Generic;
using DandyDino.Modulate;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ModulateRoot : ScriptableObject
    {
        public string Path => DDElements.Assets.GetAssetPath(this);
        public string Editor => $"{System.IO.Path.GetDirectoryName(Path)}/Editor";
        public string Templates => $"{Editor}/Templates";
        public string Textures => $"{Editor}/Textures";
        public Texture Banner => AssetDatabase.LoadAssetAtPath<Texture>($"{Textures}/Banner.png");
        public Texture Modules => AssetDatabase.LoadAssetAtPath<Texture>($"{Textures}/Modules.png");

        public static ModulateRoot GetCoreRoot()
        {
            return DDElements.Assets.GetFirstAssetInProject<ModulateRoot>();
        }

        public List<string> GetAllTemplates()
        {
            string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { Templates });
        
            List<string> templatesPath = new List<string>();
        
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                if (path.EndsWith(".txt"))
                {
                    templatesPath.Add(path);
                    
                }
            }

            return templatesPath;
        }
    }
}