using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DandyDino.Modulate;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DandyDino.Modulate
{
    public class ManagerContainerRenderingUtility
    {
        public static void DrawSerializedObjectsItemsList<T>(List<bool> drawContentList, SerializedObject serializedObject, SerializedProperty serializedProperty, List<T> itemsIst, Action<T> onBeforeItem = null, Action<T> onAfterItem = null) where T : class
        {
            DDElements.Listeners.CheckForChanges(serializedObject, () =>
            {
                DDElements.Layout.Column(() =>
                {
                    for (int i = 0; i < itemsIst.Count; i++)
                    {
                        T item = itemsIst[i];
                        SerializedProperty property = serializedProperty.GetArrayElementAtIndex(i);
                        string managerName = property.managedReferenceValue.GetType().Name;
                        string moduleName = Regex.Replace(managerName, @"Manager$", "");
                        Module module = moduleName == "Main"? GameInspector.GetMainModule() : GameInspector.GetModule(moduleName);
                
                        DDElements.Layout.Column(() =>
                        {
                            DDElements.Layout.Row(() =>
                            {
                                onBeforeItem?.Invoke(item);
                                DDElements.Layout.Space(10);
                                DDElements.Rendering.IconButton(DDElements.Icons.Folder("Show in Project"), 16, () =>
                                {
                                    moduleName = Regex.Replace(moduleName, @"Service", "");
                                    Module module = GameInspector.GetModule(moduleName);
                                    
                                    DDElements.Assets.Ping(module);
                                });
                                DDElements.Layout.FlexibleSpace();
                                
                                GUILayout.Label(managerName);
                                DDElements.Layout.FlexibleSpace();
                                
                                

                                DDElements.Rendering.IconButton(DDElements.Icons.CogWheel("Edit Manager class"), 16, () =>
                                {
                                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(module.ManagerClassPath));
                                });
                                
                                DDElements.Layout.Space(10);
                                
                                DDElements.Rendering.IconButton(DDElements.Icons.ScriptGray("Edit Service class"), 16, () =>
                                {
                                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(module.ServicesClassPath));
                                });
                                
                                DDElements.Layout.Space(10);
                                
                                DDElements.Rendering.IconButton(DDElements.Icons.Envelope("Edit Events class"), 16, () =>
                                {
                                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(module.EventsClassPath));
                                });
                                
                                DDElements.Layout.Space(15);
                                
                                DDElements.Rendering.IconButton(DDElements.Icons.Unity("Open MonoBehaviour folder"), 16, () =>
                                {
                                    DDElements.Assets.PingInsideFolder(module.MonoBehaviourDirectory);
                                });
                                
                                
                                DDElements.Layout.Space(30);
                                DDElements.Rendering.IconButton(DDElements.Icons.Delete(), 16, () =>
                                {
                                    Undo.RecordObject(serializedObject.targetObject, "Delete Item");
                                    if (Application.isPlaying)
                                    {
                                        ((ManagerContainer)serializedObject.targetObject).RemoveManager((IManager)item);
                                    }
                                    else
                                    {
                                        itemsIst.Remove(item);
                                    }
                                    DDElements.Assets.SetDirtyAndSave(serializedObject.targetObject);
                                });
                            });
                            
                            DDElements.Listeners.OnRectClick(() =>
                            {
                                drawContentList[i] = !drawContentList[i];
                            });
                            
                            DDElements.Layout.Row(() =>
                            {
                                DDElements.Layout.Space(3);
                                DDElements.Rendering.Line();
                                DDElements.Layout.Space(6);
                            });
                            if (drawContentList[i])
                            {
                                DrawSerializedPropertyItem(property, item);
                            }
                            
                        }, style: EditorStyles.helpBox);
                        DDElements.Layout.Space(2);
                    }
                }, Layout.Alignment.START);
            });
        }
        
        
        
        public static void DrawSerializedPropertyItem<T>(SerializedProperty property, T item) where T : class
        {
            if (item == null)
            {
                return;
            }
            DDElements.Layout.Row(() =>
            {
                GUILayout.Space(18);
                
                EditorGUILayout.PropertyField(property, true);
            }, style: GUIStyle.none);
        }
    }
}
