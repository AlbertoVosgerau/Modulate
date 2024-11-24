using System.Collections.Generic;
using System.IO;
using System.Linq;
using DandyDino.Elements;
using UnityEditor;

namespace DandyDino.Modulate
{
    public class ClassGenerator
    {
        private const string NAMESPACE = "#Namespace#";
        public string newNamespace = "";
        
        private const string TYPE = "#Type#";
        public string type = "";
        
        private const string NAME = "#Name#";
        public string name = "";
        
        private const string FILE_NAME = "#FileName#";
        public string fileName = "";
        
        private const string MENU_NAME = "#MenuName#";
        public string menuName = "";
        
        private const string ADDITIONAL_USING = "#AdditionalUsing#";
        public string additionalUsing = "";

        public void GenerateClass(TemplateType templateType, string directory, string className, bool pingAsset)
        {
            ModulateRoot modulateRoot = ModulateRoot.GetCoreRoot();
            List<string> templates = modulateRoot.GetAllTemplates();
            List<string> names = new List<string>();

            for (int i = 0; i < templates.Count; i++)
            {
                names.Add(Path.GetFileNameWithoutExtension(templates[i].Replace("Template", "")));
            }

            string template = "";

            switch (templateType)
            {
                case TemplateType.CustomInspector:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.CustomInspector.ToString()))];
                    break;
                case TemplateType.Enum:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.Enum.ToString()))];
                    break;
                case TemplateType.Events:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.Events.ToString()))];
                    break;
                case TemplateType.Interface:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.Interface.ToString()))];
                    break;
                case TemplateType.ManagerPropertyDrawer:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.ManagerPropertyDrawer.ToString()))];
                    break;
                case TemplateType.Manager:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.Manager.ToString()))];
                    break;
                case TemplateType.MonoBehaviour:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.MonoBehaviour.ToString()))];
                    break;
                case TemplateType.PropertyDrawer:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.PropertyDrawer.ToString()))];
                    break;
                case TemplateType.ServicePropertyDrawer:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.ServicePropertyDrawer.ToString()))];
                    break;
                case TemplateType.Service:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.Service.ToString()))];
                    break;
                case TemplateType.EmptyClass:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.EmptyClass.ToString()))];
                    break;
                case TemplateType.ScriptableObject:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.ScriptableObject.ToString()))];
                    break;
                case TemplateType.EditorWindow:
                    template = templates[names.IndexOf(names.First(x => x == TemplateType.EditorWindow.ToString()))];
                    break;
            }
            
            InternalGenerateClass(File.ReadAllText(template), directory, className, pingAsset);
        }

        private void InternalGenerateClass(string template, string directory, string className, bool pingAsset)
        {
            AssemblyDefinition asmdef = GameInspector.GetCurrentAssemblyDefinition(directory);
            if (asmdef != null)
            {
                newNamespace = asmdef.rootNamespace;
            }

            if (string.IsNullOrWhiteSpace(newNamespace))
            {
                newNamespace = "UnityProject";
            }
            
            template = template.Replace(NAMESPACE, newNamespace);
            template = template.Replace(TYPE, type);
            template = template.Replace(FILE_NAME, fileName);
            template = template.Replace(MENU_NAME, menuName);
            template = template.Replace(NAME, name);
            template = template.Replace(ADDITIONAL_USING, additionalUsing);
            
            string[] existingFiles = AssetDatabase.FindAssets("t:Script" , new[] { directory });
            List<string> sameName = new List<string>();
                
            foreach (string guid in existingFiles)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(assetPath) == className)
                {
                    sameName.Add(assetPath);
                }
            }

            if (sameName.Count > 0)
            {
                className = $"{className}{sameName.Count.ToString("00")}";
            }
                
            string path = Path.Combine(directory, $"{className}.cs");
            File.WriteAllText(path, template);
            AssetDatabase.Refresh();
            if (pingAsset)
            {
                DDElements.Assets.Ping(path);
            }
        }
    }
}
