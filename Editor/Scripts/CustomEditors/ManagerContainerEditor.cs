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
        private ServicesCollection _collection;

        private void OnEnable()
        {
            _target = (ManagerContainer)target;
            CleanupTarget();
            _classes = ReflectionUtility.GetAllClassesOfType<Manager<GameService>>();
            _drawContent.Clear();
            _drawContent = Enumerable.Repeat(false, _classes.Count).ToList();
            _game = GameInspector.GetGame();
            _collection = GameInspector.GetServicesCollection();
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
            
            for (int i = 0; i < _managers.arraySize; i++)
            {
                SerializedProperty soManager = _managers.GetArrayElementAtIndex(i);
                Type serviceType = ReflectionUtility.GetManagerGenericType(soManager.managedReferenceValue.GetType());
                GameService service = _collection.gameServices.FirstOrDefault(x => x.GetType() == serviceType);
                if (!service.IsEnabled)
                {
                    ((IManager)soManager.managedReferenceValue).SetEnabled(false);
                }
            }

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
            DDElements.ReflectionUtilities.AddClassInstanceBar<IManager>(serializedObject, OnAddItem,$"Managers", "Manager", _classes, _target.Managers, DDElements.Colors.MidOrange);
        }

        

        private void OnAddItem(IManager manager)
        {
            ServicesCollection servicesCollection = GameInspector.GetServicesCollection();
            List<Type> classes = DDElements.ReflectionUtilities.GetAllConcreteImplementations<GameService>().Where(type => type.IsClass && !type.IsAbstract && typeof(GameService).IsAssignableFrom(type)).ToList();

            string managerName = manager.GetType().Name;
            string managerSimpleName = Regex.Replace(managerName, @"Manager$", "");
            Type type = classes.FirstOrDefault(x => x.Name.ToLower().Contains(managerSimpleName.ToLower()));
            GameService instanceType = DDElements.ReflectionUtilities.InstantiateClass<GameService>(type);
            
            bool servicesCollectionContainsType = servicesCollection.gameServices.FirstOrDefault(x => x.GetType() == type) != null;
            if (servicesCollectionContainsType)
            {
                return;
            }
            
            DDElements.EditorUtils.DisplayYesNoDialog("You are adding a Manager not present in Core Services. Do you want to add it to your Project Core?",
                () =>
                {
                    servicesCollection.AddService(instanceType);
                    DDElements.Assets.SetDirtyAndSave(servicesCollection);
                }, null);
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