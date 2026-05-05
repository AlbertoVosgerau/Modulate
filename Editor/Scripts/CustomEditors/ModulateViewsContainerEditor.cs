using System;
using System.Collections.Generic;
using System.Linq;
using DandyDino.Elements;
using Reflex.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(ModulateViewsContainer))]
    public class ModulateViewsContainerEditor : Editor
    {
        private ModulateViewsContainer _target;

        private List<IView> _views = new List<IView>();
        private List<Type> _classTypes = new List<Type>();
        private List<Editor> _viewEditors = new List<Editor>();
        private List<Module> _modulesPerView = new List<Module>();
        
        private List<IInstaller> _installers = new List<IInstaller>();
        private List<Type> _installerTypes = new List<Type>();
        private List<Editor> _installerEditors = new List<Editor>();
        private Dictionary<EntityId, bool> _installerExpanded = new Dictionary<EntityId, bool>();
        private Dictionary<EntityId, Vector2> _installerScroll = new Dictionary<EntityId, Vector2>();

        private void OnEnable()
        {
            _target = (ModulateViewsContainer)target;
            _classTypes = ReflectionUtility.GetAllClassesOfType<IView>();
            _installerTypes = ReflectionUtility.GetAllClassesOfType<IInstaller>();
            RefreshViews();
            RefreshInstallers();
        }

        private void OnDisable()
        {
            foreach (Editor editor in _viewEditors)
            {
                if (editor != null)
                {
                    DestroyImmediate(editor);
                }
            }
            _viewEditors.Clear();
            _modulesPerView.Clear();

            foreach (Editor editor in _installerEditors)
            {
                if (editor != null)
                {
                    DestroyImmediate(editor);
                }
            }
            _installerEditors.Clear();
            _installerExpanded.Clear();
            _installerScroll.Clear();
        }

        private void RefreshInstallers()
        { 
            foreach (Editor editor in _installerEditors)
            {
                if (editor != null)
                {
                    DestroyImmediate(editor);
                }
            }
            _installerEditors.Clear();

            _installers = _target.GetComponents<IInstaller>().ToList();

            foreach (IInstaller installer in _installers)
            {
                if (installer is ModulateViewsContainer)
                {
                    continue;
                }
                if (installer is MonoBehaviour mb)
                {
                    mb.hideFlags = HideFlags.HideInInspector;
                    _installerEditors.Add(CreateEditor(mb));
                }
            }
        }

        private void RefreshViews()
        {
            foreach (Editor editor in _viewEditors)
            {
                if (editor != null) DestroyImmediate(editor);
            }
            _viewEditors.Clear();
            _modulesPerView.Clear();

            _views = _target.GetComponents<IView>().ToList();

            foreach (IView view in _views)
            {
                if (view is BaseView unityObject)
                {
                    unityObject.hideFlags = HideFlags.HideInInspector;
                    _viewEditors.Add(CreateEditor(unityObject));

                    string moduleName = unityObject.GetType().Assembly.GetName().Name.Split('.').Last();
                    Module module = moduleName == "Main"
                        ? GameInspector.GetMainModule()
                        : GameInspector.GetModule(moduleName);
                    _modulesPerView.Add(module);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DDElements.Layout.Column(() =>
            {
                DrawViews();
                DrawSceneScopes();
            }, style: DDElements.Styles.FlatColor(DDElements.Colors.DeepPurple));
        }

    private void DrawSceneScopes()
    {
        DDElements.ReflectionUtilities.AddClassInstanceBar<IInstaller>(
            so: serializedObject,
            instantiator: type =>
            {
                Component component = Undo.AddComponent(_target.gameObject, type);
                return component as IInstaller;
            },
            onAdd: _ => RefreshInstallers(),
            groupLabel: "Scene Scopes",
            itemLabel: DDElements.Icons.WhiteAdd("Add Installer"),
            types: _installerTypes,
            target: _installers,
            buttonColor: DDElements.Colors.BluishGray,
            excludeTypes: new []{typeof(ModulateViewsContainer), typeof(ModulateGameInstanceInstaller)}
        );
        DDElements.Layout.Space(2);

        Editor toRemove = null;

        for (int i = 0; i < _installerEditors.Count; i++)
        {
            Editor editor = _installerEditors[i];
            if (editor == null || editor.target == null)
            {
                continue;
            }

            Editor capturedEditor = editor;
            MonoBehaviour installerMb = capturedEditor.target as MonoBehaviour;
            if (installerMb == null)
            {
                continue;
            }

            EntityId key = installerMb.GetEntityId();
            if (!_installerExpanded.ContainsKey(key))
            {
                _installerExpanded[key] = false;
            }
            if (!_installerScroll.ContainsKey(key))
            {
                _installerScroll[key] = Vector2.zero;
            }

            bool isEnabled = installerMb.enabled;
            bool isExpanded = _installerExpanded[key];

            Color headerColor = DDElements.Colors.DarkGray;
            Color bgColor = isEnabled ? DDElements.Colors.BluishGray : DDElements.Colors.DarkGray;

            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Column(() =>
                {
                    DDElements.Layout.Row(() =>
                    {
                        DDElements.Rendering.Switch(isEnabled, _ =>
                        {
                            installerMb.enabled = !installerMb.enabled;
                            EditorUtility.SetDirty(_target);
                        });

                        DDElements.Layout.FlexibleSpace();
                        DDElements.Rendering.Label(installerMb.GetType().Name);
                        DDElements.Layout.FlexibleSpace();

                        DDElements.Rendering.IconButton(DDElements.Icons.DependencyInjection("Edit Scene Scope class"), 16, () =>
                        {
                            MonoScript script = MonoScript.FromMonoBehaviour(installerMb);
                            if (script != null)
                            {
                                AssetDatabase.OpenAsset(script);
                            }
                        });

                        DDElements.Layout.Space(20);

                        if (isExpanded)
                        {
                            DDElements.Rendering.IconButton(DDElements.Icons.WhiteMinus("Hide"), 10, () =>
                            {
                                _installerExpanded[key] = false;
                            });
                        }
                        else
                        {
                            DDElements.Rendering.IconButton(DDElements.Icons.WhiteArrowDown("Show"), 10, () =>
                            {
                                _installerExpanded[key] = true;
                            });
                        }

                        DDElements.Layout.Space(5);
                        DDElements.Rendering.VerticalLine();
                        DDElements.Layout.Space(20);

                        DDElements.Rendering.IconButton(DDElements.Icons.Trash("Remove installer"), 16, () =>
                        {
                            Editor capturedForRemoval = capturedEditor;
                            DDElements.EditorUtils.DisplayYesNoDialog(
                                "Do you really want to remove Installer Component?",
                                () => { toRemove = capturedForRemoval; },
                                () => { },
                                "Remove Installer Component");
                        });

                        DDElements.Layout.Space();
                    });

                    DDElements.Layout.Space(5);
                }, style: DDElements.Styles.FlatColor(headerColor));

                Vector2 scroll = _installerScroll[key];
                DDElements.Layout.ScrollView(ref scroll, () =>
                {
                    if (_installerExpanded[key])
                    {
                        SerializedObject so = editor.serializedObject;
                        so.Update();

                        DDElements.Layout.Space(10);
                        DrawPropertiesExcluding(so, "m_Script");
                        so.ApplyModifiedProperties();
                        DDElements.Layout.Space(10);
                    }
                    else
                    {
                        DDElements.Layout.Space(2);
                    }
                });
                _installerScroll[key] = scroll;

            }, style: DDElements.Styles.FlatColor(bgColor));
        }

        if (toRemove != null)
        {
            Object targetComponent = toRemove.target;
            _installerEditors.Remove(toRemove);
            DestroyImmediate(toRemove);

            if (targetComponent != null)
            {
                Undo.DestroyObjectImmediate(targetComponent);
            }

            RefreshInstallers();
            Repaint();
            EditorUtility.SetDirty(_target);
            GUIUtility.ExitGUI();
        }
    }

        private void DrawViews()
        {
            DDElements.ReflectionUtilities.AddClassInstanceBar<IView>(
                so: serializedObject,
                instantiator: type =>
                {
                    Component component = Undo.AddComponent(_target.gameObject, type);
                    return component as IView;
                },
                onAdd: _ => RefreshViews(),
                groupLabel: "Views",
                itemLabel: DDElements.Icons.WhiteAdd("Add View"),
                types: _classTypes,
                target: _views,
                buttonColor: DDElements.Colors.MidPurple
            );
            DDElements.Layout.Space(2);

            Editor toRemove = null;

            for (int i = 0; i < _viewEditors.Count; i++)
            {
                Editor editor = _viewEditors[i];
                if (editor == null || editor.target == null) continue;

                Editor capturedEditor = editor;
                BaseView baseView = capturedEditor.target as BaseView;
                if (baseView == null) continue;

                MonoBehaviour viewMb = (MonoBehaviour)_views[i];
                bool isEnabled = viewMb.enabled;
                Module module = _modulesPerView[i];

                Color headerColor = isEnabled ? DDElements.Colors.DarkPurple : DDElements.Colors.DarkGray;
                Color bgColor = isEnabled ? DDElements.Colors.BluishGray : DDElements.Colors.DarkGray;

                DDElements.Layout.Column(() =>
                {
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.Row(() =>
                        {
                            DDElements.Rendering.Switch(isEnabled, _ =>
                            {
                                viewMb.enabled = !viewMb.enabled;
                                EditorUtility.SetDirty(_target);
                            });

                            DDElements.Layout.FlexibleSpace();
                            DDElements.Rendering.Label(viewMb.GetType().Name);

                            DDElements.Layout.FlexibleSpace();

                            DDElements.Rendering.IconButton(DDElements.Icons.CogWheel("Edit Manager class"), 16, () =>
                            {
                                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(module.ManagerClassPath));
                            });

                            DDElements.Layout.Space(10);

                            DDElements.Rendering.IconButton(DDElements.Icons.ScriptGray("Edit View class"), 16, () =>
                            {
                                MonoScript script = MonoScript.FromMonoBehaviour(viewMb);
                                if (script != null) AssetDatabase.OpenAsset(script);
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

                            DDElements.Layout.Space(20);

                            if (baseView.showEditor)
                            {
                                DDElements.Rendering.IconButton(DDElements.Icons.WhiteMinus("Hide"), 10, () =>
                                {
                                    baseView.showEditor = !baseView.showEditor;
                                    EditorUtility.SetDirty(_target);
                                });
                            }
                            else
                            {
                                DDElements.Rendering.IconButton(DDElements.Icons.WhiteArrowDown("Show"), 10, () =>
                                {
                                    baseView.showEditor = !baseView.showEditor;
                                    EditorUtility.SetDirty(_target);
                                });
                            }

                            DDElements.Layout.Space(5);
                            DDElements.Rendering.VerticalLine();
                            DDElements.Layout.Space(20);
                            DDElements.Rendering.IconButton(DDElements.Icons.Trash("Remove view"), 16, () =>
                            {
                                Editor capturedForRemoval = capturedEditor;
                                DDElements.EditorUtils.DisplayYesNoDialog(
                                    "Do you really want to remove View Component?",
                                    () => { toRemove = capturedForRemoval; },
                                    () => { },
                                    "Remove View Component");
                            });

                            DDElements.Layout.Space();
                        });

                        DDElements.Layout.Space(5);
                    }, style: DDElements.Styles.FlatColor(headerColor));

                    DDElements.Layout.ScrollView(ref baseView.scrollPosition, () =>
                    {
                        if (baseView.showEditor)
                        {
                            SerializedObject so = editor.serializedObject;
                            so.Update();

                            DDElements.Layout.Space(10);
                            DrawPropertiesExcluding(so, "m_Script");
                            so.ApplyModifiedProperties();
                            DDElements.Layout.Space(10);
                        }
                        else
                        {
                            DDElements.Layout.Space(2);
                        }
                    });

                }, style: DDElements.Styles.FlatColor(bgColor));
            }

            if (toRemove != null)
            {
                Object targetComponent = toRemove.target;
                _viewEditors.Remove(toRemove);
                DestroyImmediate(toRemove);

                if (targetComponent != null)
                {
                    Undo.DestroyObjectImmediate(targetComponent);
                }

                RefreshViews();
                Repaint();
                EditorUtility.SetDirty(_target);
                GUIUtility.ExitGUI();
            }

            DDElements.Layout.FlexibleSpace();
        }
    }
}