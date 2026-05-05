using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ModuleCreator
    {
        public static string CreateModule(string path, string moduleName, List<string> assembliesToAdd, bool createResources = false, bool isSingleton = true)
        {
            Game game = GameInspector.GetGame();
            string moduleRoot = Path.Combine(path, moduleName);
            string editorFolder = Path.Combine(moduleRoot, StringLibrary.EDITOR);
            string editorScriptsFolder = Path.Combine(editorFolder, StringLibrary.SCRIPTS);
            string scriptsFolder = Path.Combine(moduleRoot, StringLibrary.SCRIPTS);
            string managerFolder = Path.Combine(scriptsFolder, StringLibrary.MANAGER);
            string viewsFolder = Path.Combine(scriptsFolder, StringLibrary.VIEWS);
            string monoBehavioursFolder = Path.Combine(scriptsFolder, StringLibrary.MONOBEHAVIOURS);
            string eventsFolder = Path.Combine(scriptsFolder, StringLibrary.EVENTS);
            string testsFolder = Path.Combine(scriptsFolder, StringLibrary.TESTS);

            AssetDatabase.CreateFolder(path, moduleName);
            AssetDatabase.CreateFolder(moduleRoot, StringLibrary.SCRIPTS);
            AssetDatabase.CreateFolder(scriptsFolder, StringLibrary.MANAGER);
            AssetDatabase.CreateFolder(scriptsFolder, StringLibrary.VIEWS);
            AssetDatabase.CreateFolder(scriptsFolder, StringLibrary.MONOBEHAVIOURS);
            AssetDatabase.CreateFolder(scriptsFolder, StringLibrary.EVENTS);
            AssetDatabase.CreateFolder(scriptsFolder, StringLibrary.SCENE_SCOPES);
            AssetDatabase.CreateFolder(moduleRoot, StringLibrary.EDITOR);
            AssetDatabase.CreateFolder(editorFolder, StringLibrary.SCRIPTS);
            
            AssetDatabase.CreateFolder(scriptsFolder, StringLibrary.TESTS);
            AssetDatabase.CreateFolder(testsFolder, StringLibrary.EDITOR);
            AssetDatabase.CreateFolder(testsFolder, StringLibrary.RUNTIME);

            if (createResources)
            {
                AssetDatabase.CreateFolder(moduleRoot, StringLibrary.RESOURCES);
            }

            string assemblyDefinitionName = $"{game.GameName}.{moduleName}";

            List<string> assemblies = new List<string>();
            assemblies.Add(StringLibrary.ASSEMBLY_DEFINITION);
            assemblies.AddRange(assembliesToAdd);
            
            if (moduleName != StringLibrary.COMMONS_MODULE)
            {
                string commonsModuleName = string.Empty;
                if (moduleName == StringLibrary.MAIN_MODULE)
                {
                    commonsModuleName = $"{game.GameName}.{StringLibrary.COMMONS_MODULE}";
                    
                }
                else
                {
                    Module commonsModule = GameInspector.GetCommonsModule();
                    AssemblyDefinition commonsAsmdef = commonsModule.AssemblyDefinition;
                    commonsModuleName = commonsAsmdef.name;
                }
                assemblies.Add(commonsModuleName);
            }
            assemblies.Add(StringLibrary.REFLEX);

            AssetCreationUtils.CreateAssemblyDefinition(moduleRoot, assemblyDefinitionName, assemblyDefinitionName, references: assemblies.ToArray());
            AssetCreationUtils.CreateAssemblyDefinition(editorFolder, $"{assemblyDefinitionName}.{StringLibrary.EDITOR}", assemblyDefinitionName,
                references: new []{StringLibrary.ASSEMBLY_DEFINITION_EDITOR, assemblyDefinitionName, StringLibrary.ELEMENTS_ASSEMBLY_DEFINITION}, includePlatforms: new []{"Editor"});
            
           assemblies.Add( StringLibrary.UNITY_EDITOR_TEST_RUNNER);
           assemblies.Add(StringLibrary.UNITY_ENGINE_TEST_RUNNER);
                
            AssetCreationUtils.CreateAssemblyDefinition(Path.Combine(testsFolder, StringLibrary.RUNTIME), 
                $"{assemblyDefinitionName}.{StringLibrary.TESTS}.{StringLibrary.RUNTIME}", 
                assemblyDefinitionName, references: assemblies.ToArray(),
                             defineConstraints: new []{StringLibrary.UNITY_INCLUDE_TESTS},
                             precompiledReferences: new []{StringLibrary.NUNIT_DLL}, overrideReferences: true, autoReferenced: true);
            
            AssetCreationUtils.CreateAssemblyDefinition(Path.Combine(testsFolder, StringLibrary.EDITOR), 
                $"{assemblyDefinitionName}.{StringLibrary.TESTS}.{StringLibrary.EDITOR}", 
                assemblyDefinitionName,
                             references: new []{StringLibrary.ASSEMBLY_DEFINITION_EDITOR, assemblyDefinitionName, StringLibrary.ELEMENTS_ASSEMBLY_DEFINITION, StringLibrary.UNITY_EDITOR_TEST_RUNNER, StringLibrary.UNITY_ENGINE_TEST_RUNNER},
                             includePlatforms: new []{"Editor"},
                             defineConstraints: new []{StringLibrary.UNITY_INCLUDE_TESTS},
                             precompiledReferences: new []{StringLibrary.NUNIT_DLL}, overrideReferences: true, autoReferenced: true);
            
            
            AssetCreationUtils.CreateRootFile<Module>($"{moduleRoot}/{moduleName}.asset", moduleName, root =>
            {
                root.SetModuleName(moduleName);
            });

            string nameSpace = $"{GameInspector.GetGame().GameName}.{moduleName}";
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName,
                type = moduleName
            }.GenerateClass(TemplateType.Manager, managerFolder, $"{moduleName}{StringLibrary.MANAGER}", false, isSingleton);
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName,
                type = moduleName
            }.GenerateClass(TemplateType.View, viewsFolder, $"{moduleName}{StringLibrary.VIEW}", false);
            
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName
            }.GenerateClass(TemplateType.Events, eventsFolder, $"{moduleName}{StringLibrary.EVENTS}", false);
            
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName
            }.GenerateClass(TemplateType.EditorTest, Path.Combine(testsFolder, StringLibrary.EDITOR), $"{moduleName}{StringLibrary.EDITOR}Tests", false);
            
            
            new ClassGenerator()
            {
                newNamespace = nameSpace,
                name = moduleName
            }.GenerateClass(TemplateType.RuntimeTest, Path.Combine(testsFolder, StringLibrary.RUNTIME), $"{moduleName}{StringLibrary.RUNTIME}Tests", false);
            

            if (moduleName != StringLibrary.MAIN_MODULE)
            {
                Module mainModule = GameInspector.GetMainModule();
                AssemblyDefinition mainAsmdef = mainModule.AssemblyDefinition;
                mainAsmdef.AddDependency(assemblyDefinitionName);
                mainAsmdef.SaveToPath( mainModule.AssemblyDefinitionAssetPath);
            }
            
            
            AssetDatabase.Refresh();
            return moduleRoot;
        }

        public static void CreateScene(string sceneFolder, string sceneName)
        {
            string scenePath = $"{sceneFolder}/{sceneName}.unity";

            UnityEngine.SceneManagement.Scene newScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects,
                UnityEditor.SceneManagement.NewSceneMode.Single);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(newScene, scenePath);
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
        
        private static string GetViewTemplate()
        {
            return File.ReadAllText(GetViewTemplatePath());
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
        
        private static string GetViewTemplatePath()
        {
            string templatesFolder = GetTemplatesFolder();
            return Path.Combine(templatesFolder, "ViewTemplate.txt");
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