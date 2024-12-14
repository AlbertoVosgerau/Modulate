using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DandyDino.Modulate
{
    public class EditorUtils
    {
        public void CopyToClipboard(string str)
        {
            EditorGUIUtility.systemCopyBuffer = str;
        }
        
        public void CopyToClipboard(Object obj)
        {
            string dataPath = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                return;
            }
            
            EditorGUIUtility.systemCopyBuffer = dataPath;
        }
        
        public string GetClipboard()
        {
            return EditorGUIUtility.systemCopyBuffer;
        }

        public T PasteFromClipboard<T>() where T : Object
        {
            string clipboardStr = GetClipboard();
            T obj = AssetDatabase.LoadAssetAtPath<T>(clipboardStr);
            if (obj == null)
            {
                return null;
            }

            return obj;
        }
        
        public void RenameAction<T>(string asset, Action onRename) where T : Object
        {
            RenameAction(DDElements.Assets.LoadAssetAtPath<T>(asset), onRename);
        }

        public void RenameAction(Object asset, Action onRename)
        {
            string assetPath = DDElements.Assets.GetAssetPath(asset);
            RenameAssetUtility renameAssetUtility = ScriptableObject.CreateInstance<RenameAssetUtility>();
            renameAssetUtility.onRename = onRename;
            
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                asset.GetInstanceID(),
                renameAssetUtility,
                assetPath,
                AssetPreview.GetMiniThumbnail(asset), 
                null
            );
        }
        
        public GUILayoutOption[] InjectGUILayoutOption(ref GUILayoutOption[] options, GUILayoutOption optionToAdd)
        {
            if (options.Contains(optionToAdd))
            {
                return options;
            }

            List<GUILayoutOption> optionsList = new List<GUILayoutOption>(options);
            optionsList.Add(optionToAdd);
            options = optionsList.ToArray();
            return options;
        }
        
        
        #region Custom Inspector
        public Editor CreateCustomInspectorInstance<T>(T customInspector) where T : Object
        {
            Editor result = null;
            result = Editor.CreateEditor(customInspector);
            result.CreateInspectorGUI();
            return result;
        }
        public void DrawDefaultInspector(Editor editor)
        {
            editor.DrawDefaultInspector();
        }
        public void DrawEditorPreview(Editor editor, Rect rect)
        {
            editor.DrawPreview(rect);
        }

        /// <summary>
        /// Sometimes there's a gap between the label and an item that looks bad.
        /// This method safely changes and restores the EditorGUIUtility.labelWidth globally to set a fixed width to the label.
        /// </summary>
        public void SafeLabelWidthArea(int size, Action content)
        {
            EditorGUIUtility.labelWidth = size;
            content.Invoke();
            EditorGUIUtility.labelWidth = 0;

        }
        
        /// <summary>
        /// Sometimes there's a gap between the label and an item that looks bad.
        /// This method safely changes and restores the EditorGUIUtility.labelWidth globally to set a fixed width to the label.
        /// This overload doesn't set a prefixed value, but you can safely change EditorGUIUtility.labelWidth inside content().
        /// </summary>
        public void SafeLabelWidthArea(Action content)
        {
            content?.Invoke();
            EditorGUIUtility.labelWidth = 0;
        }

        public void SetLabelWidth(int width)
        {
            EditorGUIUtility.labelWidth = width;
        }

        /// <summary>
        /// Any content inside this method will be grayed out and locked for interaction.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="ignoreDisabledGroup"></param>
        public void DisabledGroupArea(Action content, bool ignoreDisabledGroup = false)
        {
            if (ignoreDisabledGroup)
            {
                content?.Invoke();
                return;
            }
            EditorGUI.BeginDisabledGroup(true); 
            content?.Invoke();
            EditorGUI.EndDisabledGroup();
        }
        #endregion
        
        public Scene GetTempScene(string name, Action onSceneLoad)
        {
            Scene scene = GetTempScene(name);
            onSceneLoad?.Invoke();
            return scene;
        }

        public Scene GetTempScene(string name)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = name;
            return scene;
        }
        
        public string GetSelectedPath()
        {
            Object selectedObject = Selection.activeObject;
            
            if (selectedObject != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                return assetPath;
            }

            return null;
        }

        public Object[] GetSelection()
        {
            return Selection.objects;
        }

        public void DisplayOkDialog(string message, Action onOk, string title = default)
        {
            bool isOk = EditorUtility.DisplayDialog(title, message, "Ok");
            if (isOk)
            {
                onOk?.Invoke();
            }
        }
        

        public Rect GetPopupWindowPosition(Vector2 windowSize)
        {
            Vector2 mousePosition = GUIUtility.GUIToScreenPoint(UnityEngine.Event.current.mousePosition);
            return new Rect(mousePosition.x - windowSize.x / 2, mousePosition.y - windowSize.y / 2, windowSize.x, windowSize.y);
        }

        public void PopupWindowCloseState(EditorWindow window)
        {
            if (UnityEngine.Event.current.keyCode == KeyCode.Escape)
            {
                window.Close();
            }
            
            if (EditorWindow.focusedWindow != window)
            {
                window.Close();
            }
        }

        public void DisplayYesNoDialog(string message, Action onYes, Action onNo, string title = default)
        {
            bool isOk = EditorUtility.DisplayDialog(title, message, "Yes", "No");
            if (isOk)
            {
                onYes?.Invoke();
                return;
            }
            
            onNo?.Invoke();
        }

        public void DisplayAcceptCancelDialog(string message,Action onAccept, Action onCancel, string title = default)
        {
            bool isOk = EditorUtility.DisplayDialog(title, message, "Accept", "Cancel");
            
            if (isOk)
            {
                onAccept?.Invoke();
                return;
            }
            
            onCancel?.Invoke();
        }

        public float GetWidth()
        {
            return EditorGUIUtility.currentViewWidth;
        }

        public void ReserveSpace(out Rect rect, float width, float height)
        {
            rect = GUILayoutUtility.GetRect(width, height);
        }

        public void OpenAssetPicker<T>() where T : Object
        {
            EditorGUIUtility.ShowObjectPicker<T>(null, false, "", 0);
        }
        
        public void GetPickedAsset<T>(out T obj) where T : Object
        {
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                obj = EditorGUIUtility.GetObjectPickerObject() as T;
                return;
            }
            
            obj = null;
        }
        
        public void GetPickedAsset<T>(Action<T> onAssetPicked) where T : Object
        {
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                T obj = EditorGUIUtility.GetObjectPickerObject() as T;
                if (obj != null)
                {
                    onAssetPicked?.Invoke(obj);
                }
            }
        }
    }
}