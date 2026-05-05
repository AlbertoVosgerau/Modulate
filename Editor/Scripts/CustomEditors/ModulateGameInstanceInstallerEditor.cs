using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(ModulateGameInstanceInstaller))]
    public class ModulateGameInstanceInstallerEditor : Editor
    {
        private SerializedProperty _featureToggleAsset;
        private Editor _featureToggleAssetEditor;
        private Object _cachedAsset;

        private void OnEnable()
        {
            _featureToggleAsset = serializedObject.FindProperty("_featureToggleAsset");
            RebuildSubEditor();
        }

        private void OnDisable()
        {
            if (_featureToggleAssetEditor != null)
            {
                DestroyImmediate(_featureToggleAssetEditor);
                _featureToggleAssetEditor = null;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_featureToggleAsset);
            
            if (_featureToggleAsset.objectReferenceValue != _cachedAsset)
            {
                RebuildSubEditor();
            }

            if (_featureToggleAssetEditor != null)
            {
                _featureToggleAssetEditor.OnInspectorGUI();
            }
            else
            {
                EditorGUILayout.HelpBox("Assign a Feature Toggle Asset to configure managers.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RebuildSubEditor()
        {
            if (_featureToggleAssetEditor != null)
            {
                DestroyImmediate(_featureToggleAssetEditor);
                _featureToggleAssetEditor = null;
            }

            _cachedAsset = _featureToggleAsset.objectReferenceValue;
            if (_cachedAsset != null)
            {
                _featureToggleAssetEditor = CreateEditor(_cachedAsset);
            }
        }
    }
}