using System;
using System.Collections.Generic;
using System.IO;
using DandyDino.Elements;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class GameCreator
    {
        public static void CreateGame(string gamePath, string gameName, string companyName, Texture gameBanner, List<string> assembliesToAdd, Action onCreateGame)
        {
            string gameRoot = Path.Combine(gamePath, gameName);
            string modulesFolder = Path.Combine(gameRoot, "Modules");

            AssetDatabase.CreateFolder(gamePath, gameName);
            AssetDatabase.CreateFolder(gameRoot, "Modules");
            
            Game asset = AssetCreationUtils.CreateGameRoot($"{gameRoot}/{gameName}.asset");
            asset.SetCompanyName(companyName);
            asset.SetGameName(gameName);
            if (gameBanner != null)
            {
                asset.SetBannerTexture(gameBanner);
                AssetDatabase.Refresh();
            }

            ModuleCreator.CreateModule(gameRoot, "Main", assembliesToAdd);
            
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            DDElements.Assets.PingFolder(modulesFolder);
            onCreateGame?.Invoke();
        }
    }
}
