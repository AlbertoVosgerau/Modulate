using System;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class RenameAssetUtility : EndNameEditAction
    {
        public Action onRename;
    
        public override void Action(int instanceId, string path, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return;
            }
            
            onRename?.Invoke();
            Debug.Log($"Path: {path}, New Name: {newName}");
            AssetDatabase.RenameAsset(path, newName);
            AssetDatabase.SaveAssets();
        }
    }
}