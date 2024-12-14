using System;
using DandyDino.Modulate;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class CodeFactoryEditorWindow : EditorWindow
    {
        private static CodeFactoryEditorWindow _window;
        private static string _currentPath;
        private static bool _gameExists;
        private Color _hoverColor = new Color(0.4f, 0.27f, 0.68f, 0.16f);
        
        [Shortcut(StringLibrary.CODE_FACTORY_WINDOW, KeyCode.N, ShortcutModifiers.Control | ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
        public static void Init()
        {
            _gameExists = GameInspector.GameRootExists();
            _currentPath = DDElements.Assets.GetActiveFolderViaReflection();

            if (string.IsNullOrWhiteSpace(_currentPath) || (focusedWindow != null && focusedWindow.titleContent.text != "Project"))
            {
                return;
            }
            
            _window = CreateInstance<CodeFactoryEditorWindow>();

            int height = 35;

            int multiplier = 6;


            if (DDElements.Assets.IsSelectedMonoBehaviour() || DDElements.Assets.IsSelectedScriptableObject())
            {
                multiplier++;
            }
            
            Vector2 windowSize = !GameInspector.GameRootExists()? new Vector2(250, height) : new Vector2(250, (height * multiplier) + 40);
            _window.position = DDElements.EditorUtils.GetPopupWindowPosition(windowSize);
            _window.ShowPopup();
        }
        
        private void OnGUI()
        {
            if (string.IsNullOrWhiteSpace(_currentPath))
            {
                return;
            }
            
            DDElements.Layout.DrawBackground(new Rect(new Vector2(0,0), new Vector2(position.width, position.height)), DDElements.Colors.MidDarkGray);
            DDElements.EditorUtils.PopupWindowCloseState(this);
            
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Space(6);
                if (!_gameExists)
                {
                    DDElements.Templates.LeadingIconAndButton(DDElements.Icons.CSharp(), "Create Game", _hoverColor, () =>
                    {
                        Close();
                        CreateGameWindow window = GetWindow<CreateGameWindow>();
                        window.Init(_currentPath);
                    });
                }
                else
                {
                    if (!_currentPath.Contains("Editor"))
                    {
                        DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Monkey(), "Create MonoBehaviour", _hoverColor, () =>
                        {
                            Close();
                            CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.Monkey(), TemplateType.MonoBehaviour, _currentPath, className: "NewMonoBehaviour");
                        });
                    }
                    else
                    {
                        DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Window(), "Create Editor Window",  _hoverColor,() =>
                        {
                            Close();
                            CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.Window(), TemplateType.EditorWindow, _currentPath, className:"NewEditorWindow");
                        });
                    }

                    if (DDElements.Assets.IsSelectedMonoBehaviour() || DDElements.Assets.IsSelectedScriptableObject())
                    {
                        DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Unity(), "Create Custom Inspector", _hoverColor, () =>
                        {
                            Close();
                            Type type = DDElements.Assets.SelectedObject().GetType();
                            Type classType = null;
                            if (type == typeof(MonoScript))
                            {
                                classType = ((MonoScript)DDElements.Assets.SelectedObject()).GetClass();
                            }
                            Module module = GameInspector.GetModuleInParentDirectories(_currentPath);
                            string editorScripts = GameInspector.GetModuleEditorScriptsPath(module);
                            
                            DDElements.Assets.PingInsideFolder(editorScripts);
                            CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.Unity(), TemplateType.CustomInspector, editorScripts, className: $"{DDElements.Assets.SelectedObject().name}Editor", type: classType);
                        });
                    }

                    DDElements.Templates.LeadingIconAndButton(DDElements.Icons.CSharp(), "Create Empty Class", _hoverColor, () =>
                    {
                        Close();
                        CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.CSharp(), TemplateType.EmptyClass, _currentPath, className:"NewEmptyClass");
                    });
                    
                    DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Layers(), "Create Interface",  _hoverColor,() =>
                    {
                        Close();
                        CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.Layers(), TemplateType.Interface, _currentPath, className: "INewInterface");
                    });
                    
                    DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Enumerate(), "Create Enum", _hoverColor, () =>
                    {
                        Close();
                        CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.Enumerate(), TemplateType.Enum, _currentPath, className:"NewEnumType");
                    });
                    
                    DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Comment(), "Create ScriptableObject",  _hoverColor,() =>
                    {
                        Close();
                        CodeCreatorEditorWindow.OpenPopup(DDElements.Icons.Comment(), TemplateType.ScriptableObject, _currentPath, className: "NewTypeOfObject");
                    });

                    DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Cubes(), "Create Module",  _hoverColor,() =>
                    {
                        Close();
                        string modulesPath = GameInspector.GetModulesPath();
                        DDElements.Assets.PingInsideFolder(modulesPath);
                        CreateModuleWindow window = GetWindow<CreateModuleWindow>();
                        window.Init(modulesPath);
                    }); 
                }
                DDElements.Layout.FlexibleSpace();
            });
            Repaint();
        }
    }
}
