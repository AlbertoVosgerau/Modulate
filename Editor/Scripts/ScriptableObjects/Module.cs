using System.Collections.Generic;
using System.IO;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Module : ScriptableObject
    {
        public string ModuleName => _moduleName;
        [SerializeField] private string _moduleName;
        
        public string ModulePath => DDElements.Assets.GetAssetPath(this);
        public string ModuleDirectory => Path.GetDirectoryName(ModulePath);
        public string ModuleEditor => $"{System.IO.Path.GetDirectoryName(ModulePath)}/Editor/Scripts";
        public string ModuleScripts => $"{System.IO.Path.GetDirectoryName(ModulePath)}/Scripts";

        public void SetModuleName(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            
            _moduleName = name;
            if (_moduleName.ToLower() == "main")
            {
                return;
            }
            DDElements.Assets.Ping(this);
        }
        
        public void RenameModule(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            Game game = GameInspector.GetGame();
            GameProjectUtils.RenameInWholeProject(_moduleName, name);
            AssetDatabase.RenameAsset(ModulePath, name);
            AssetDatabase.RenameAsset(ModuleDirectory, name);
            DDElements.Assets.SetDirtyAndSave(this);
            _moduleName = name;
        }
        
        public string GetNamespace()
        {
            Game game = GameInspector.GetGame();
            if (game == null)
            {
                return string.Empty;
            }

            return $"{game.GameName}.{ModuleName}";
        }

        public List<(string path, AssemblyDefinition asmdef)> GetAssemblyDefinitions()
        {
            List<string> asmDefFiles = DDElements.Assets.GetAssetPathsInDirectory(ModuleDirectory, "asmdef");
            List<(string path, AssemblyDefinition asmdef)> result = new List<(string path, AssemblyDefinition asmdef)>();

            for (int i = 0; i < asmDefFiles.Count; i++)
            {
                string jsonContent = File.ReadAllText(asmDefFiles[i]);
                result.Add((asmDefFiles[i], AssemblyDefinition.FromJson(jsonContent)));
            }

            return result;
        }
    }
}