using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Helpers
    {
        private Dictionary<Color, Texture> _storedTextures = new Dictionary<Color, Texture>();
    
        public Texture2D GenerateColorTexture(Color color, int width = 16, int height = 16)
        {
            if (_storedTextures.ContainsKey(color))
            {
                if (_storedTextures[color] != null)
                {
                    return (Texture2D)_storedTextures[color];
                }
                
                _storedTextures.Remove(color);
            }
            //

            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = color;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            _storedTextures.Add(color, result);
            return result;
        }
        
        public  Rect GetNextRect(float height)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            rect.height = height;
            return rect;
        }
        
        public Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }
    }
}