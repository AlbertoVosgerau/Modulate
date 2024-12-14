using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ReflectionUtilities
    {
        public void AddClassInstanceBar<T>(SerializedObject so, Action<T> onAdd, string groupLabel,string itemLabel, List<Type> types, List<T> target, Color buttonColor) where T : class
        {
            GenericMenu menuHeader = new GenericMenu();
            foreach (Type item in types)
            {
                string label = item.ToString().Split('.').Last();
                
                if (target.Any(x => x != null && x.GetType() == item))
                {
                    menuHeader.AddDisabledItem(new GUIContent(label), false);
                }
                else
                {
                    Undo.RecordObject(so.targetObject, $"Add {itemLabel}");
                    menuHeader.AddItem(new GUIContent(label), false, () => AddItemToClassList<T>(so, onAdd, types, target, item));
                }
            }
            
            DDElements.Layout.Row(()=>
            {
                DDElements.Layout.Column(() =>
                {
                    DDElements.Layout.Space(5);
                    DDElements.Layout.Row(() =>
                    {
                        DDElements.Rendering.LabelField(groupLabel.ToGUIContent(), EditorStyles.largeLabel);
                        DDElements.Rendering.FlatColorButton($"Add {itemLabel}".ToGUIContent(), buttonColor,  () =>
                        {
                            menuHeader.ShowAsContext();
                        });
                    });
                    DDElements.Layout.Space(5);
                });
                
            }, style: DDElements.Styles.FlatColor(DDElements.Colors.MidDarkGray));
        }

        private void AddItemToClassList<T>(SerializedObject serializedObject, Action<T> onAdd, List<Type> src, List<T> target, Type type) where T : class
        {
            if (src.Any(x => x != null && x.GetType() == type))
            {
                return;
            }

            T newItem = InstantiateClass<T>(type);
            
            if (!target.Contains(newItem))
            {
                target.Add(newItem);
                onAdd?.Invoke(newItem);
            }
            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        public T InstantiateClass<T>(Type type) where T : class
        {
            object newObj = Activator.CreateInstance(type);
            return newObj as T;
        }
        
        public List<Type> GetAllConcreteImplementations<T>()
        {
            Type baseType = typeof(T);
            List<Type> implementations = new List<Type>();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    implementations.AddRange(
                        assembly.GetTypes()
                            .Where(type => type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                    );
                }
                catch (ReflectionTypeLoadException ex)
                {
                    foreach (Type type in ex.Types)
                    {
                        if (type != null && type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                        {
                            implementations.Add(type);
                        }
                    }
                }
            }

            return implementations;
        }
    }
}