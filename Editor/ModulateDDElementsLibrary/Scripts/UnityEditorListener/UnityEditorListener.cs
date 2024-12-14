using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

namespace DandyDino.Modulate
{
    [InitializeOnLoad]
    public class UnityEditorListener
    {
        public static List<Action> onEditorUpdate = new List<Action>();
        public static List<Action> onHierarchyChanged = new List<Action>();
        public static List<Action> onProjectChanged = new List<Action>();
        public static List<Action> onSelectionChanged = new List<Action>();
        public static List<Action<SceneView>> onSceneGUI = new List<Action<SceneView>>();
        
        static UnityEditorListener()
        {
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.projectChanged += OnProjectChanged;
            Selection.selectionChanged += OnSelectionChanged;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (onSceneGUI.Count == 0)
            {
                return;
            }

            for (int i = 0; i < onSceneGUI.Count; i++)
            {
                onSceneGUI[i]?.Invoke(sceneView);
            }
        }

        private static void OnEditorUpdate()
        {
            if (onEditorUpdate.Count == 0)
            {
                return;
            }

            for (int i = 0; i < onEditorUpdate.Count; i++)
            {
                onEditorUpdate[i]?.Invoke();
            }
        }

        private static void OnHierarchyChanged()
        {
            if (onHierarchyChanged.Count == 0)
            {
                return;
            }
            
            for (int i = 0; i < onHierarchyChanged.Count; i++)
            {
                onHierarchyChanged[i]?.Invoke();
            }
        }

        private static void OnProjectChanged()
        {
            if (onProjectChanged.Count == 0)
            {
                return;
            }
            
            for (int i = 0; i < onProjectChanged.Count; i++)
            {
                onProjectChanged[i]?.Invoke();
            }
        }

        private static void OnSelectionChanged()
        {
            if (onSelectionChanged.Count == 0)
            {
                return;
            }
            
            for (int i = 0; i < onSelectionChanged.Count; i++)
            {
                onSelectionChanged[i]?.Invoke();
            }
        }
    }
}
