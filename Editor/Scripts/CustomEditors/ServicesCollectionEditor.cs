using System;
using System.Collections.Generic;
using System.Linq;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(ServicesCollection))]
    public class ServicesCollectionEditor : Editor
    {
        private ServicesCollection _target;
        private List<Type> _classes;
        private SerializedProperty _gameServices;
        private List<bool> _drawContent = new List<bool>();
        private Game _game;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }
            _target = (ServicesCollection)target;
            
            CleanupTarget();
            _classes = ReflectionUtility.GetAllClassesOfType<GameService>();
            _drawContent = Enumerable.Repeat(false, _target.gameServices.Count).ToList();
            _game = GameInspector.GetGame();
        }

        private void CleanupTarget()
        {
            for (int i = _target.gameServices.Count - 1; i >= 0; i--)
            {
                if (_target.gameServices[i] == null)
                {
                    _target.gameServices.RemoveAt(i);
                    DDElements.Assets.SetDirtyAndSave(_target);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                return;
            }
            if (_game == null)
            {
                return;
            }
            _gameServices = serializedObject.FindProperty(nameof(_target.gameServices));
            
            GUIStyle editorStyle = new GUIStyle();
            editorStyle.normal.background = DDElements.Helpers.GenerateColorTexture(DDElements.Colors.MidDarkGray);
            
            DDElements.Layout.Column(() =>
            {
                DDElements.Rendering.DrawBannerTexture(_game.BannerTexture, DDElements.EditorUtils.GetWidth(), DDElements.EditorUtils.GetWidth() * 0.2f, ScaleMode.ScaleAndCrop);
                DDElements.Layout.Space();
                DrawEditor();
                DDElements.Layout.FlexibleSpace();
            }, style: editorStyle);
        }

        private void DrawEditor()
        {
            DrawItemHeader();
            if (_target.gameServices.Count != _drawContent.Count)
            {
                _drawContent = Enumerable.Repeat(false, _target.gameServices.Count).ToList();
            }
            DrawServices(serializedObject, _gameServices, _target.gameServices);
        }

        private void DrawItemHeader()
        {
            DDElements.ReflectionUtilities.AddClassInstanceBar(serializedObject, OnAddItem, "Game Services", "Game Service", _classes, _target.gameServices, DDElements.Colors.SoftGreen);
        }
        
        private void OnAddItem(IService manager)
        {
            DDElements.Assets.SetDirtyAndSave(_target);
        }
        
        private void DrawToggle(IService service)
        {
            DDElements.Rendering.Switch(service.IsEnabled, value =>
            {
                service.SetEnabled(value);
            });
        }

        private void DrawServices(SerializedObject so, SerializedProperty property, List<IService> itemsList)
        {
            ManagerContainerRenderingUtility.DrawSerializedObjectsItemsList(_drawContent, so, property, itemsList, DrawToggle);
        }
    }
}