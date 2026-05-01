using System;
using System.Collections.Generic;
using System.Linq;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(ModulateViewsContainer))]
    public class ModulateViewsContainerEditor : Editor
    {
        private ModulateViewsContainer _target;
        private List<IView> _views;
        private List<Type> _classTypes;
        private List<Editor> _viewEditors = new List<Editor>();

        private void OnEnable()
        {
            _target = (ModulateViewsContainer)target;
            _views = _target.GetComponents<IView>().ToList();
            
            _classTypes = ReflectionUtility.GetAllClassesOfType<IView>();
            
            foreach (IView view in _views)
            {
                if (view is Object unityObject)
                {
                    unityObject.hideFlags = HideFlags.HideInInspector;
                    _viewEditors.Add(CreateEditor(unityObject));
                }
            }
        }
        
        private void OnDisable()
        {
            foreach (Editor editor in _viewEditors)
            {
                if (editor != null) DestroyImmediate(editor);
            }
            _viewEditors.Clear();
        }

        public override void OnInspectorGUI()
        {
            DDElements.ReflectionUtilities.AddClassInstanceBar<IView>(
                so: serializedObject,
                onAdd: AddView,
                groupLabel: "Views",
                itemLabel: "View",
                types: _classTypes,
                target: _views,
                buttonColor: DDElements.Colors.BluishGray // or whatever color you prefer
            );

            foreach (Editor editor in _viewEditors)
            {
                if (editor == null) continue;
                editor.OnInspectorGUI();
                EditorGUILayout.Space();
            }
        }
        
        private void AddView(IView newView)
        {
            // Refresh the views list and rebuild editors
            _views = _target.GetComponents<IView>().ToList();
    
            foreach (Editor editor in _viewEditors)
            {
                if (editor != null) DestroyImmediate(editor);
            }
            _viewEditors.Clear();

            foreach (IView view in _views)
            {
                if (view is Object unityObject)
                {
                    unityObject.hideFlags = HideFlags.HideInInspector;
                    _viewEditors.Add(CreateEditor(unityObject));
                }
            }
        }

    }
}