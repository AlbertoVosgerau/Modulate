using DandyDino.Modulate;
using UnityEditor;

namespace DandyDino.Modulate
{
    public static class CreateModuleMenus
    {
        [MenuItem(StringLibrary.CREATE_MODULE, false, 1)]
        public static void OpenCreateModuleWindow()
        {
            CreateModuleWindow window = EditorWindow.GetWindow<CreateModuleWindow>();
            window.Init(DDElements.Assets.GetActiveFolderPath());
        }
    }
}