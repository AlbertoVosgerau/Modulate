using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ModuleCreator
    {
        public static string CreateModule(string path, string moduleName, List<string> assembliesToAdd)
        {
            Game game = GameInspector.GetGame();
            string moduleRoot = Path.Combine(path, moduleName);
            string editorFolder = Path.Combine(moduleRoot, "Editor");
            string editorScriptsFolder = Path.Combine(editorFolder, "Scripts");
            string scriptsFolder = Path.Combine(moduleRoot, "Scripts");
            string serviceFolder = Path.Combine(scriptsFolder, "Service");
            string managerFolder = Path.Combine(scriptsFolder, "Manager");
            string monoBehavioursFolder = Path.Combine(scriptsFolder, "MonoBehaviours");
            string eventsFolder = Path.Combine(scriptsFolder, "Events");

            AssetDatabase.CreateFolder(path, moduleName);
            AssetDatabase.CreateFolder(moduleRoot, "Scripts");
            AssetDatabase.CreateFolder(scriptsFolder, "Manager");
            AssetDatabase.CreateFolder(scriptsFolder, "Service");
            AssetDatabase.CreateFolder(scriptsFolder, "MonoBehaviours");
            AssetDatabase.CreateFolder(scriptsFolder, "Events");
            AssetDatabase.CreateFolder(moduleRoot, "Editor");
            AssetDatabase.CreateFolder(editorFolder, "Scripts");

            string assemblyDefinitionName = $"{game.GameName}.{moduleName}";
            
            List<string> assemblies = new List<string>();
            assemblies.Add(StringLibrary.ASSEMBLY_DEFINITION);
            assemblies.AddRange(assembliesToAdd);
                
            AssetCreationUtils.CreateAssemblyDefinition(moduleRoot, assemblyDefinitionName, assemblyDefinitionName, references: assemblies.ToArray());
            AssetCreationUtils.CreateAssemblyDefinition(editorFolder, $"{assemblyDefinitionName}.Editor", assemblyDefinitionName, references: new []{StringLibrary.ASSEMBLY_DEFINITION_EDITOR, assemblyDefinitionName, StringLibrary.ELEMENTS_ASSEMBLY_DEFINITION}, includePlatforms: new []{"Editor"});
                 
            
            AssetCreationUtils.CreateRootFile<Module>($"{moduleRoot}/{moduleName}.asset", moduleName, root =>
            {
                root.SetModuleName(moduleName);
            });

            string nameSpace = $"{GameInspector.GetGame().GameName}.{moduleName}";

            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName
            }.GenerateClass(TemplateType.Service, serviceFolder, $"{moduleName}Service", false);
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName,
                type = moduleName
            }.GenerateClass(TemplateType.Manager, managerFolder, $"{moduleName}Manager", false);
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName,
                type = moduleName
            }.GenerateClass(TemplateType.ManagerPropertyDrawer, editorScriptsFolder, $"{moduleName}ManagerPropertyDrawer", false);
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName,
                type = moduleName
            }.GenerateClass(TemplateType.ServicePropertyDrawer, editorScriptsFolder, $"{moduleName}ServicePropertyDrawer", false);
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName
            }.GenerateClass(TemplateType.Events, eventsFolder, $"{moduleName}Events", false);

            if (moduleName != "Main")
            {
                Module mainModule = GameInspector.GetMainModule();
                mainModule.AssemblyDefinition.AddDependency(assemblyDefinitionName);
            }
            
            AssetDatabase.Refresh();
            return moduleRoot;
        }
        
        private static string GetTemplatesFolder()
        {
            string coreRootGUID = AssetDatabase.FindAssets("t:CoreRoot").FirstOrDefault();
            string coreRootPath = AssetDatabase.GUIDToAssetPath(coreRootGUID);
            string coreFolder = Path.GetDirectoryName(coreRootPath);
            string templatesPath = Path.Combine(coreFolder, "Editor/Templates");
            Debug.Log(templatesPath);
            return templatesPath;
        }
        
        private static string GetServiceTemplate()
        {
            return File.ReadAllText(GetServiceTemplatePath());
        }
        private static string GetServicePropertyDrawerTemplate()
        {
            return File.ReadAllText(GetServicePropertyDrawerTemplatePath());
        }
        private static string GetManagerTemplate()
        {
            return File.ReadAllText(GetManagerTemplatePath());
        }
        
        private static string GetEventsTemplate()
        {
            return File.ReadAllText(GetEventsTemplatePath());
        }
        private static string GetManagerPropertyDrawerTemplate()
        {
            return File.ReadAllText(GetManagerPropertyDrawerTemplatePath());
        }

        private static string GetServiceTemplatePath()
        {
            string templatesFolder = GetTemplatesFolder();
            return Path.Combine(templatesFolder, "ServiceTemplate.txt");
        }
        private static string GetServicePropertyDrawerTemplatePath()
        {
            string templatesFolder = GetTemplatesFolder();
            return Path.Combine(templatesFolder, "ServicePropertyDrawerTemplate.txt");
        }
        private static string GetManagerTemplatePath()
        {
            string templatesFolder = GetTemplatesFolder();
            return Path.Combine(templatesFolder, "ManagerTemplate.txt");
        }
        
        private static string GetEventsTemplatePath()
        {
            string templatesFolder = GetTemplatesFolder();
            return Path.Combine(templatesFolder, "EventsTemplate.txt");
        }
        
        private static string GetManagerPropertyDrawerTemplatePath()
        {
            string templatesFolder = GetTemplatesFolder();
            return Path.Combine(templatesFolder, "ManagerPropertyDrawerTemplate.txt");
        }
    }
}