using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(ModulateBehaviour), true)]
    public class ModulateBehaviourEditor : Editor
    {
        private ModulateBehaviour _target;

        private void OnEnable()
        {
            _target = (ModulateBehaviour)target;
            ModulateViewsContainer viewsContainer = _target.gameObject.GetComponent<ModulateViewsContainer>();

            if (viewsContainer == null)
            {
                Debug.LogWarning($"Can't create an IView Component outside ViewsContainer. Please Right click on hierarchy, go to Modulate! -> Create Views Container and add your View.");
                DestroyImmediate(_target);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}