using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class AssemblyDefinition
    {
        public string name { get; set; }
        public string rootNamespace { get; set; }
        public string[] references { get; set; }
        public string[] includePlatforms { get; set; }
        public string[] excludePlatforms { get; set; }
        public string[] defineConstraints { get; set; }
        public bool allowUnsafeCode { get; set; }

        public static AssemblyDefinition FromPath(string asmdefPath)
        {
            string asmdefText = File.ReadAllText(asmdefPath);
            return JsonConvert.DeserializeObject<AssemblyDefinition>(asmdefText);
        }

        public void SaveToPath(string path)
        {
            string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, jsonString, Encoding.UTF8);
        }

        public static AssemblyDefinition FromJson(string jsonContent)
        {
            return JsonConvert.DeserializeObject<AssemblyDefinition>(jsonContent);
        }
        
        public void AddDependency(string dependency)
        {
            List<string> referencesList = references.ToList();
            if (!referencesList.Contains(dependency))
            {
                referencesList.Add(dependency);
            }
            
            references = referencesList.ToArray();
        }
        
        public void RemoveDependency(string dependency, string path)
        {
            List<string> referencesList = references.ToList();
            if (referencesList.Contains(dependency))
            {
                referencesList.Remove(dependency);
            }
            
            references = referencesList.ToArray();
        }
        
        public void Rename(string oldFilePath, string newName)
        {
            string newFileName = $"{newName}.asmdef";
            if (!File.Exists(oldFilePath))
            {
                Debug.LogError($"Assembly definition file not found at: {oldFilePath}");
                return;
            }

            string oldAssetPath = oldFilePath.Replace(Application.dataPath, "Assets");
            string oldGuid = AssetDatabase.AssetPathToGUID(oldAssetPath);

            if (string.IsNullOrEmpty(oldGuid))
            {
                Debug.LogError("Failed to retrieve GUID for the old assembly definition.");
                return;
            }

            string directory = Path.GetDirectoryName(oldFilePath);
            string newFilePath = Path.Combine(directory, newFileName);
            string newAssetPath = newFilePath.Replace(Application.dataPath, "Assets");

            if (Path.GetFileName(oldAssetPath) != newFileName)
            {
                AssetDatabase.RenameAsset(oldAssetPath, newName);
                AssetDatabase.Refresh();
            }

            string newGuid = AssetDatabase.AssetPathToGUID(newAssetPath);
            if (string.IsNullOrEmpty(newGuid))
            {
                Debug.LogError("Failed to retrieve GUID for the new assembly definition.");
                return;
            }

            SaveToPath(newFilePath);
            UpdateProjectReferences(oldGuid, newGuid);
            AssetDatabase.Refresh();
        }
        
        private void UpdateProjectReferences(string oldGuid, string newGuid)
        {
            string[] allAssemblyDefinitionPaths = Directory.GetFiles(Application.dataPath, "*.asmdef", SearchOption.AllDirectories);

            foreach (string asmdefPath in allAssemblyDefinitionPaths)
            {
                string jsonContent = File.ReadAllText(asmdefPath);
                if (jsonContent.Contains(oldGuid))
                {
                    jsonContent = jsonContent.Replace(oldGuid, newGuid);
                    File.WriteAllText(asmdefPath, jsonContent);
                }
            }
        }
    }
}