using System;
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
        public string ModuleEditorScriptsDirectory => $"{ModuleDirectory}/Editor/Scripts";
        public string ModuleScriptsDirectory => $"{ModuleDirectory}/Scripts";
        
        public string MonoBehaviourDirectory => $"{ModuleScriptsDirectory}/MonoBehaviours";
        public string EventsDirectory => $"{ModuleScriptsDirectory}/Events";
        public string ManagerDirectory => $"{ModuleScriptsDirectory}/Manager";
        public string ServiceDirectory => $"{ModuleScriptsDirectory}/Service";

        public string AssemblyDefinitionAssetPath => $"{ModuleDirectory}/{GameInspector.GetGame().GameName}.{ModuleName}.asmdef";
        public string EditorAssemblyDefinitionAssetPath => $"{ModuleDirectory}/Editor/{GameInspector.GetGame().GameName}.{ModuleName}.Editor.asmdef";
        public AssemblyDefinition AssemblyDefinition => AssemblyDefinition.FromPath(AssemblyDefinitionAssetPath);
        public AssemblyDefinition EditorAssemblyDefinition => AssemblyDefinition.FromPath(EditorAssemblyDefinitionAssetPath);
        
        public string ManagerClassPath => $"{ManagerDirectory}/{ModuleName}Manager.cs";
        public string ServicesClassPath => $"{ServiceDirectory}/{ModuleName}Service.cs";
        public string EventsClassPath => $"{EventsDirectory}/{ModuleName}Events.cs";
        

        public void SetModuleName(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            
            _moduleName = name;
            if (_moduleName.ToLower() == StringLibrary.MAIN_MODULE)
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