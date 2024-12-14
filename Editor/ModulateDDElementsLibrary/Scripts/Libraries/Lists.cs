using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Lists
    {
        
        
        public void ReorderableList<T>(bool canReorder, ref int selectedIndex, List<T> list,int spacing, int height, Color defaultColor, Color hoverColor,Action<int, int> onItemReordered,  Action<int, T> elementRenderer, List<int> customHeights = null) where T : class
        {
            bool mouseIsDragged = Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown;
            if (!canReorder)
            {
                mouseIsDragged = false;
                selectedIndex = -1;
            }
            
            GUILayout.BeginVertical();
            GUILayout.Space(spacing);
            for (int i = 0; i < list.Count; i++)
            {
                int localHeight = height;
                bool isSelected =  mouseIsDragged && i == selectedIndex;
                
                if (customHeights != null)
                {
                    if (i < customHeights.Count)
                    {
                        localHeight = customHeights[i];
                    }
                }

                if (isSelected)
                {
                    localHeight += localHeight/3;
                }

                // This will reserve a space that can be filled later and return the rect, so we can inspect an element before we draw it.
                // This is important because we need to check if we are hovering an index BEFORE drawing it, so we can replace items
                // For some reason height is always returning 18 (which is de default Editor indentation, that's a clue).
                // That's why we set the height right after it.
                Rect nextRect = EditorGUILayout.GetControlRect();
                nextRect.height = localHeight;
                T label = list[i];

                if (mouseIsDragged)
                {
                    EditorGUIUtility.AddCursorRect(nextRect, MouseCursor.Pan);
                }

                GUIStyle style = new GUIStyle();
                style.fixedHeight = localHeight;
                style.margin = new RectOffset(2,1,2,2); 
            
                GUILayout.BeginHorizontal();

                ListElement(mouseIsDragged, ref selectedIndex, nextRect, localHeight, i, list, onItemReordered, (isHovered) =>
                {
                    Color color = isHovered ? hoverColor : defaultColor;
                    style.normal.background = DDElements.Helpers.GenerateColorTexture(color);
                
                    DrawItemInternal(() =>
                    {
                        elementRenderer(i, label);
                    }, style);
                });
                GUILayout.EndHorizontal();
                GUILayout.Space(spacing);
            }
            GUILayout.EndVertical();
        }
        
        // TODO: This is Core specific and not generic at this point.
        
        
        private void DrawItemInternal(Action drawItem, GUIStyle style)
        {
            DDElements.Layout.Row(() =>
            {
                drawItem();
            }, style : style);
        }
        
        private void OnElementHovered<T>(bool mouseIsDragged, ref int selectedIndex, List<T> list, int index, Action<int, int> onItemReordered) where T : class
        {
            // Mouse dragged is being passed as argument because we need a better solution in place.
            // Event.current.type == EventType.MouseDrag doesn't seem to work here for some reason. Need a better solution.
            if (mouseIsDragged)
            {
                if (selectedIndex < 0)
                {
                    selectedIndex = index;
                    return;
                }

                if (index != selectedIndex)
                {
                    int oldIndex = selectedIndex;
                    int newIndex = index;
                    
                    T item = list[oldIndex];
                    list.RemoveAt(oldIndex);
                    list.Insert(newIndex, item);
                    selectedIndex = newIndex;
                    onItemReordered(oldIndex, newIndex);
                }
            }
            else
            {
                selectedIndex = -1;
            }
        }

        private void ListElement<T>(bool mouseIsDragged, ref int selectedIndex, Rect rect, int height, int index, List<T> list, Action<int, int> onItemReordered, Action<bool> DrawElement) where T : class
        {
            bool isHovered = DDElements.Layout.RectIsHovered(rect);
        
            if (isHovered)
            {
                OnElementHovered<T>(mouseIsDragged, ref selectedIndex,  list, index, onItemReordered);
            }
        
            DDElements.Layout.Column(() =>
            {
                // This is the trick that makes everything work.
                // After we reserved the space with EditorGUILayout.GetControlRect(), we will have an empty space
                // The the very next thing we draw will have a negative space of the same value. Now our item will appear were we checked the hover.
                EditorGUILayout.Space(-18);
                DrawElement(isHovered);
            });
        }
    }
}