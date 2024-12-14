using DandyDino.Modulate;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    [CustomEditor(typeof(Game))]
    public class GameEditor : Editor
    {
        private Game _target;
        private string _companyName;
        private string _gameName;

        public int imageWidthOffset = 0;
        public float multiplier = 0.2f;

        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }
            _target = (Game)target;
            _companyName = _target.CompanyName;
            _gameName = _target.GameName;
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                return;
            }
            
            DDElements.Layout.Column(()=>
            {
                DrawBanner();
                DDElements.Layout.Space(5);
                CompanyNameSection();
                DDElements.Layout.Space(5);
                GameNameSection();
            });
        }

        private void GameNameSection()
        {
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Row(() =>
                {
                    DDElements.Rendering.Label("Game Name: ", GUILayout.Width(100));
                    DDElements.Rendering.TextField(ref _gameName, null, style: DDElements.Styles.TextFieldUnderline(), GUILayout.Height(25));
                    DDElements.Rendering.FlatColorButton("Apply".ToGUIContent(), DDElements.Colors.SoftGreen, () =>
                    {
                        _target.RenameGame(_gameName);
                    },  GUILayout.Width(100), GUILayout.Height(20));
                });
            });
        }

        private void DrawBanner()
        {
            DDElements.Layout.Column(() =>
            {
                if (_target.BannerTexture == null)
                {
                    DDElements.EditorUtils.ReserveSpace(out Rect reservedRect, DDElements.EditorUtils.GetWidth() - imageWidthOffset, (DDElements.EditorUtils.GetWidth() - imageWidthOffset) * multiplier);
                }
                else
                {
                    DDElements.Rendering.DrawBannerTexture(_target.BannerTexture, DDElements.EditorUtils.GetWidth()- imageWidthOffset, (DDElements.EditorUtils.GetWidth() - imageWidthOffset) * multiplier, ScaleMode.ScaleAndCrop);
                }
                
                DDElements.Layout.Space(-40);
                DDElements.Layout.Row(() =>
                {
                    DDElements.Layout.FlexibleSpace();
                    DDElements.Rendering.IconButton(DDElements.Icons.Picture("Change Texture"), 25, () =>
                    {
                        DDElements.EditorUtils.OpenAssetPicker<Texture>();
                    });
                    DDElements.Layout.Space(5);
                });
                
                DDElements.EditorUtils.GetPickedAsset<Texture>(texture =>
                {
                    _target.SetBannerTexture(texture);
                });
                
                DDElements.Layout.Space(15);
                DDElements.Rendering.Label("*Ideal texture size is 2048x410".ToGUIContent(), DDElements.Styles.Label(10));
            });
        }

        private void CompanyNameSection()
        {
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Row(() =>
                {
                    DDElements.Rendering.Label("Company Name: ", GUILayout.Width(100));
                    DDElements.Rendering.TextField(ref _companyName, null, style: DDElements.Styles.TextFieldUnderline(), GUILayout.Height(25));
                    DDElements.Rendering.FlatColorButton("Apply".ToGUIContent(), DDElements.Colors.SoftGreen, () =>
                    {
                        _target.RenameCompany(_companyName);
                    },GUILayout.Width(100), GUILayout.Height(20));
                });
            });
        }
    }
}
