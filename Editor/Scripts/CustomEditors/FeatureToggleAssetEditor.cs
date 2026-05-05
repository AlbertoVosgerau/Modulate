using System.Collections.Generic;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(FeatureToggleAsset))]
    public class FeatureToggleAssetEditor : Editor
    {
        private FeatureToggleAsset _target;

        private void OnEnable()
        {
            _target = (FeatureToggleAsset)target;
            _target.Refresh();
        }

        public override void OnInspectorGUI()
        {
            List<IManager> managers = _target.Managers;
            if (managers == null)
            {
                return;
            }
            
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Row(() =>
                {
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.Space(10);
                
                        DDElements.Layout.Row(() =>
                        {
                            DDElements.Layout.FlexibleSpace();
                            DDElements.Rendering.Label("Feature Toggle",
                                style: DDElements.Styles.Label(16, FontStyle.Bold, TextAnchor.MiddleCenter));
                            DDElements.Layout.FlexibleSpace();
                        });
                        DDElements.Layout.Space(5);
                        DDElements.Rendering.Label("Use Feature Toggle to enable or disable Managers. Disabled features won't be created on Startup.");
                        DDElements.Rendering.Line();
                        DDElements.Layout.Space(10);
                
                        for (int i = 0; i < managers.Count; i++)
                        {
                            IManager manager = managers[i];
                            if (manager == null) continue;
                
                            System.Type managerType = manager.GetType();
                            string managerName = managerType.Name;
                            bool isEnabled = _target.IsEnabled(managerType);
                
                            DDElements.Layout.Column(() =>
                            {
                                DDElements.Layout.Row(() =>
                                {
                                    DDElements.Layout.Row(() =>
                                    {
                                        DDElements.Layout.Row(() =>
                                        {
                                            DDElements.Rendering.Switch(isEnabled, value =>
                                            {
                                                Undo.RecordObject(_target, "Toggle Manager");
                                                _target.SetEnabled(managerType, value);
                                                EditorUtility.SetDirty(_target);
                                                AssetDatabase.SaveAssetIfDirty(_target);
                                            });
                
                                            DDElements.Layout.Space(5);
                                            DDElements.Rendering.Label(managerName);
                                        }, style: DDElements.Styles.FlatColor(DDElements.Colors.DarkGray));
                                    });
                                });
                            });
                
                            DDElements.Layout.Space(5);
                        }
                    });
                    DDElements.Layout.Space(12);
                });
                
                DDElements.Layout.FlexibleSpace();
            }, style: DDElements.Styles.FlatColor(DDElements.Colors.DarkGray));
        }
    }
}