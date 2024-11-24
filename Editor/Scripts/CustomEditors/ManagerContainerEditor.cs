using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(ManagerContainer))]
    public class ManagerContainerEditor : Editor
    {
        private ManagerContainer _target;
        private List<Type> _classes;
        private SerializedProperty _managers;
        private List<bool> _drawContent = new List<bool>();
        private Game _game;

        private void OnEnable()
        {
            _target = (ManagerContainer)target;
            CleanupTarget();
            _classes = ReflectionUtility.GetAllClassesOfType<IManager>();
            _drawContent.Clear();
            _drawContent = Enumerable.Repeat(false, _classes.Count).ToList();
            _game = GameInspector.GetGame();
        }
        
        private void CleanupTarget()
        {
            for (int i = _target.Managers.Count - 1; i >= 0; i--)
            {
                if (_target.Managers[i] == null)
                {
                    _target.Managers.RemoveAt(i);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            _managers = serializedObject.FindProperty(nameof(_target.Managers));

            GUIStyle editorStyle = new GUIStyle();
            editorStyle.normal.background = DDElements.Helpers.GenerateColorTexture(DDElements.Colors.MidLightGray);
            
            DDElements.Layout.Column(() =>
            {
                DDElements.Rendering.DrawBannerTexture(_game.BannerTexture, DDElements.EditorUtils.GetWidth(), DDElements.EditorUtils.GetWidth() * 0.2f, ScaleMode.ScaleAndCrop);
                DDElements.Layout.Space(5);
                DrawEditor();
            }, style: editorStyle);
            
            Repaint();
        }

        private void DrawEditor()
        {
            serializedObject.Update();
            DrawItemHeader();
            DrawServices(serializedObject, _managers, _target.Managers);
        }

        private void DrawItemHeader()
        {
            DDElements.ReflectionUtilities.AddClassInstanceBar<IManager>(serializedObject, null,$"Managers", "Manager", _classes, _target.Managers, DDElements.Colors.MidOrange);
        }
        
        private void DrawToggle(IManager manager)
        {
            DDElements.Rendering.Switch(manager.IsEnabled, value =>
            {
                manager.SetEnabled(value);
            });
        }

        private void DrawServices(SerializedObject so, SerializedProperty property, List<IManager> itemsList)
        {
            ManagerContainerRenderingUtility.DrawSerializedObjectsItemsList<IManager>(_drawContent, so, property, itemsList, DrawToggle);
        }
    }
}