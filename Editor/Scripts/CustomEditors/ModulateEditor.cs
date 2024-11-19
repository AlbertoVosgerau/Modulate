using UnityEditor;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(Modulate))]
    public class ModulateEditor : Editor
    {
        private Modulate _target;
        private Editor _servicesEditor;

        private void OnEnable()
        {
            Modulate.Init();
            _target = (Modulate)target;
            if (Modulate.Main != _target)
            {
                Selection.activeObject = Modulate.Main.gameObject;
                if (_target != null)
                {
                    DestroyImmediate(_target.gameObject);
                }
                return;
            }
            _servicesEditor = CreateEditor(_target.Services);
            _servicesEditor.CreateInspectorGUI();
        }
        
        public override void OnInspectorGUI()
        {
            _servicesEditor.OnInspectorGUI();
        }
    }
}