using DandyDino.Elements;
using UnityEditor;

namespace DandyDino.Modulate
{
    public static class CreateGameMenus
    {
        [MenuItem(StringLibrary.CREATE_GAME, false, 0)]
        public static void OpenCreateModuleWindow()
        {
            CreateGameWindow window = EditorWindow.GetWindow<CreateGameWindow>();
            window.Init(DDElements.Assets.GetActiveFolderPath());
        }
    }
}
