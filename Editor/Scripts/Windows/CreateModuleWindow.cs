using System.Collections.Generic;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class CreateModuleWindow : EditorWindow
    {
        private CreateModuleWindow _window;
        private string _path;
        private string _moduleName;
        private Game _game;
        private ModulateRoot _modulate;
        private bool _hasFocused = false;
        
        private Vector2 _includedAssembliesScroll;
        private Vector2 _availableAssembliesScroll;
        private List<string> _assemblies;
        private List<string> _assembliesToAdd = new List<string>();
        private string _assemblyQuery;

        public void Init(string path)
        {
            if (!path.Contains("Modules"))
            {
                path = GameInspector.GetModulesPath();
            }
            _path = path;
        }

        private void OnEnable()
        {
            _window = GetWindow<CreateModuleWindow>();
            _window.minSize = new Vector2(900, 400);
            _assemblies = GameInspector.GetAvailableAssemblyDefinitionNames();
            _game = GameInspector.GetGame();
            _modulate = ModulateRoot.GetCoreRoot();
        }

        private void OnGUI()
        {
            DDElements.Layout.DrawBackground(new Rect(new Vector2(0,0), new Vector2(position.width, position.height)), DDElements.Colors.BluishGray);
            
            DDElements.Rendering.DrawBannerTexture(_modulate.Modules, DDElements.EditorUtils.GetWidth(), DDElements.EditorUtils.GetWidth() * 0.1f, ScaleMode.ScaleAndCrop);
            DDElements.Layout.Space(8);
            
            GUI.SetNextControlName("InputField");
            DDElements.Rendering.TextField(ref _moduleName, str =>
            {
                        
            }, style: DDElements.Styles.TextFieldUnderline(), options: GUILayout.Height(25));
                        
            if (!_hasFocused)
            {
                GUI.FocusControl("InputField");
                _hasFocused = true; 
            }
            
            bool isEmpty = string.IsNullOrWhiteSpace(_moduleName);
            if (!isEmpty)
            {
                _moduleName = _moduleName.Replace(" ", "");
            }

            bool isValid =  !isEmpty && AssetCreationUtils.IsValidFolderName(_moduleName);
            
            if (!isValid)
            {
                EditorGUILayout.HelpBox("Invalid module name", MessageType.Warning);
                return;
            }
            
            SharedWindowsElements.DrawAssembliesList(ref _assemblyQuery, ref _availableAssembliesScroll, ref _includedAssembliesScroll, _assemblies, _assembliesToAdd);
            DDElements.Layout.Space(10);
            DDElements.Rendering.FlatColorButton("Create".ToGUIContent(), DDElements.Colors.MidOrange,  () =>
            {
                ProceedToModuleCreation();
            });

            if (UnityEngine.Event.current.keyCode == KeyCode.Return)
            {
                ProceedToModuleCreation();
            }
        }

        private void ProceedToModuleCreation()
        {
            _window.Close();
            string modulePath = ModuleCreator.CreateModule(_path, _moduleName, _assembliesToAdd);
            EditorUtility.FocusProjectWindow();
            DDElements.Assets.Ping(modulePath);
        }
    }
}