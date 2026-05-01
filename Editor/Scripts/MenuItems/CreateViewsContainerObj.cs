using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class CreateViewsContainerObj
    {
        [MenuItem(StringLibrary.GO_CREATE_VIEWS_CONTAINER_GAMEOBJECT)]
        public static void CreateManagersContainer()
        {
            ModulateViewsContainer existing = Object.FindAnyObjectByType<ModulateViewsContainer>();

            if (existing != null)
            {
                Debug.LogWarning($"Can't have more than one Views Container");
                return;
            }
            
            GameObject newObject = new GameObject(StringLibrary.VIEWS_CONTAINER_NAME);
            newObject.name = StringLibrary.VIEWS_CONTAINER_NAME;
            newObject.transform.SetSiblingIndex(0);
            if (Modulate.Main != null)
            {
                Modulate.Main.transform.SetSiblingIndex(0);
            }
            
            Selection.activeObject = newObject;
            newObject.AddComponent<ModulateViewsContainer>();
        }
    }
}
