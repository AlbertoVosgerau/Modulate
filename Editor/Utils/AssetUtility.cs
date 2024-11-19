using UnityEditor;
using UnityEngine;

namespace DandiDino.Modulate
{
    public class AssetUtility
    {
        public static string GetResourcesPath(string parentFolder = null)
        {
            return string.IsNullOrWhiteSpace(parentFolder) ? "Assets/Resources" : $"Assets/{parentFolder}/Resources";
        }
        
        public static T GetOrCreateScriptableObject<T>(string folder, string fileName) where T : ScriptableObject
        {
            T asset = Resources.Load<T>(fileName);
            if (asset != null)
            {
                return asset;
            }
            
#if UNITY_EDITOR
            asset = AssetUtility.CreateScriptableObjectAsset<T>(folder, fileName);
#endif
            return asset;
        }
        public static T CreateScriptableObjectAsset<T>(string folder, string fileName) where T : ScriptableObject
        {
            T newInstance = ScriptableObject.CreateInstance<T>();
            string path = $"{folder}/{fileName}.asset";
     
            AssetDatabase.CreateAsset(newInstance, path);
            AssetDatabase.Refresh();
            return Resources.Load<T>(fileName);
        }
    }
}