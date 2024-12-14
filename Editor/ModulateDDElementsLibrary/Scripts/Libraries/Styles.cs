using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Styles
    {
        public GUIStyle TextFieldUnderline(Color textColor = default)
        {
            if (textColor == default)
            {
                textColor = Color.white;
            }
            
            GUIStyle underlineStyle = new GUIStyle(GUI.skin.textField)
            {
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,

                normal = { background = null, textColor = textColor },
                focused = { background = null, textColor = textColor },
                hover = { background = null, textColor = textColor },
                active = { background = null, textColor = textColor },

                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 2),
            };

            return underlineStyle;
        }
        public GUIStyle LargeLabel()
        {
            return LargeLabel(24);
        }
        
        public GUIStyle LargeLabel(int size)
        {
            GUIStyle style = new GUIStyle(EditorStyles.largeLabel);
            style.fontSize = size;
            return style;
        }

        public GUIStyle Label()
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            return style;
        }
        
        public GUIStyle Label(int fontSize)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = fontSize;
            return style;
        }
        
        public GUIStyle Label(int fontSize, FontStyle fontStyle)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            return style;
        }
        
        public GUIStyle Label(int fontSize, TextAnchor textAnchor)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = fontSize;
            style.alignment = textAnchor;
            return style;
        }
        
        public GUIStyle Label(int fontSize, FontStyle fontStyle, TextAnchor textAnchor)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            style.alignment = textAnchor;
            return style;
        }
        
        
        public GUIStyle FlatColorButton(Color normal)
        {
            return FlatColorButton(normal, normal, normal);
        }
        
        public GUIStyle FlatColorButton(Color normal, Color pressed)
        {
            return FlatColorButton(normal, normal, pressed);
        }
        
        public GUIStyle FlatColorButton(Color normal, Color hover, Color pressed)
        {
            return FlatColorButton(normal, hover, normal, hover);
        }
        
        public GUIStyle FlatColorButton(Color normal, Color hover, Color pressed, Color focused)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);


            style.normal.background = DDElements.Helpers.GenerateColorTexture(normal);
            style.normal.textColor = Color.white;

            style.hover.background = DDElements.Helpers.GenerateColorTexture(hover);
            style.hover.textColor = Color.white;
            
            style.active.background = DDElements.Helpers.GenerateColorTexture(pressed);
            style.active.textColor = Color.white;
            
            style.focused.background = DDElements.Helpers.GenerateColorTexture(focused);
            style.focused.textColor = Color.white;
            return style;
        }

        public GUIStyle FlatColor(Color color)
        {
            GUIStyle style = new GUIStyle();

            style.normal.background = DDElements.Helpers.GenerateColorTexture(color);
            return style;
        }
        
        public GUIStyle FlatColor(Color color, Color textColor)
        {
            GUIStyle style = new GUIStyle();

            style.normal.textColor = textColor;
            style.normal.background = DDElements.Helpers.GenerateColorTexture(color);
            return style;
        }

        public GUIStyle DarkBackground()
        {
            return FlatColor(DDElements.Colors.DarkGray);
        }

        public GUIStyle EmptyStyle()
        {
            return new GUIStyle();
        }

        public GUIStyle FixedWidthHelpBox(int width)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.fixedWidth = width;
            return style;
        }
        
        public GUIStyle FixedHeightBox(int height)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.fixedHeight = height;
            return style;
        }
        
        public GUIStyle FixedSizeHelpBox(int width, int height)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.fixedHeight = height;
            style.fixedWidth = width;
            return style;
        }
    }
}