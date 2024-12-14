using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DandyDino.Modulate;
using Newtonsoft.Json;
using UnityEditor;

namespace DandyDino.Modulate
{
    public class GameProjectUtils
    {
        public static void RenameInWholeProject(string oldStr, string newStr)
        {
            Game game = GameInspector.GetGame();
            List<string> names = GameInspector.GetAssemblyDefinitionNames();
            List<(string path, AssemblyDefinition asmdef)> assemblies = GameInspector.GetGameAssemblyDefinitions().Where(x => names.Contains(x.asmdef.name)).ToList();

            oldStr = oldStr.Trim();
            oldStr = oldStr.Replace(" ", "");
            
            List<string> classes = GameInspector.GetAllClassesPaths();
            for (int i = 0; i < classes.Count; i++)
            {
                string classPath = classes[i].Replace("\\", "/");
                string classDirectory = Path.GetDirectoryName(classPath);
                string classFileName = Path.GetFileNameWithoutExtension(classPath);
                string classContent = File.ReadAllText(classes[i]);
                string pattern = $@"\b({string.Join("|", names)})\b";
                
                
                string newContent = Regex.Replace(classContent, pattern, match =>
                {
                    string matchedValue = match.Value;
                    return matchedValue.Replace(oldStr, newStr);
                });

                if (classContent == newContent)
                {
                    AssetDatabase.RenameAsset(classPath, classFileName.Replace(oldStr, newStr));
                    continue;
                }
                
                File.WriteAllText(classes[i], newContent);
                AssetDatabase.RenameAsset(classPath, classFileName.Replace(oldStr, newStr));
            }
            AssetDatabase.Refresh();

            for (int i = 0; i < assemblies.Count; i++)
            {
                (string path, AssemblyDefinition asmdef) assembly = assemblies[i];

                string originalContent = File.ReadAllText(assembly.path);
                
                assembly.asmdef.rootNamespace = assembly.asmdef.rootNamespace.Replace(oldStr, newStr);
                string newName = assembly.asmdef.name.Replace(oldStr, newStr);
                assembly.asmdef.name = newName;
                for (int j = 0; j < assembly.asmdef.references.Length; j++)
                {
                    string refStr = assembly.asmdef.references[j];
                    
                    string pattern = $@"\b({string.Join("|", names)})\b";
                
                    string newRef = Regex.Replace(refStr, pattern, match =>
                    {
                        string matchedValue = match.Value;
                        return matchedValue.Replace(oldStr, newStr);
                    });

                    assembly.asmdef.references[j] = newRef;
                }
                
                string jsonString = JsonConvert.SerializeObject(assembly.asmdef, Formatting.Indented);
                if (originalContent == jsonString)
                {
                    continue;
                }
                assembly.asmdef.Rename(assembly.path, newName);
            }
        }
    }
}
