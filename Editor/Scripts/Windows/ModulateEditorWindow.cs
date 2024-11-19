using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class ModulateEditorWindow : EditorWindow
    {
        private static Editor _servicesEditor;
        [MenuItem(StringLibrary.MODULATE_WINDOW)]
        public static void Init()
        {
            ModulateEditorWindow window = GetWindow<ModulateEditorWindow>();
            window.titleContent = new GUIContent("Game Services");
            
            RefreshAsset();
        }

        private void OnFocus()
        {
            RefreshAsset();
        }

        private static void RefreshAsset()
        {
            ServicesCollection services = GameInspector.GetServicesCollection();
            _servicesEditor = Editor.CreateEditor(services);
            _servicesEditor.CreateInspectorGUI(); 
        }

        private void OnGUI()
        {
            DDElements.Layout.Column(() =>
            {
                _servicesEditor.OnInspectorGUI();
            });
        }
    }
}