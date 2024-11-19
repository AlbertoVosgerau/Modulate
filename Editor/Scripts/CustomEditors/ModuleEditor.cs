using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(Module))]
    public class ModuleEditor : Editor
    {
        private Module _target;
        private Game _game;
        private string _moduleName;

        private void OnEnable()
        {
            _target = (Module)target;
            _game = GameInspector.GetGame();
            _moduleName = _target.ModuleName;
        }

        public override void OnInspectorGUI()
        {
            if (_target.ModuleName == "Main")
            {
                return;
            }
            
            DDElements.Rendering.DrawBannerTexture(_game.BannerTexture, DDElements.EditorUtils.GetWidth(), DDElements.EditorUtils.GetWidth() * 0.2f, ScaleMode.ScaleAndCrop);
            DDElements.Layout.Space(10);
            RenameOption();

        }
        
        private void RenameOption()
        {
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Row(() =>
                {
                    DDElements.Rendering.Label("Module Name: ");
                    DDElements.Rendering.TextField(ref _moduleName, null, style: DDElements.Styles.TextFieldUnderline(), GUILayout.Height(25));
                    DDElements.Rendering.FlatColorButton("Apply".ToGUIContent(), DDElements.Colors.SoftGreen, () =>
                    {
                        _target.RenameModule(_moduleName);
                    },GUILayout.Width(100), GUILayout.Height(20));
                });
            });
        }
    }
}
