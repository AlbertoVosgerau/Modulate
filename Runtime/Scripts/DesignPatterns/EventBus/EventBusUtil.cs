using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DandyDino.Modulate
{
    public static class EventBusUtils
    {
        private static readonly List<Action> _clearActions = new();

        public static void RegisterBus(Action clearAction) => _clearActions.Add(clearAction);

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                foreach (var clear in _clearActions)
                {
                    clear();
                }
            }
        }
#endif
    }
}