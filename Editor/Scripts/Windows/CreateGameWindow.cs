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
                CreateGame();
            });
            
            if (UnityEngine.Event.current.keyCode == KeyCode.Return)
            {
                CreateGame();
            }
        }

        // Assembly Definition was disabled as currently the project's philosophy is to contain global data on main module.
        // This might change in near future, hence the commented code.
        // Alberto, 17/11/2024. Hello to you from the future, btw!
        private void CreateGame()
        {
            string gameRoot = Path.Combine(_path, _gameName);
            string resourcesFolder = Path.Combine(gameRoot, "Resources");
            string modulesFolder = Path.Combine(gameRoot, "Modules");
            //string editorFolder = Path.Combine(gameRoot, "Editor");
            //string editorTexturesFolder = Path.Combine(editorFolder, "EditorTextures");
            string mainServiceFolder = Path.Combine(gameRoot, "MainModule");

            AssetDatabase.CreateFolder(_path, _gameName);
            AssetDatabase.CreateFolder(gameRoot, "Resources");
            AssetDatabase.CreateFolder(gameRoot, "MainModule");
            //AssetDatabase.CreateFolder(gameRoot, "Editor");
            //AssetDatabase.CreateFolder(editorFolder, "Scripts");
            //AssetDatabase.CreateFolder(editorFolder, "EditorTextures");
            AssetDatabase.CreateFolder(gameRoot, "Modules");

            //string assemblyDefinitionName = $"{_companyName}.{_gameName}";

            // List<string> assemblies = new List<string>();
            // assemblies.Add(StringLibrary.ASSEMBLY_DEFINITION);
            // assemblies.AddRange(_assembliesToAdd);

            //AssetCreationUtils.CreateAssemblyDefinition(gameRoot, assemblyDefinitionName, assemblyDefinitionName, references: assemblies.ToArray());
            //AssetCreationUtils.CreateAssemblyDefinition(editorFolder, $"{assemblyDefinitionName}.Editor", assemblyDefinitionName, references: new[] { StringLibrary.ASSEMBLY_DEFINITION_EDITOR, assemblyDefinitionName }, includePlatforms: new[] { "Editor" });

            AssetCreationUtils.CreateServicesCollection($"{resourcesFolder}/ServicesCollection.asset");
            Game asset = AssetCreationUtils.CreateGameRoot($"{gameRoot}/{_gameName}.asset");
            asset.SetCompanyName(_companyName);
            asset.SetGameName(_gameName);
            if (_texture != null)
            {
                asset.SetBannerTexture(_texture);
                AssetDatabase.Refresh();
            }

            ModuleCreator.CreateModule(mainServiceFolder, "Main", _assembliesToAdd);
            
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            DDElements.Assets.PingFolder(modulesFolder);
            _window.Close();
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