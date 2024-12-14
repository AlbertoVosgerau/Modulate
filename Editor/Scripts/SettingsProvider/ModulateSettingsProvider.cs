using DandyDino.Modulate;
using UnityEditor;

namespace DandyDino.Modulate
{
    public class ModulateSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
        {
            if (!GameInspector.GameRootExists())
            {
                return null;
            }
            
            Game game = GameInspector.GetExistingGameRoot();
            GameEditor gameEditor = (GameEditor)Editor.CreateEditor(game);
            gameEditor.imageWidthOffset = 600;
            gameEditor.multiplier = 0.3f;
            SettingsProvider provider = new SettingsProvider($"Project/{game.GameName} Settings", SettingsScope.Project)
            {
                label = $"{game.GameName} Settings",
                guiHandler = (searchContext) =>
                {
                    DDElements.Layout.Column(() =>
                    {
                        DDElements.Layout.Space(10);
                        gameEditor.OnInspectorGUI();
                    });
                },
                
                keywords = new[] { "My Game", "Settings", "Core" }
            };

            return provider;
        }
    }
}