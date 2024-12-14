using System;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Rendering
    {
        public void FlatColorButton(GUIContent content, Color color, Action callback, params GUILayoutOption[] options)
        {
            Button(content, callback, DDElements.Styles.FlatColorButton(color), options);
        }
        
        public void IconButton(GUIContent icon,  Action callback)
        {
            IconButton(icon, 14, callback);
        }
        
        public void IconButton(GUIContent icon, int size, Action callback, float spacing = 0)
        {
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Space();
                Button(icon, () =>
                {
                    callback?.Invoke();
                }, style: DDElements.Styles.EmptyStyle(), new []{GUILayout.Width(size), GUILayout.Height(size), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)});
            });
        }
        
        public void Button(GUIContent content, Action callback, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.button);
            }

            if (GUILayout.Button(content, style, options))
            {
                callback?.Invoke();
            }
        }

        public void LabelField(GUIContent content,params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(content, options);
        }
        public void LabelField(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(content,style, options);
        }
        
        public void SearchBar(ref string queryStr, Action<string> onChange, params GUILayoutOption[] options)
        {
            string tempQuery = queryStr;
            DDElements.Layout.Row(() =>
            {
                DDElements.Rendering.Label(DDElements.Icons.Search(), 14);
                DDElements.Rendering.TextField(ref tempQuery, onChange, DDElements.Styles.TextFieldUnderline(), options);
            });
            queryStr = tempQuery;
        }
        
        public void Label(GUIContent content, int size)
        {
            GUILayout.Label(content, new []{GUILayout.Width(size), GUILayout.Height(size)});
        }
        
        public void Label(string content,params GUILayoutOption[] options)
        {
            DDElements.EditorUtils.InjectGUILayoutOption(ref options, GUILayout.ExpandWidth(false));
            GUIStyle style = DDElements.Styles.Label();
            GUILayout.Label(content,style, options);
        }
        public void Label(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            DDElements.EditorUtils.InjectGUILayoutOption(ref options, GUILayout.ExpandWidth(false));
            GUILayout.Label(content,style, options);
        }
        public void Label(string content, GUIStyle style, params GUILayoutOption[] options)
        {
            DDElements.EditorUtils.InjectGUILayoutOption(ref options, GUILayout.ExpandWidth(false));
            GUILayout.Label(content,style, options);
        }
        public void Label(GUIContent content,params GUILayoutOption[] options)
        {
            DDElements.EditorUtils.InjectGUILayoutOption(ref options, GUILayout.ExpandWidth(false));
            GUIStyle style = DDElements.Styles.Label();
            GUILayout.Label(content,style,  options);
        }
        public void Label(string content,  Color? borderColor = null, Color? backgroundColor = null, int border = 0, GUIStyle style = null, params GUILayoutOption[] options)
        {
            Label(new GUIContent(content), border, borderColor, backgroundColor, style, options);
        }
        public void Label(GUIContent content, int border, Color? borderColor , Color? backgroundColor , GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
                style = new GUIStyle(GUI.skin.label);

            if (border > 0)
            {
                if (!borderColor.HasValue)
                    borderColor = Color.black;

                if (!backgroundColor.HasValue)
                    backgroundColor = GUI.color;

                DrawUIBox(content, borderColor.Value, backgroundColor.Value, style, border);
            }
            else
            {
                if (backgroundColor.HasValue)
                    style.normal.background = DDElements.Helpers.GenerateColorTexture(backgroundColor.Value);

                GUILayout.Label(content, style, options);
            }
        }
        
        public void IntField(string label, ref int value, float labelWidth = 50)
        {
            IntField(label.ToGUIContent(), ref value, labelWidth);
        }
        public void IntField(GUIContent label, ref int value, float labelWidth = 50)
        {
            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
            value = EditorGUILayout.IntField(label, value, GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }
        
        public void RenameField(ref string str, params GUILayoutOption[] options)
        {
            string storedStr = str;
            str = EditorGUILayout.TextField(str, options);
        }
        
        public void RenameField(ref string str, Action<string> onChange, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            str = EditorGUILayout.TextField(str, options);
            if (EditorGUI.EndChangeCheck()) 
            {
                onChange?.Invoke(str);
            }
        }

        public void TextField(ref string str, Action<string> onChange, GUIStyle style = null, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            str = EditorGUILayout.TextField(str, style, options);
            if (EditorGUI.EndChangeCheck()) 
            {
                onChange?.Invoke(str);
            }
        }
        
        public void Line()
        {
            Line(1, 10, Color.gray);
        }
        
        public void Line(float thickness = 1, int padding = 10, Color color = default)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            
            if (color == default)
            {
                color = Color.gray;
            }
            EditorGUI.DrawRect(r, color);
        }
        
        public bool Switch(bool value,Action<bool> onChange = null)
        {
            return Switch(value, null, onChange);
        }
        
        public bool Switch(bool value, GUIContent label, Action<bool> onChange = null)
        {
            float height = 12;
            float witdh = height * 2;
            var newValue = value;

            GUIStyle style = new GUIStyle(GUIStyle.none);
            style.fixedWidth = witdh;
            
            DDElements.Layout.Row(() =>
            {
                DDElements.Layout.Space();
                DDElements.Layout.Column(() =>
                {
                    DDElements.Layout.Space();
                    GUIContent icon = newValue ? DDElements.Icons.SwitchOn() : DDElements.Icons.SwitchOff();
                    DDElements.Rendering.Button(icon, () =>
                    {
                        newValue = !newValue;
                    }, style: GUIStyle.none,GUILayout.Width(witdh), GUILayout.Height(height));
                });
                if (label != null)
                {
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.FlexibleSpace();
                        DDElements.Rendering.LabelField(label);
                        DDElements.Layout.FlexibleSpace();
                    });
                }
            }, style: style);

            if (newValue != value)
            {
                value = newValue;
                onChange?.Invoke(value);
            }

            return value;
        }

        public void DrawBannerTexture(Texture texture, float width, float height, ScaleMode scaleMode)
        {
            DDElements.EditorUtils.ReserveSpace(out Rect rect, width, height);
            EditorGUI.DrawPreviewTexture(rect, texture, null, scaleMode);
        }
        
        public void DrawTexture(Texture texture, float height, ScaleMode scaleMode, bool reservedSpace = true)
        {
            Rect rect = DDElements.Helpers.GetNextRect(height);
            GUI.DrawTexture(rect, texture, scaleMode);
            if (reservedSpace)
            {
                DDElements.Layout.Column(() =>
                {
                    EditorGUILayout.Space(height - 24);
                });
            }
        }
        private void DrawUIBox(GUIContent content, Color borderColor, Color backgroundColor, GUIStyle style, int width = 2)
        {
            Rect outter = GUILayoutUtility.GetRect(content, style);
            Rect inner = new Rect(outter.x + width, outter.y + width, outter.width - width * 2, outter.height - width * 2);
            EditorGUI.DrawRect(outter, borderColor);
            EditorGUI.DrawRect(inner, backgroundColor);
        }
        public void ProgressBar(float value, GUIContent content, float height, RectOffset padding, Color backgroundColor, Color mainColor, Color textColor, int fontSize = 0)
        {
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, height);
            float width = rect.width - rect.x - padding.right - 10;
            float xPosition = rect.x + padding.left;

            GUIStyle bgStyle = new GUIStyle();
            bgStyle.padding = padding;
            var bkpBg = bgStyle.normal.background;
            bgStyle.normal.background = DDElements.Helpers.GenerateColorTexture(backgroundColor);
            GUI.Box(new Rect(xPosition, rect.y, width, height), "");

            GUIStyle fgStyle = new GUIStyle();
            fgStyle.normal.background = DDElements.Helpers.GenerateColorTexture(mainColor);
            fgStyle.padding = padding;
            GUI.Box(new Rect(xPosition, rect.y, width * value, height), "", fgStyle);

            bgStyle.normal.background = bkpBg;

            GUIStyle textStyle = new GUIStyle();
            textStyle.alignment = TextAnchor.MiddleLeft;
            textStyle.clipping = TextClipping.Clip;
            if (fontSize > 0)
            {
                textStyle.fontSize = fontSize;
            }
            textStyle.padding = new RectOffset(5, 0, 0, 0);
            textStyle.normal.textColor = textColor;
            GUI.Box(new Rect(xPosition, rect.y, width, height), content, textStyle);
        }

        public void ModifiedPropertiesListener(SerializedObject so, Action action)
        {
            so.Update();
            action();
            so.ApplyModifiedProperties();
        }
        
        public void FloatSlider(GUIContent label, ref float value, float min, float max, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Slider(label, value, min, max, options);
        }
        
        public void DragAndDropArea<T>(Rect dropArea, Action<T> onDrop) where T : UnityEngine.Object
        {
            Event evt = Event.current;
        
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropArea.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                            {
                                if (obj is T)
                                {
                                    onDrop?.Invoke(obj as T);
                                }
                                
                            }
                        
                            Event.current.Use();
                        }
                    }
                    break;
            }
        }
        
        public void Dropdown(ref int selectedIndex, GUIContent label, GUIContent[] elements, Color color = default, params GUILayoutOption[] options)
        {
            Dropdown(ref selectedIndex, label, elements, null, color, options);
        }

        public void Dropdown(ref int selectedIndex, GUIContent label, GUIContent[] elements, Action<int> onIndexChange, Color color = default, params GUILayoutOption[] options)
        {

            color = color == default ? new Color(0.35f, 0.34f, 0.4f) : color;
            GUIStyle myStyle = new GUIStyle(EditorStyles.popup);
            myStyle.normal.background = DDElements.Helpers.GenerateColorTexture(color);
            
            int tempIndex = selectedIndex;
            int storedIndex = tempIndex;
            
            DDElements.Layout.Row(() =>
            {
                DDElements.Rendering.Label(label);
                tempIndex = EditorGUILayout.Popup(tempIndex, elements, myStyle, options);
                
                GUIContent icon = DDElements.Icons.ArrowDown();
                Rect lastRect = DDElements.Helpers.GetLastRect();
                Rect pos = new Rect(new Vector2(lastRect.x + lastRect.width - 20, lastRect.y), new Vector2(20, 20));
                GUI.Label(pos, icon);
            });

            if (storedIndex != tempIndex)
            {
                selectedIndex = tempIndex;
                onIndexChange?.Invoke(selectedIndex);
            }
        }
        
        public void Line(float thickness = 1, int padding = 10, Color color = default, int xPadding = 2, int width = -1)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6 - xPadding;
            
            if (width > 0)
            {
                r.width = width;
            }
            
            if (color == default)
            {
                color = Color.gray;
            }
            EditorGUI.DrawRect(r, color);
        }
        
        public void VerticalLine(float thickness = 1, int padding = 0, Color color = default, int yPadding = 0, int height = -1)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
            
            r.width = thickness;
            r.x += padding / 2 - 1;
            r.y -= 2;
            r.height += 6 - yPadding;
            
            if (height > 0)
            {
                r.height = height;
            }
            
            if (color == default)
            {
                color = Color.gray;
            }
            EditorGUI.DrawRect(r, color);
        }

        public void Icon(GUIContent icon, params GUILayoutOption[] options)
        {
            Icon(icon, 16);
        }
        
        public void Icon(GUIContent icon, int size, params GUILayoutOption[] options)
        {
            DDElements.EditorUtils.InjectGUILayoutOption(ref options, GUILayout.Width(size));
            DDElements.EditorUtils.InjectGUILayoutOption(ref options, GUILayout.Height(size));
            GUILayout.Label(icon, DDElements.Styles.Label(size), options);
        }
    }
}