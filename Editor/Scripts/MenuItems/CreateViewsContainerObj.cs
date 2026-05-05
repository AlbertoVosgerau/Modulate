using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DandyDino.Modulate
{
    public class CreateViewsContainerObj
    {
        [MenuItem(StringLibrary.GO_CREATE_VIEWS_CONTAINER_GAMEOBJECT)]
        public static void CreateManagersContainer(MenuCommand menuCommand)
        {
            Scene targetScene;
            if (menuCommand.context is GameObject contextGo)
            {
                targetScene = contextGo.scene;
            }
            else
            {
                targetScene = SceneManager.GetActiveScene();
            }

            if (!targetScene.IsValid() || !targetScene.isLoaded)
            {
                Debug.LogWarning("Target scene is not valid or not loaded.");
                return;
            }


            if (SceneHasViewsContainer(targetScene))
            {
                Debug.LogWarning($"Scene '{targetScene.name}' already has a Views Container.");
                return;
            }

            Scene previousActive = SceneManager.GetActiveScene();
            bool changedActive = previousActive != targetScene;

            if (changedActive)
            {
                SceneManager.SetActiveScene(targetScene);
            }

            GameObject newObject = new GameObject(StringLibrary.VIEWS_CONTAINER_NAME);


            Undo.RegisterCreatedObjectUndo(newObject, "Create Views Container");

            newObject.transform.SetSiblingIndex(0);
            
            if (Modulate.Main != null && Modulate.Main.gameObject.scene == targetScene)
            {
                Modulate.Main.transform.SetSiblingIndex(0);
            }

            newObject.AddComponent<ModulateViewsContainer>();
            
            if (changedActive)
            {
                SceneManager.SetActiveScene(previousActive);
            }
            
            EditorSceneManager.MarkSceneDirty(targetScene);
            Selection.activeObject = newObject;
        }

        private static bool SceneHasViewsContainer(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.GetComponentInChildren<ModulateViewsContainer>(true) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}