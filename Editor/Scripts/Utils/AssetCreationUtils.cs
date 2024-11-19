using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class AssetCreationUtils
    {
        public static bool IsValidFolderName(string folderName)
        {
            string invalidCharsPattern = @"[\\\/:*?""<>|]";
            
            if (Regex.IsMatch(folderName, invalidCharsPattern) || 
                folderName.EndsWith(".") || folderName.EndsWith(" "))
            {
                return false;
            }
            
            return !string.IsNullOrEmpty(folderName);
        }
    
        public static void CreateAssemblyDefinition(string folderPath, string assemblyName, string rootNamespace, string[] references = null, string[] includePlatforms = null, string[] excludePlatforms = null,  string[] defineConstraints = null, bool allowUnsafeCode = false)
        {
            AssemblyDefinition asmDef = new AssemblyDefinition
            {
                name = assemblyName,
                rootNamespace = rootNamespace,
                references = references ?? new string[] { }, 
                includePlatforms = includePlatforms ?? new string[] { },
                excludePlatforms = excludePlatforms ?? new string[] { },
                defineConstraints = defineConstraints ?? new string[] { },
                allowUnsafeCode = allowUnsafeCode
            };
            
            string jsonString = JsonConvert.SerializeObject(asmDef, Formatting.Indented);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            
            string filePath = Path.Combine(folderPath, $"{assemblyName}.asmdef");
            
            File.WriteAllText(filePath, jsonString, Encoding.UTF8);
        }

        public static void AddDependencyToAssemblyDefinition(string newDependency, string asmdefPath)
        {
            string asmdefText = File.ReadAllText(asmdefPath);
            AssemblyDefinition asmDef = JsonConvert.DeserializeObject<AssemblyDefinition>(asmdefText);

            string[] newRefArray = new string [asmDef.references.Length + 1];

            for (int i = 0; i < asmDef.references.Length; i++)
            {
                newRefArray[i] = asmDef.references[i];
            }

            newRefArray[^1] = newDependency;
            asmDef.references = newRefArray;
            
            string jsonString = JsonConvert.SerializeObject(asmDef, Formatting.Indented);
            File.WriteAllText(asmdefPath, jsonString, Encoding.UTF8);
        }
        
        public static void CreateRootFile<T>(string path, string name, Action<T> onCreate) where T : ScriptableObject
        {
            if (AssetDatabase.LoadAssetAtPath<Module>(path) != null)
            {
                return;
            }
            T rootFile = ScriptableObject.CreateInstance<T>();
            onCreate?.Invoke(rootFile);
            
            AssetDatabase.CreateAsset(rootFile, path);
            AssetDatabase.SaveAssets();
        }

        public static ServicesCollection CreateServicesCollection(string path)
        {
            ServicesCollection servicesCollection = ScriptableObject.CreateInstance<ServicesCollection>();
            
            AssetDatabase.CreateAsset(servicesCollection, path);
            AssetDatabase.SaveAssets();
            return servicesCollection;
        }
        
        
        public static Game CreateGameRoot(string path)
        {
            if (GameInspector.GameRootExists())
            {
                Debug.LogWarning($"Game Root already exists.");
                return GameInspector.GetExistingGameRoot();
            }
            
            Game game = ScriptableObject.CreateInstance<Game>();
            
            AssetDatabase.CreateAsset(game, path);
            AssetDatabase.SaveAssets();
            return game;
        }
    }
}