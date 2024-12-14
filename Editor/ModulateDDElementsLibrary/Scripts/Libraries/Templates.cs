using System;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public partial class Templates
    {
        public void LeadingIconAndButton(GUIContent icon, string title, Color hoverColor, Action onClick, Action leadingContent = null, Action trailingContent = null, params GUILayoutOption[] options)
        {
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Row(() =>
                {
                    leadingContent?.Invoke();
                    DDElements.Layout.Space(10);
                    DDElements.Rendering.Icon(icon, 18);
                    DDElements.Rendering.FlatColorButton(title.ToGUIContent(), DDElements.Colors.Clear, onClick, options);
                    DDElements.Layout.Space(4);
                    trailingContent?.Invoke();
                });
                
                Rect lastRect = DDElements.Helpers.GetLastRect();
                if (lastRect.Contains(Event.current.mousePosition))
                {
                    Handles.DrawSolidRectangleWithOutline(lastRect, hoverColor, Color.clear);
                }
                
                DDElements.Rendering.Line();
            }, options: options);
        }
    }
}