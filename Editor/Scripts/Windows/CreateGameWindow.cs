using System.Collections.Generic;
using System.IO;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class CreateGameWindow : EditorWindow
    {
        private CreateGameWindow _window;

        private string _path;
        private string _companyName = "";
        private string _gameName = "";
        
        private Texture _texture;

        private Vector2 _includedAssembliesScroll;
        private Vector2 _availableAssembliesScroll;
        private List<string> _assemblies;
        private List<string> _assembliesToAdd = new List<string>();
        private string _assemblyQuery;

        public void Init(string path)
        {
            _path = path;
            _texture = ModulateRoot.GetCoreRoot().Banner;
        }

        private void OnEnable()
        {
            _window = GetWindow<CreateGameWindow>();
            _window.minSize = new Vector2(900, 400);
            _assemblies = GameInspector.GetAvailableAssemblyDefinitionNames();
        }

        private void OnGUI()
        {
            DDElements.Layout.DrawBackground(new Rect(new Vector2(0,0), new Vector2(position.width, position.height)), DDElements.Colors.BluishGray);
            DrawBanner();
            DDElements.Layout.Space(10);
            
            DDElements.Layout.Row(() =>
            {
                DDElements.Rendering.Icon(DDElements.Icons.Joystick());
                GUILayout.Label("Game Name: ", GUILayout.Width(90));
                _gameName = EditorGUILayout.TextField(_gameName);
                
                DDElements.Rendering.Icon(DDElements.Icons.Enterprise());
                GUILayout.Label("Company Name: ", GUILayout.Width(100));
                _companyName = EditorGUILayout.TextField(_companyName);
            });
            
            if (!string.IsNullOrWhiteSpace(_gameName))
            {
                _gameName = _gameName.Replace(" ", "");
            }
            
            if (!string.IsNullOrWhiteSpace(_companyName))
            {
                _companyName = _companyName.Replace(" ", "");
            }

            bool isGameValid = !string.IsNullOrWhiteSpace(_gameName) && AssetCreationUtils.IsValidFolderName(_gameName);
            

            if (!isGameValid)
            {
                EditorGUILayout.HelpBox("Invalid Game Name", MessageType.Warning);
                return;
            }
            
            bool isCompanyNameValid = !string.IsNullOrWhiteSpace(_companyName) && AssetCreationUtils.IsValidFolderName(_companyName);
            if (!isCompanyNameValid)
            {
                EditorGUILayout.HelpBox("Invalid Company Name", MessageType.Warning);
                return;
            }
            DDElements.Rendering.Line();

            DDElements.Layout.Space(10);
            SharedWindowsElements.DrawAssembliesList(ref _assemblyQuery, ref _availableAssembliesScroll, ref _includedAssembliesScroll, _assemblies, _assembliesToAdd);

            DDElements.Layout.Space(10);
            DDElements.Rendering.FlatColorButton("Create".ToGUIContent(), DDElements.Colors.SoftGreen,() =>
            {
                GameCreator.CreateGame(_path, _gameName, _companyName, _texture, _assembliesToAdd, ()=> _window.Close());
            });
            
            if (UnityEngine.Event.current.keyCode == KeyCode.Return)
            {
                GameCreator.CreateGame(_path, _gameName, _companyName, _texture, _assembliesToAdd, ()=> _window.Close());
            }
        }
        
        private void DrawBanner()
        {
            DDElements.Layout.Column(() =>
            {
                if (_texture == null)
                {
                    DDElements.EditorUtils.ReserveSpace(out Rect reservedRect, DDElements.EditorUtils.GetWidth() , (DDElements.EditorUtils.GetWidth()) * 0.2f);
                }
                else
                {
                    DDElements.Rendering.DrawBannerTexture(_texture, DDElements.EditorUtils.GetWidth(), DDElements.EditorUtils.GetWidth() * 0.2f, ScaleMode.ScaleAndCrop);
                }
                
                DDElements.Layout.Space(-40);
                DDElements.Layout.Row(() =>
                {
                    DDElements.Layout.FlexibleSpace();
                    DDElements.Rendering.IconButton(DDElements.Icons.Picture("Change Game Banner"), 25, () =>
                    {
                        DDElements.EditorUtils.OpenAssetPicker<Texture>();
                    });
                    DDElements.Layout.Space(5);
                });
                
                DDElements.EditorUtils.GetPickedAsset<Texture>(texture =>
                {
                    _texture = texture;
                });
                
                DDElements.Layout.Space(15);
                DDElements.Rendering.Label("*Ideal texture size is 2048x410".ToGUIContent(), DDElements.Styles.Label(10));
            });
        }
    }
}