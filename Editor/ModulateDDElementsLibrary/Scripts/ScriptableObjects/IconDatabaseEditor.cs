using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(IconsDatabase))]
    public class IconDatabaseEditor : Editor
    {
        private IconsDatabase _target;

        private void OnEnable()
        {
            _target = (IconsDatabase)target;
        }

        public override void OnInspectorGUI()
        {
            DDElements.Rendering.Button("Refresh".ToGUIContent(), () =>
            {
                _target.Refresh();
            });
            
            base.OnInspectorGUI();
        }
    }
}