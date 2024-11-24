using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DandyDino.Elements;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class GameInspector
    {
        public static bool GameRootExists()
        {
            string assetGUID = AssetDatabase.FindAssets("t:Game").FirstOrDefault();
            string path = AssetDatabase.GUIDToAssetPath(assetGUID);
            Game asset = AssetDatabase.LoadAssetAtPath<Game>(path);

            return asset != null;
        }

        public static string GetGamePath()
        {
            return DDElements.Assets.GetAssetPath(GetExistingGameRoot());
        }
        
        public static string GetModulesPath()
        {
            string gamePath = Path.GetDirectoryName(GetGamePath());
            return $"{gamePath}/Modules";
        }
        
        public static Game GetExistingGameRoot()
        {
            string assetGUID = AssetDatabase.FindAssets("t:Game").FirstOrDefault();
            string path = AssetDatabase.GUIDToAssetPath(assetGUID);
            Game asset = AssetDatabase.LoadAssetAtPath<Game>(path);

            return asset;
        }
        
        public static Module GetMainModule()
        {
            Module mainModule = GetModules(true)?.Where(x => x.ModuleName == "Main").FirstOrDefault();
            return mainModule;
        }
        
        public static Module GetModule(string name)
        {
            return GetModules().Where(x => x.ModuleName.ToLower() == name.ToLower()).FirstOrDefault();
        }
        
        public static List<Module> GetModules(bool includeMain = false)
        {
            List<Module> modules = new List<Module>();
            
            string[] assetGUIDs = AssetDatabase.FindAssets("t:Module");
            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                Module asset = AssetDatabase.LoadAssetAtPath<Module>(path);

                if (asset == null)
                {
                    continue;
                }

                if (!includeMain && (asset.ModuleName == "MainModule" || asset.ModuleName == "Main"))
                {
                    continue;
                }

                modules.Add(asset);
            }

            return modules;
        }
        
        public static Module GetModuleInParentDirectories(string path)
        {
           return DDElements.Assets.GetFirstAssetInParentDirectories<Module>(path);
        }
        
        public static string GetModuleEditorScriptsPath(Module module)
        {
            string modulePath = DDElements.Assets.GetAssetPath(module);
            string moduleDirectory = Path.GetDirectoryName(modulePath);
            return $"{moduleDirectory}/Editor/Scripts";
        }

        public static string GetAsmdefPath(Module module)
        {
            string modulePath = DDElements.Assets.GetAssetDirectory(module);
            string[] asmdefGUIDs = AssetDatabase.FindAssets("t:asmdef", new []{modulePath});
            List<string> paths = new List<string>();

            for (int i = 0; i < asmdefGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(asmdefGUIDs[i]);
                paths.Add(path);
            }
            
            return paths.FirstOrDefault(x => !x.ToLower().Contains(".editor"));
        }
        
        public static string GetEditorAsmdefPath(Module module)
        {
            string modulePath = DDElements.Assets.GetAssetDirectory(module);
            string[] asmdefGUIDs = AssetDatabase.FindAssets("t:asmdef", new []{modulePath});
            List<string> paths = new List<string>();

            for (int i = 0; i < asmdefGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(asmdefGUIDs[i]);
                paths.Add(path);
            }
            
            return paths.FirstOrDefault(x => x.ToLower().Contains(".editor"));
        }
        
        public static Module GetCurrentModule()
        {
            string currentFolder = DDElements.Assets.GetActiveFolderPath();
            if (!DDElements.Assets.IsFolderValid(currentFolder))
            {
                return null;
            }

            Module module = DDElements.Assets.GetFirstAssetInParentDirectories<Module>(currentFolder);
            return module;
        }

        public static Game GetGame()
        {
            return GetExistingGameRoot();
        }
        
        public static ProjectMap GetProjectMap()
        {
            ProjectMap projectMap = new ProjectMap();
            
            projectMap.game = GetExistingGameRoot();
            projectMap.modules = GetModules();
            projectMap.mainModule = projectMap.modules.Where(x => x.ModuleName == "Main").FirstOrDefault();

            return projectMap;
        }
        
        public static List<(string path, AssemblyDefinition asmdef)> GetGameAssemblyDefinitions()
        {
            Game game = GetGame();
            List<string> asmDefFiles = DDElements.Assets.GetAssetPathsInDirectory(game.GameDirectory, "asmdef");
            List<(string path, AssemblyDefinition asmdef)> result = new List<(string path, AssemblyDefinition asmdef)>();

            for (int i = 0; i < asmDefFiles.Count; i++)
            {
                string jsonContent = File.ReadAllText(asmDefFiles[i]);
                result.Add((asmDefFiles[i], AssemblyDefinition.FromJson(jsonContent)));
            }

            return result;
        }
        
        public static List<string> GetAvailableAssemblyDefinitionNames(string[] excludedPaths = null, bool includePackages = true)
        {
            string[] asmdefGuids = AssetDatabase.FindAssets("t:asmdef");
            List<string> assemblyDefinitionNames = new List<string>();
            
            foreach (string guid in asmdefGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                bool excludePath = (excludedPaths != null && excludedPaths.Contains(path)) || ((path.StartsWith("Packages/") || path.StartsWith("Packages\\")) && !includePackages);
                if (excludePath)
                {
                    continue;
                }
                
                string jsonContent = File.ReadAllText(path);
                
                AssemblyDefinition asmdefData = JsonConvert.DeserializeObject<AssemblyDefinition>(jsonContent);
                
                if (!string.IsNullOrEmpty(asmdefData.name))
                {
                    assemblyDefinitionNames.Add(asmdefData.name);
                }
            }

            return assemblyDefinitionNames;
        }
        
        public static List<(string path, AssemblyDefinition asmdef)> GetAllAssemblyDefinitionsInProject()
        {
            string[] asmdefGuids = AssetDatabase.FindAssets("t:asmdef");
            List<(string path, AssemblyDefinition asmdef)> assemblyDefinitions = new List<(string path, AssemblyDefinition asmdef)>();
            
            foreach (string guid in asmdefGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string jsonContent = File.ReadAllText(path);
                
                AssemblyDefinition asmdefData = JsonConvert.DeserializeObject<AssemblyDefinition>(jsonContent);
                
                if (asmdefData != null)
                {
                    assemblyDefinitions.Add((path, asmdefData));
                }
            }

            return assemblyDefinitions;
        }
        
        public static string GetCurrentAssemblyName()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            return currentAssembly.GetName().Name;
        }

        public static AssemblyDefinition GetCurrentAssemblyDefinition(string folderPath)
        {
            string currentPath = folderPath;
            List<string> asmdefFiles = new List<string>();
            while (!string.IsNullOrEmpty(currentPath))
            {
                string[] assets = AssetDatabase.FindAssets("t:asmdef" , new[] { currentPath });
                
                foreach (string guid in assets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    asmdefFiles.Add(assetPath);
                }
                
                int lastSlashIndex = currentPath.LastIndexOf('/');
                if (lastSlashIndex == -1) break;
                currentPath = currentPath.Substring(0, lastSlashIndex);
            }
            
            if (asmdefFiles.Count == 0)
            {
                Debug.LogWarning($"No .asmdef file found in folder: {folderPath}");
                return null;
            }
            
            string asmdefFilePath = asmdefFiles[0];
            string jsonContent = File.ReadAllText(asmdefFilePath);
            
            AssemblyDefinition asmdefData = JsonConvert.DeserializeObject<AssemblyDefinition>(jsonContent);
            return asmdefData;
        }

        public static ServicesCollection GetServicesCollection()
        {
            string assetGUID = AssetDatabase.FindAssets("t:ServicesCollection").FirstOrDefault();
            string path = AssetDatabase.GUIDToAssetPath(assetGUID);
            ServicesCollection asset = AssetDatabase.LoadAssetAtPath<ServicesCollection>(path);
            return asset;
        }
    }
}