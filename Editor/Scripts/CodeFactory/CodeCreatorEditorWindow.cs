using System;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class CodeCreatorEditorWindow : EditorWindow
    {
        private static CodeCreatorEditorWindow _window;
        private static string _path;
        private static GUIContent _icon;
        private bool _hasFocused = false;

        private static TemplateType _templateType;
        private static string _type = "FileType";
        public static string _className = "";
        public static string _menuName = "MyMenu";
        private static string _fileName = "MyFile";
        private static string _additionalUsing = "";
        private static readonly int height = 23;
        
        public static void OpenPopup(GUIContent icon, TemplateType templateType, string path, string className = "MyClass", string menuName = "MyMenu", string fileName = "MyFile", string additionalUsing = "",  Type type = null)
        {
            _path = path;
            _icon = icon;
            _templateType = templateType;
            _className = className;
            _menuName = menuName;
            _fileName = fileName;
            _additionalUsing = "";
            if (type != null)
            {
                _type = type.Name;
            }
            
            _window = CreateInstance<CodeCreatorEditorWindow>();
            int multiplier = 1;
            if (_templateType == TemplateType.EditorWindow)
            {
                multiplier++;
            }
            if (_templateType == TemplateType.ScriptableObject)
            {
                multiplier+= 2;
            }
            
            _window.position = DDElements.EditorUtils.GetPopupWindowPosition(new Vector2(300, height * multiplier + 6));
            _window.ShowPopup();
        }

        private void OnGUI()
        {
            DDElements.Layout.DrawBackground(new Rect(new Vector2(0,0), new Vector2(position.width, position.height)), DDElements.Colors.MidDarkGray);
            DDElements.EditorUtils.PopupWindowCloseState(this);
            
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.FlexibleSpace();

                DDElements.Layout.Row(() =>
                {
                    DDElements.Layout.Space(4);
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.FlexibleSpace();
                        DDElements.Rendering.Icon(_icon, height);
                        DDElements.Layout.FlexibleSpace();
                    });
                    
                    DDElements.Layout.Space(8);
                    
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.FlexibleSpace();

                        if (_templateType == TemplateType.EditorWindow)
                        {
                            DDElements.Layout.Row(() =>
                            {
                                DDElements.Rendering.Label("Menu Name:", options: GUILayout.Width(70));
                                DDElements.Layout.Space(5);
                                DDElements.Rendering.TextField(ref _menuName, str =>
                                {
                        
                                }, style: DDElements.Styles.TextFieldUnderline(), options: GUILayout.Height(height));
                            });
                        }
                        else if (_templateType == TemplateType.ScriptableObject)
                        {
                            DDElements.Layout.Row(() =>
                            {
                                DDElements.Rendering.Label("Menu Name:", options: GUILayout.Width(70));
                                DDElements.Layout.Space(5);
                                DDElements.Rendering.TextField(ref _menuName, str =>
                                {
                        
                                }, style: DDElements.Styles.TextFieldUnderline(), options: GUILayout.Height(height));
                            });
                            
                            DDElements.Layout.Row(() =>
                            {
                                DDElements.Rendering.Label("File Name:", options: GUILayout.Width(70));
                                DDElements.Layout.Space(5);
                                DDElements.Rendering.TextField(ref _fileName, str =>
                                {
                        
                                }, style: DDElements.Styles.TextFieldUnderline(), options: GUILayout.Height(height));
                            });
                        }
                        
                        
                        GUI.SetNextControlName("ClassName");
                        DDElements.Rendering.TextField(ref _className, str =>
                        {
                        
                        }, style: DDElements.Styles.TextFieldUnderline(), options: GUILayout.Height(height));
                        
                        if (!_hasFocused)
                        {
                            GUI.FocusControl("ClassName");
                            _hasFocused = true; 
                        }
                        
                        _className = _className.Trim();
                        _className = _className.Replace(" ", "");
                        DDElements.Layout.FlexibleSpace();
                    });
                    
                    DDElements.Layout.Space(8);
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.FlexibleSpace();
                        DDElements.Rendering.IconButton(DDElements.Icons.Check(), height, () =>
                        {
                            CreateCode();
                        });
                        DDElements.Layout.Space(3);
                        DDElements.Layout.FlexibleSpace();
                    });
                });
                DDElements.Layout.FlexibleSpace();
            });
            
            if (UnityEngine.Event.current.keyCode == KeyCode.Return)
            {
                CreateCode();
            }
            Repaint();
        }

        private void CreateCode()
        {
            Close();
            ClassGenerator classGenerator = new ClassGenerator()
            {
                name = _className,
                menuName = _menuName,
                fileName = _fileName,
                type = _type,
                additionalUsing = _additionalUsing
            };

            classGenerator.GenerateClass(_templateType, _path, _className, true);
        }
    }
}
