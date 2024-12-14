using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DandyDino.Modulate
{
    public class Assets
    {
        
        public void LoadScriptByName(string typeName, out object instance)
        {
            instance = null;
            
            Type type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);

            if (type == null)
            {
                Debug.LogError($"Type '{typeName}' not found.");
                return;
            }

            try
            {
                instance = Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to instantiate type '{typeName}': {ex.Message}");
                instance = null;
            }
        }
        
        
        public T CreateScriptableObjectAsset<T>(string folder, string fileName) where T : ScriptableObject
        {
            T newInstance = ScriptableObject.CreateInstance<T>();
            string path = folder + $"{folder}/{fileName}.asset";
     
            AssetDatabase.CreateAsset(newInstance, path);
            AssetDatabase.Refresh();
            return Resources.Load<T>(path);
        }
        
        public void CreateClass(string directory, string fileName, string classContent)
        {
            fileName = $"{fileName}.cs";
            string path = Path.Combine(directory, fileName);

            if (AssetDatabase.AssetPathExists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
            
            File.WriteAllText(path, classContent);
            AssetDatabase.Refresh();
        }
        
        public void Ping(Object obj)
        {
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        public void Ping(string path)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset == null)
            {
                return;
            }
            
            Ping(asset);
        }

        public void SetDirty(Object obj)
        {
            EditorUtility.SetDirty(obj);
        }
        
        public string GetActiveFolderViaReflection()
        {
            Type projectWindowUtilType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectWindowUtil");
            if (projectWindowUtilType == null)
            {
                Debug.LogError("ProjectWindowUtil type not found.");
                return null;
            }
            
            MethodInfo tryGetActiveFolderPathMethod = projectWindowUtilType.GetMethod(
                "TryGetActiveFolderPath", 
                BindingFlags.NonPublic | BindingFlags.Static
            );

            if (tryGetActiveFolderPathMethod == null)
            {
                Debug.LogError("TryGetActiveFolderPath method not found.");
                return null;
            }
            
            object[] parameters = new object[] { null };
            
            bool success = (bool)tryGetActiveFolderPathMethod.Invoke(null, parameters);
            
            if (success)
            {
                return (string)parameters[0];
            }
            else
            {
                Debug.Log("Failed to get the active folder path.");
                return null;
            }
        }
        
        public string GetActiveFolderPath()
        {
            Object[] selectedObjects = Selection.objects;
            
            if (selectedObjects.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(selectedObjects[0]);
                
                if (AssetDatabase.IsValidFolder(path))
                {
                    return path;
                }
                
                path = System.IO.Path.GetDirectoryName(path);
                return path;
            }
            return null;
        }
        
        public string GetScriptPathFromType(Type type)
        {
            string[] guids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (monoScript != null && monoScript.GetClass() == type)
                {
                    return path;
                }
            }

            return null;
        }
        
        public void PingFolder(string folderPath)
        {
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(folderPath));
        }
        
        public void PingInsideFolder(string folderPath)
        {
            UnityEngine.Object folderObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(folderPath);
            if (folderObject == null)
            {
                Debug.LogError("Folder not found at path: " + folderPath);
                return;
            }

            // Access the internal ProjectBrowser class via reflection
            Type projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            if (projectBrowserType == null)
            {
                Debug.LogError("ProjectBrowser type not found.");
                return;
            }
            
            MethodInfo showFolderContents = projectBrowserType.GetMethod("ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance);
            if (showFolderContents == null)
            {
                Debug.LogError("ShowFolderContents method not found.");
                return;
            }
            
            EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
            if (projectBrowser == null)
            {
                Debug.LogError("Could not get a ProjectBrowser instance.");
                return;
            }
            
            int folderInstanceID = folderObject.GetInstanceID();
            if (projectBrowser == null || folderInstanceID < 0)
            {
                return;
            }
            showFolderContents.Invoke(projectBrowser, new object[] { folderInstanceID, true });
        }
        
        public bool IsSelectedMonoBehaviour()
        {
            Object selectedObject = Selection.activeObject;
            
            if (selectedObject is MonoScript script)
            {
                var type = script.GetClass();
                if (type != null && type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool IsSelectedScriptableObject()
        {
            Object selectedObject = Selection.activeObject;
            
            if (selectedObject is MonoScript script)
            {
                var type = script.GetClass();
                if (type != null && type.IsSubclassOf(typeof(ScriptableObject)))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void SetDirty(Scene scene)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }

        public void SetDirty(Object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                SetDirty(objects[i]);
            }
        }

        public void SetDirtyAndSave(Object obj)
        {
            SetDirty(obj);
            AssetDatabase.SaveAssetIfDirty(obj);
        }
        
        public string GetClassFilePath<T>()
        {
            MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)System.Activator.CreateInstance(typeof(T)));
            if (script == null)
            {
                script = MonoScript.FromScriptableObject((ScriptableObject)System.Activator.CreateInstance(typeof(T)));
            }

            if (script != null)
            {
                return AssetDatabase.GetAssetPath(script);
            }

            return null;
        }
        
        public void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void RefreshAssets()
        {
            AssetDatabase.Refresh();
        }
        
        public void SetDirtyAndSave(Scene scene)
        {
            SetDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.Refresh();
        }
        
        public void SetDirtyAndSave(Object[] objects)
        {
            SetDirty(objects);
            for (int i = 0; i < objects.Length; i++)
            {
                AssetDatabase.SaveAssetIfDirty(objects[i]);
            }
            AssetDatabase.Refresh();
        }

        public bool OpenPrefabScene(GameObject prefab)
        {
            if (prefab != null)
            {
                return AssetDatabase.OpenAsset(prefab);
            }

            return false;
        }

        public void ClosePrefabScene()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                StageUtility.GoToMainStage();
            }
        }
        
        public bool ModifyPrefab(GameObject prefab, Action<GameObject> modifyAction)
        {
            if (!IsPrefab(prefab))
            {
                return false;
            }
            
            string prefabPath = GetAssetPath(prefab);
            if (prefab != null)
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                if (instance != null)
                {
                    modifyAction?.Invoke(instance);
                    
                    PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                    Object.DestroyImmediate(instance);
                    return true;
                }
            }

            return true;
        }

        public string GetAssetPath(Object obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }

        public GameObject GetPrefab(GameObject obj)
        {
            if (!IsPrefab(obj))
            {
                return null;
            }

            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
            
            if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Model || prefab.transform.parent != null)
            {
                return null;
            }

            return prefab;
        }


        public bool IsPrefab(GameObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            
            PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(obj);
            PrefabInstanceStatus instanceStatus = PrefabUtility.GetPrefabInstanceStatus(obj);
            
            if (assetType != PrefabAssetType.NotAPrefab || instanceStatus != PrefabInstanceStatus.NotAPrefab)
            {
                return true;
            }

            return false;
        }
        
        public bool IsPrefabRoot(GameObject obj)
        {
            if (!IsPrefab(obj))
            {
                return false;
            }
            
            GameObject prefabAssetRoot = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
            return prefabAssetRoot == null && PrefabUtility.GetCorrespondingObjectFromSource(obj) == null;
        }
        
        public GameObject GetPrefabRoot(GameObject obj)
        {
            if (!IsPrefab(obj))
            {
                return null;
            }

            return PrefabUtility.GetNearestPrefabInstanceRoot(obj);
        }
        
        public T GetFirstAssetInProject<T>() where T : Object
        {
            List<T> results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[]{});
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null)
                {
                    continue;
                }
                results.Add(asset);
            }

            return results.FirstOrDefault();
        }

        public List<T> GetAssetsInProject<T>() where T : Object
        {
            List<T> results = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[]{});
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null)
                {
                    continue;
                }
                results.Add(asset);
            }

            return results;
        }
        
        public bool IsFolderValid(string path)
        {
            return AssetDatabase.IsValidFolder(path);
        }
        
        public T GetFirstAssetInParentDirectories<T>(string folderPath) where T : Object
        {
            if (string.IsNullOrEmpty(folderPath) || !IsFolderValid(folderPath))
            {
                Debug.LogError("Invalid folder path provided.");
                return null;
            }
            
            string currentPath = folderPath;
            while (!string.IsNullOrEmpty(currentPath))
            {
                string[] assets = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { currentPath });
                
                foreach (string guid in assets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset != null)
                        return asset;
                }
                
                int lastSlashIndex = currentPath.LastIndexOf('/');
                if (lastSlashIndex == -1) break;
                currentPath = currentPath.Substring(0, lastSlashIndex);
            }
            
            Debug.LogWarning($"No asset of type {typeof(T).Name} found in parent folders of '{folderPath}'.");
            return null;
        }
        
        public List<T> GetAssetsInParentDirectories<T>(string folderPath) where T : Object
        {
            List<T> assetsList = new List<T>();
            
            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Invalid folder path provided.");
                return assetsList;
            }
            
            string currentPath = folderPath;
            while (!string.IsNullOrEmpty(currentPath))
            {
                string[] assets = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { currentPath });
                
                foreach (string guid in assets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset != null && !assetsList.Contains(asset))
                    {
                        assetsList.Add(asset);
                    }
                }
                
                int lastSlashIndex = currentPath.LastIndexOf('/');
                if (lastSlashIndex == -1) break;
                currentPath = currentPath.Substring(0, lastSlashIndex);
            }

            if (assetsList.Count == 0)
            {
                Debug.LogWarning($"No assets of type {typeof(T).Name} found in parent folders of '{folderPath}'.");
            }
        
            return assetsList;
        }
        
        public string GetGUID(string path)
        {
            return AssetDatabase.FindAssets("", new[] { path }).FirstOrDefault();
        }
        
        public string GetPathFromGUID(string GUID)
        {
            return AssetDatabase.AssetPathToGUID(GUID);
        }

        public List<string> GetGUIDsInFolder(string path)
        {
            if (!IsFolderValid(path))
            {
                return new List<string>();
            }
            
            return AssetDatabase.FindAssets("", new[] { path }).ToList();
        }
        
        public List<string> GetGUIDsInFolders(List<string> paths)
        {
            return AssetDatabase.FindAssets("", paths.ToArray()).ToList();
        }

        public T GetAssetAtPath<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        
        public List<string> GetAssetsPathsInFolder(string path)
        {
            List<string> paths = new List<string>();
            
            List<string> guids = GetGUIDsInFolder(path);
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                paths.Add(assetPath);
            }

            return paths;
        }

        public void OpenAsset(Object asset)
        {
            AssetDatabase.OpenAsset(asset);
        }

        public void OpenAsset<T>(string objectPath) where T : Object
        {
            T loadedObject = LoadAssetAtPath<T>(objectPath);
            AssetDatabase.OpenAsset(loadedObject);
        }
        
        public void DeleteAssetsInFolder(string path)
        {
            DeleteAssetsInFolder<Object>(path);
        }
        
        public void DeleteAssetsInFolder<T>(string path) where T : Object
        {
            if (IsFolderValid(path))
            {
                List<string> assets = GetAssetsPathsInFolder(path);
                assets = assets.Where(x => !IsFolderValid(x)).ToList();

                for (int i = assets.Count - 1; i >= 0; i--)
                {
                    string assetPath = assets[i];
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset == null)
                    {
                        assets.Remove(assetPath);
                    }
                }
                if (assets.Count == 0)
                {
                    return;
                }
                List<string> failedPaths = new List<string>();
                AssetDatabase.DeleteAssets(assets.ToArray(), failedPaths);
                AssetDatabase.Refresh();
            }
        }

        public void DeleteAssetsAndSubFolders(string path)
        {
            DeleteAssetsAndSubFolders(path);
        }

        public void DeleteAssetsAndSubFolders<T>(string path) where T : Object
        {
            if (IsFolderValid(path))
            {
                List<string> assets = GetAssetsPathsInFolder(path);

                for (int i = assets.Count - 1; i >= 0; i--)
                {
                    string assetPath = assets[i];
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset == null)
                    {
                        assets.Remove(assetPath);
                    }
                }
                if (assets.Count == 0)
                {
                    return;
                }
                List<string> failedPaths = new List<string>();
                AssetDatabase.DeleteAssets(assets.ToArray(), failedPaths);
                AssetDatabase.Refresh();
            }
        }
        
        public List<string> GetAssetsPathsInFolders(List<string> paths)
        {
            List<string> objPaths = new List<string>();

            foreach (string path in paths)
            {
                objPaths.AddRange(GetAssetsPathsInFolder(path));
            }

            return objPaths;
        }
        
        public List<GameObject> GetPrefabsInFolder(string path)
        {
            List<GameObject> objs = new List<GameObject>();
            
            List<string> guids = GetGUIDsInFolder(path);
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (asset != null && IsPrefab(asset))
                {
                    objs.Add(asset);
                }
            }

            return objs;
        }

        public List<T> GetAssetsInDirectory<T>(string path) where T : Object
        {
            List<T> objs = new List<T>();
            
            List<string> guids = GetGUIDsInFolder(path);
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    objs.Add(asset);
                }
            }

            return objs;
        }
        
        public List<string> GetClassesPathsDirectory(string path)
        {
            List<string> guids = AssetDatabase.FindAssets("glob:\"*.cs\"", new[] { path }).ToList();
            List<string> results = new List<string>();
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (!string.IsNullOrWhiteSpace(assetPath))
                {
                    results.Add(assetPath);
                }
            }

            return results;
        }
        
        public List<string> GetAssetPathsInDirectory(string path, string type)
        {
            List<string> guids = AssetDatabase.FindAssets($"t:{type}", new[] { path }).ToList();
            List<string> results = new List<string>();
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (!string.IsNullOrWhiteSpace(assetPath))
                {
                    results.Add(assetPath);
                }
            }

            return results;
        }
        
        
        
        
        public List<T> GetAssetsInDirectories<T>(List<string> paths) where T : Object
        {
            List<T> objs = new List<T>();

            foreach (string path in paths)
            {
                objs.AddRange(GetAssetsInDirectory<T>(path));
            }

            return objs;
        }
        

        public string GetAssetDirectory(Object obj)
        {
            return GetAssetDirectory(GetAssetPath(obj));
        }

        public string GetAssetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }
        
                public Object SelectedObject()
        {
            return Selection.activeObject;
        }

        public List<Object> SelectedObjects()
        {
            return Selection.objects.ToList();
        }
        
        public string GetAssetFolder(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            return Path.GetDirectoryName(path);
        }
        
        public void RenameAsset(Object obj, string newName, bool autosave = false)
        {
            AssetDatabase.RenameAsset(GetAssetPath(obj), newName);
            DDElements.Assets.SetDirty(obj);
            if (autosave)
            {
                AssetDatabase.SaveAssetIfDirty(obj);
            }
        }

        public T LoadAssetAtPath<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        
        public T LoadAssetAtDirectory<T>(string directory) where T : Object
        {
            if (!IsFolderValid(directory))
            {
                return null;
            }
            
            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(IconsDatabase)}", new[] { directory });
            if (assetGuids.Length == 0)
            {
                return null;
            }
            
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public bool IsAssetMesh3D(string path)
        {
            GameObject asset = LoadAssetAtPath<GameObject>(path);
            
            if (asset != null)
            {
                MeshFilter meshFilter = asset.GetComponentInChildren<MeshFilter>();
                SkinnedMeshRenderer skinnedMeshRenderer = asset.GetComponentInChildren<SkinnedMeshRenderer>();

                if ((meshFilter != null && meshFilter.sharedMesh != null) || (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetProjectFolderName()
        {
            string[] dataPathFolder = Application.dataPath.Split("/");
            return dataPathFolder[^2];
        }
        public void UnloadUnusedAssets()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
    }
}