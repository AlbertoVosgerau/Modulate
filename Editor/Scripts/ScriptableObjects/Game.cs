using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DandyDino.Modulate;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class Game : ScriptableObject
    {
        public string GameName => _gameName;
        [SerializeField] private string _gameName = "My Game";
        
        public string CompanyName => _companyName;
        [SerializeField] private string _companyName = "Dandy Dino";
        
        public string GamePath => DDElements.Assets.GetAssetPath(this);
        public string GameDirectory => Path.GetDirectoryName(GamePath);
        public string GameEditor => $"{System.IO.Path.GetDirectoryName(GamePath)}/Editor";
        public string GameModules => $"{System.IO.Path.GetDirectoryName(GamePath)}/Modules";
        public string GameMainModule => $"{System.IO.Path.GetDirectoryName(GamePath)}";

        public Texture BannerTexture => _bannerTexture == null? _bannerTexture = ModulateRoot.GetCoreRoot().Banner : _bannerTexture;
        [HideInInspector][SerializeField] private Texture _bannerTexture;

        public void SetGameName(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            _gameName = name;
            DDElements.Assets.SetDirtyAndSave(this);
            DDElements.Assets.Ping(this);
        }
        
        public void RenameGame(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            GameProjectUtils.RenameInWholeProject(_gameName, name);
            AssetDatabase.RenameAsset(GamePath, name);
            AssetDatabase.RenameAsset(GameDirectory, name);
            DDElements.Assets.SetDirtyAndSave(this);
            _gameName = name;
        }
        
        public void SetCompanyName(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            _companyName = name;
            DDElements.Assets.SetDirtyAndSave(this);
        }
        
        public void RenameCompany(string name)
        {
            name = name.Trim();
            name = name.Replace(" ", "");
            GameProjectUtils.RenameInWholeProject(_companyName, name);
            DDElements.Assets.SetDirtyAndSave(this);
            _companyName = name;
        }

        public void SetBannerTexture(Texture texture)
        {
            _bannerTexture = texture;
            DDElements.Assets.SetDirtyAndSave(this);
        }
    }
}