using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class CreateGameObjectsMenus
    {
        // Should we allow the creation of a GameObjet? So far we are keeping only the Editor Window
        //[MenuItem(CorePaths.GO_CREATE_CORE_GAMEOBJECT)]
        public static void CreateCore()
        {
            Modulate.Init();
            Modulate.Main.transform.SetSiblingIndex(0);
            Selection.activeObject = Modulate.Main;
        }

        [MenuItem(StringLibrary.GO_CREATE_MANAGERS_GAMEOBJECT)]
        public static void CreateManagersContainer()
        {
            List<ManagerContainer> containers = Object.FindObjectsByType<ManagerContainer>(FindObjectsSortMode.None).ToList();
            if (containers.Count > 0)
            {
                Debug.LogWarning($"Can't have more than one manager container.");
                return;
            }
            
            GameObject newObject = new GameObject(StringLibrary.MANAGER_CONTAINER_NAME);
            newObject.name = $"Managers";
            newObject.transform.SetSiblingIndex(0);
            if (Modulate.Main != null)
            {
                Modulate.Main.transform.SetSiblingIndex(0);
            }
            
            Selection.activeObject = newObject;
            newObject.AddComponent<ManagerContainer>();
        }
    }
}