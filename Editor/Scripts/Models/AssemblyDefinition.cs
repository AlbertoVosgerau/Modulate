using System.IO;
using System.Text;
using Newtonsoft.Json;
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

        public static string ToJson(AssemblyDefinition asmDef, string path)
        {
            string jsonString = JsonConvert.SerializeObject(asmDef, Formatting.Indented);
            File.WriteAllText(path, jsonString, Encoding.UTF8);
            Debug.Log($"Saving json to {path}");
            return jsonString;
        }
        
        public static AssemblyDefinition FromJson(string jsonContent)
        {
            return JsonConvert.DeserializeObject<AssemblyDefinition>(jsonContent);
        }
    }
}