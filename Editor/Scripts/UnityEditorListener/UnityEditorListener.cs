using UnityEditor;

namespace DandyDino.Modulate
{
    [InitializeOnLoad]
    public class UnityEditorListener
    {
        static UnityEditorListener()
        {
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.projectChanged += OnProjectChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnEditorUpdate()
        {
        }

        private static void OnHierarchyChanged()
        {
        }

        private static void OnProjectChanged()
        {
        }

        private static void OnSelectionChanged()
        {
        }
    }
}
