using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Listeners
    {
        public void CheckForChanges(SerializedObject serializedObject,Action content,  Action onChanged = null)
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            content?.Invoke();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                onChanged?.Invoke();
            }
        }
        
        public void OnRectClick(Action onClick)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            OnRectClick(lastRect,onClick);
        }

        public void OnRectClick(Rect rect, Action onClick)
        {
            bool isHovered = rect.Contains(Event.current.mousePosition);

            if ((Event.current.type == EventType.MouseDown ||  Event.current.type == EventType.MouseDrag) && isHovered)
            {
                onClick?.Invoke();
                Event.current.Use();
                GUI.changed = true;
            }
        }
    }
}