using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class IconPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool shouldRefresh = false;
            IconsDatabase iconsDatabase = null;
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                string importedAsset = importedAssets[i];
                string assetDirectory = DDElements.Assets.GetAssetDirectory(importedAsset);
                if (!importedAsset.EndsWith(".png"))
                {
                    return;
                }

                iconsDatabase = DDElements.Assets.LoadAssetAtDirectory<IconsDatabase>(assetDirectory);
                if (iconsDatabase == null)
                {
                    return;
                }
                
                TextureImporter textureImporter = AssetImporter.GetAtPath(importedAsset) as TextureImporter;
                if (textureImporter != null && textureImporter.textureType != TextureImporterType.Sprite)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.maxTextureSize = 32;
                    textureImporter.SaveAndReimport();
                }

                shouldRefresh = true;
            }

            if (!shouldRefresh)
            {
                for (int i = 0; i < deletedAssets.Length; i++)
                {
                    string importedAsset = deletedAssets[i];
                    string directory = DDElements.Assets.GetAssetDirectory(importedAsset);
                
                    iconsDatabase = DDElements.Assets.GetAssetsInDirectory<IconsDatabase>(directory).FirstOrDefault();
                    shouldRefresh = iconsDatabase != null;
                }
            }

            if (iconsDatabase == null || !shouldRefresh)
            {
                return;
            }

            iconsDatabase.Refresh();
        }
    }
}