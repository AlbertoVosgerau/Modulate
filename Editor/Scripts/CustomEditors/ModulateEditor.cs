using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(Modulate))]
    public class ModulateEditor : Editor
    {
        private Modulate _target;
        private Editor _servicesEditor;

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                _target = (Modulate)target;
                return;
            }
            
            _target = (Modulate)target;
            Debug.LogError($"This component cannot be added in Editor. Please run the game, it will be automatically generated");
            DestroyImmediate(_target);
        }
    }
}