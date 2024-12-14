using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DandyDino.Modulate
{
    public static class StringExtension
    {
        public static GUIContent ToGUIContent(this string str)
        {
            return new GUIContent(str);
        }
        
        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            
            string[] words = str.Split(new[] { '-', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }
            
            return string.Join("", words);
        }
    }
}