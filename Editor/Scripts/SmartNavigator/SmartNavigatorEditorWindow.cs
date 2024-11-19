using System.Collections.Generic;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class SmartNavigatorEditorWindow : EditorWindow
    {
        private static SmartNavigatorEditorWindow _window;
        private static Vector2 _windowSize = new Vector2(600, 350);
        
        private List<Module> _modules = new List<Module>();
        private Game _game;
        private Color hoverColor = new Color(0.25f, 0.32f, 0.68f, 0.51f);
        
        private Vector2 _modulesScroll;

        [MenuItem(StringLibrary.SMART_NAVIGATOR)]
        public static void Init()
        {
            if ((focusedWindow != null && focusedWindow.titleContent.text != "Project"))
            {
                return;
            }
            
            _window = CreateInstance<SmartNavigatorEditorWindow>();
            
            _window.position = DDElements.EditorUtils.GetPopupWindowPosition(_windowSize);
            _window.ShowPopup();
        }

        private void OnEnable()
        {
            _game = GameInspector.GameRootExists() ? GameInspector.GetGame() : null;
            if (_game == null)
            {
                return;
            }

            _modules = GameInspector.GetModules();
        }

        private void OnGUI()
        {
            if (_game == null)
            {
                return;
            }
            
            DDElements.Layout.DrawBackground(new Rect(new Vector2(0,0), new Vector2(position.width, position.height)), DDElements.Colors.BluishGray);
            DDElements.EditorUtils.PopupWindowCloseState(this);
            
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Row(() =>
                {
                    GameColumn();
                    ModulesColumn();
                });
            });
            
            Repaint();
        }
        
        private void GameColumn()
        {
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Space(5);
                DDElements.Layout.Row(() =>
                {
                    DDElements.Layout.Space(5);
                    DDElements.Rendering.IconButton(DDElements.Icons.HomeDefault("Go to Assets/"), 18, () =>
                    {
                        DDElements.Assets.PingFolder("Assets");
                        Close();
                    });
                    DDElements.Layout.Space(10);
                    DDElements.Rendering.IconButton(DDElements.Icons.Cubes("Go to Modules Folder"), 18, () =>
                    {
                        DDElements.Assets.PingInsideFolder(_game.GameModules);
                        Close();
                    });
                    DDElements.Layout.FlexibleSpace();
                });
                DDElements.Layout.FlexibleSpace();

                DDElements.Rendering.Line();
                DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Joystick(), "Go to Game", hoverColor, () =>
                {
                    DDElements.Assets.PingFolder(GameInspector.GetGamePath());
                    Close();
                }, options: GUILayout.Width(200));
                
                DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Cubes(), "Go to Main Module", hoverColor, () =>
                {
                    DDElements.Assets.Ping(GameInspector.GetMainModule());
                    Close();
                }, options: GUILayout.Width(200));
                
                DDElements.Layout.FlexibleSpace();
            }, style: DDElements.Styles.FlatColor(DDElements.Colors.MidBlue), options: GUILayout.Width(150));
        }
        
        private void ModulesColumn()
        {
            DDElements.Layout.ScrollView(ref _modulesScroll, () =>
            {
                DDElements.Layout.Column(() =>
                {
                    DDElements.Layout.FlexibleSpace();
                
                    for (int i = 0; i < _modules.Count; i++)
                    {
                        Module module = _modules[i];
                    
                        DDElements.Templates.LeadingIconAndButton(DDElements.Icons.Cubes(), $"{module.name}", hoverColor, () =>
                        {
                            DDElements.Assets.PingFolder(module.ModulePath);
                            Close();
                        });
                    }
                
                    DDElements.Layout.FlexibleSpace();
                });
            }, options: GUILayout.Width(400));
        }
    }
}