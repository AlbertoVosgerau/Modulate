using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace DandyDino.Modulate
{
    public class AssemblyDefinitionPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,  string[] movedFromAssetPaths)
        {
            List<Module> modules = GameInspector.GetModules(true);
            List<(string path, AssemblyDefinition asmdef)> assembliesInProject = new List<(string path, AssemblyDefinition asmdef)>();
            bool asmdefFound = false;
            
            for (int i = 0; i < deletedAssets.Length; i++)
            {
                string deletedAsset = deletedAssets[i];
            
                if (!deletedAsset.EndsWith(".asmdef"))
                {
                    continue;
                }

                if (!asmdefFound)
                {
                    foreach (Module module in modules)
                    {
                        if (module == null || !File.Exists(module.ModulePath))
                        {
                            continue;
                        }

                        List<(string path, AssemblyDefinition asmdef)> ModuleAsmDef = module.GetAssemblyDefinitions();
                        if (ModuleAsmDef == null || ModuleAsmDef.Count == 0)
                        {
                            continue;
                        }
                        
                        assembliesInProject.AddRange(ModuleAsmDef);
                    }
                    asmdefFound = true;
                }
            
                string fileName = Path.GetFileNameWithoutExtension(deletedAsset);
            
                for (int j = 0; j < assembliesInProject.Count; j++)
                {
                    if (assembliesInProject[j].path == deletedAsset)
                    {
                        continue;
                    }
                    AssemblyDefinition projectAsmDef = assembliesInProject[j].asmdef;
            
                    List<string> references = new List<string>();
                    references.AddRange(projectAsmDef.references.Where(x => x != null && x != fileName));
            
                    if (projectAsmDef.references.Length == references.Count)
                    {
                        continue;
                    }
                    
                    projectAsmDef.references = references.ToArray();
                    projectAsmDef.SaveToPath(assembliesInProject[j].path);
                }
            }
        }
    }
}
