using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Layout
    {
        public enum Alignment
        {
            START,
            END,
            MIDDLE,
            NONE
        }
        
        public void DrawBackground(EditorWindow window, Color color)
        {
            GUI.Box(new Rect(0,0, window.position.width, window.position.height), "", DDElements.Styles.FlatColor(color));
        }
        
        public void DrawBackground(Rect rect, Color color)
        {
            GUI.Box(rect, "", DDElements.Styles.FlatColor(color));
        }
        
        public bool RectIsHovered(Rect rect)
        {
            bool isHovered = rect.Contains(Event.current.mousePosition);
            return isHovered;
        }
        
        public void Row(Action content, Alignment alignment = Alignment.NONE, GUIStyle style = null, params GUILayoutOption[] options)
        {
            style = style == null ? GUIStyle.none : style;
            GUILayout.BeginHorizontal(style, options);
            ApplyAlignment(content, alignment);
            GUILayout.EndHorizontal();
        }
        
        public void Column(Action content, Alignment alignment = Alignment.NONE, GUIStyle style = null, params GUILayoutOption[] options)
        {
            style = style == null ? GUIStyle.none : style;
            GUILayout.BeginVertical(style, options);
            ApplyAlignment(content, alignment);
            GUILayout.EndVertical();
        }

        public void Column(Color color1, Color color2, int count, Action<int> onDrawItem)
        {
            Column(() =>
            {
                for (var i = 0; i < count; i++)
                {
                    var color = i % 2 == 0 ? color1 : color2;
                    Row(() => { onDrawItem?.Invoke(i); }, style: DDElements.Styles.FlatColor(color));
                }
            });
        }

        public void ScrollView(ref Vector2 scrollPos, Action content, params GUILayoutOption[] options)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, options);
            content();
            GUILayout.EndScrollView();
        }

        public void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }

        public void Space(int space = 5)
        {
            GUILayout.Space(space);
        }

        private void ApplyAlignment(Action content, Alignment alignment)
        {
            if (alignment == Alignment.MIDDLE || alignment == Alignment.END)
            {
                GUILayout.FlexibleSpace();
            }
            content();
            if (alignment == Alignment.MIDDLE || alignment == Alignment.START)
            {
                GUILayout.FlexibleSpace();
            }
        }

        public void CollapsableItem(ref bool isOpen, Action header, Action content)
        {
            var tempIsOpen = isOpen;
            Column(() =>
            {
                Column(() =>
                {
                    Row(() =>
                    {
                        Column(() =>
                        {
                            var icon = tempIsOpen
                                ? DDElements.Icons.ArrowDown()
                                : DDElements.Icons.Dash();

                            Space(8);
                            DDElements.Rendering.Icon(icon, 16);
                        }, options: new[]
                        {
                            GUILayout.Width(15),
                            GUILayout.Height(15),
                        });
                        header?.Invoke();
                    });

                    DDElements.Listeners.OnRectClick(() => { tempIsOpen = !tempIsOpen; });

                    Row(() =>
                    {
                        Space(3);
                        DDElements.Rendering.Line();
                        Space(6);
                    });
                    if (tempIsOpen)
                    {
                        content?.Invoke();
                    }
                }, style: EditorStyles.helpBox);
                Space(2);
            });

            isOpen = tempIsOpen;
        }
        
        public void Tabs(ref int index, string[] tabs, Action[] actions, params GUILayoutOption[] options)
        {
            Tabs(ref index, tabs, actions, null, options);
        }

        public void Tabs(ref int index, string[] tabs, Action[] actions, Action<int> onTabChanged, params GUILayoutOption[] options)
        {
            var storedIndex = index;
            index = GUILayout.Toolbar(index, tabs);

            for (var i = 0; i < tabs.Length; i++)
            {
                if (i != index)
                {
                    continue;
                }

                actions[i]?.Invoke();
            }

            if (storedIndex != index)
            {
                onTabChanged?.Invoke(index);
            }
        }
    }
}