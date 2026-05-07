using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DandyDino.Modulate.Bootstrap
{
    [InitializeOnLoad]
    internal static class ModulateDependencyInstaller
    {
        private const string SessionKey = "DandyDino.Modulate.DepsChecked";
        private const string LogPrefix = "[Modulate Installer]";
        private const string PendingNuGetWindowKey = "DandyDino.Modulate.PendingNuGetWindow";

        private static readonly Dictionary<string, string> RequiredDependencies =
            new Dictionary<string, string>
            {
                { "com.dandydino.elements", "https://github.com/AlbertoVosgerau/DDElements.git#0.2.0" },
                { "com.cysharp.r3",         "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity#1.3.0" },
                { "com.cysharp.unitask",    "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask#2.5.10" },
                { "com.gustavopsantos.reflex", "14.3.0" },
                {"com.github-glitchenzo.nugetforunity", "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity#v4.4.0"}
            };
        
        private const string OpenUpmName = "OpenUPM";
        private const string OpenUpmUrl  = "https://package.openupm.com";
        private static readonly string[] OpenUpmScopes =
        {
            "com.gustavopsantos.reflex",
        };

        static ModulateDependencyInstaller()
        {
            // Existing first-run guard
            if (!SessionState.GetBool(SessionKey, false))
            {
                SessionState.SetBool(SessionKey, true);
                EditorApplication.delayCall += Run;
            }

            // Second-stage: open the NuGet window after deps were just installed
            if (SessionState.GetBool(PendingNuGetWindowKey, false))
            {
                EditorApplication.delayCall += TryOpenNuGetWindow;
            }
        }
        
        private static void TryOpenNuGetWindow()
        {
            SessionState.SetBool(PendingNuGetWindowKey, false);
            
            bool nugetLoaded = System.AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "NugetForUnity.Editor");

            if (!nugetLoaded)
            {
                Debug.LogWarning($"{LogPrefix} NuGetForUnity assembly not found yet. " +
                                 "Open NuGet > Manage NuGet Packages manually once it finishes installing.");
                return;
            }

            if (EditorUtility.DisplayDialog(
                    "Modulate – install R3 from NuGet",
                    "UPM packages are installed. The last step is installing R3 from NuGet.\n\n" +
                    "I'll open the NuGet window. Search for \"R3\" and click Install (version 1.3.0 or newer).",
                    "Open NuGet window",
                    "Skip"))
            {
                EditorApplication.ExecuteMenuItem("NuGet/Manage NuGet Packages");
            }
        }

        private static void Run()
        {
            try
            {
                var manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Packages", "manifest.json"));
                if (!File.Exists(manifestPath))
                {
                    Debug.LogWarning($"{LogPrefix} manifest.json not found at {manifestPath}");
                    return;
                }

                var raw = File.ReadAllText(manifestPath);
                var manifest = JObject.Parse(raw);

                bool changed = false;
                changed |= EnsureDependencies(manifest);
                changed |= EnsureScopedRegistry(manifest);

                if (!changed)
                {
                    return;
                }

                if (!ConfirmWithUser())
                {
                    Debug.Log($"{LogPrefix} User declined automatic dependency installation. " +
                              "Add the missing entries manually to Packages/manifest.json.");
                    return;
                }

                var output = manifest.ToString(Formatting.Indented);
                File.WriteAllText(manifestPath, output);
                Debug.Log($"{LogPrefix} Updated Packages/manifest.json. Resolving packages...");

                Client.Resolve();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{LogPrefix} Failed to patch manifest.json: {ex}");
            }
        }

        private static bool EnsureDependencies(JObject manifest)
        {
            if (manifest["dependencies"] is not JObject deps)
            {
                deps = new JObject();
                manifest["dependencies"] = deps;
            }

            bool changed = false;
            foreach (var kvp in RequiredDependencies)
            {
                if (deps[kvp.Key] == null)
                {
                    deps[kvp.Key] = kvp.Value;
                    changed = true;
                    Debug.Log($"{LogPrefix} Will add dependency: {kvp.Key} -> {kvp.Value}");
                }
            }
            return changed;
        }

        private static bool EnsureScopedRegistry(JObject manifest)
        {
            if (manifest["scopedRegistries"] is not JArray registries)
            {
                registries = new JArray();
                manifest["scopedRegistries"] = registries;
            }

            JObject openUpm = null;
            foreach (var entry in registries)
            {
                if (entry is JObject obj && (string)obj["url"] == OpenUpmUrl)
                {
                    openUpm = obj;
                    break;
                }
            }

            bool changed = false;

            if (openUpm == null)
            {
                openUpm = new JObject
                {
                    ["name"]    = OpenUpmName,
                    ["url"]     = OpenUpmUrl,
                    ["scopes"]  = new JArray(OpenUpmScopes),
                };
                registries.Add(openUpm);
                changed = true;
                Debug.Log($"{LogPrefix} Will add scoped registry: {OpenUpmName} ({OpenUpmUrl})");
                return changed;
            }

            if (openUpm["scopes"] is not JArray scopes)
            {
                scopes = new JArray();
                openUpm["scopes"] = scopes;
                changed = true;
            }

            var existing = new HashSet<string>();
            foreach (var s in scopes)
            {
                if (s.Type == JTokenType.String) existing.Add((string)s);
            }

            foreach (var required in OpenUpmScopes)
            {
                if (!existing.Contains(required))
                {
                    scopes.Add(required);
                    changed = true;
                    Debug.Log($"{LogPrefix} Will add scope to {OpenUpmName}: {required}");
                }
            }

            return changed;
        }

        private static bool ConfirmWithUser()
        {
            return EditorUtility.DisplayDialog(
                "Modulate – install dependencies?",
                "Modulate needs to add the following to your project's Packages/manifest.json:\n\n" +
                " • DDElements (Git)\n" +
                " • R3 (Git)\n" +
                " • UniTask (Git)\n" +
                " • Reflex (OpenUPM scoped registry)\n\n" +
                "Proceed? Your manifest.json will be modified and packages will be re-resolved.",
                "Install",
                "Cancel");
        }
        
        [MenuItem("Tools/Modulate/Install Dependencies")]
        private static void RunFromMenu()
        {
            SessionState.SetBool(SessionKey, false);
            Run();
        }
    }
}