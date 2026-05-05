using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class FeatureToggleAsset : ScriptableObject
    {
        private List<IManager> _managers;

        // Persisted: every type we've ever seen, and the subset the user has explicitly disabled.
        [SerializeField] private List<string> _knownManagers = new();
        [SerializeField] private List<string> _disabledManagers = new();

        public List<IManager> Managers => _managers;

        public bool IsEnabled(Type type) => !_disabledManagers.Contains(type.FullName);

        public void SetEnabled(Type type, bool enabled)
        {
            string name = type.FullName;
            if (enabled)
            {
                _disabledManagers.Remove(name);
            }
            else
            {
                if (!_disabledManagers.Contains(name)) _disabledManagers.Add(name);
            }
        }

        public List<IManager> Refresh()
        {
            List<Type> managerTypes = ReflectionUtility.GetAllManagerTypes().ToList();

            _managers ??= new List<IManager>();
            _knownManagers ??= new List<string>();
            _disabledManagers ??= new List<string>();

            HashSet<string> validNames = new HashSet<string>(managerTypes.Select(t => t.FullName));

            // Drop stale runtime instances and stale persisted entries.
            _managers.RemoveAll(m => m == null || !validNames.Contains(m.GetType().FullName));
            _knownManagers.RemoveAll(n => !validNames.Contains(n));
            _disabledManagers.RemoveAll(n => !validNames.Contains(n));

            foreach (Type type in managerTypes)
            {
                string name = type.FullName;

                // Recreate runtime instance if missing (e.g. after domain reload).
                bool hasInstance = _managers.Exists(m => m != null && m.GetType() == type);
                if (!hasInstance)
                {
                    _managers.Add((IManager)Activator.CreateInstance(type));
                }

                // First time we've ever seen this type → record it. Defaults to enabled
                // because we don't add it to _disabledManagers.
                if (!_knownManagers.Contains(name))
                {
                    _knownManagers.Add(name);
                    Debug.Log($"[Modulate] Discovered new manager: {type.Name}");
                }
            }

            return _managers;
        }
    }
}