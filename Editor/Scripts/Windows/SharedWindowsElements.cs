using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DandyDino.Modulate
{
    public class SharedWindowsElements
    {
        internal static void DrawAssembliesList(ref string searchQuery, ref Vector2 availableAssembliesScroll, ref Vector2 includedAssembliesScroll, List<string> assemblies, List<string> assembliesToAdd)
        {
            Vector2 availableScroll = availableAssembliesScroll;
            Vector2 includedScroll = includedAssembliesScroll;
            string tempQuery = searchQuery;
            DDElements.Layout.Column(() =>
            {
                DDElements.Layout.Space(5);
                DDElements.Rendering.SearchBar(ref tempQuery, null);
                DDElements.Layout.Space(5);
                
                DDElements.Layout.Row(() =>
                {
                    DDElements.Layout.FlexibleSpace();

                    DDElements.Layout.Column(() =>
                    {
                        GUILayout.Label("Included Dependencies: ");
                        DDElements.Layout.Space(2);
                        DDElements.Rendering.Line();
                        DDElements.Layout.ScrollView(ref availableScroll, () =>
                        {
                            for (int i = 0; i < assembliesToAdd.Count; i++)
                            {
                                string assembly = assembliesToAdd[i];
                                
                                if (!string.IsNullOrWhiteSpace(tempQuery) && !assembly.Contains(tempQuery))
                                {
                                    continue;
                                }
                                
                                DDElements.Layout.Row(() =>
                                {
                                    DDElements.Rendering.IconButton(DDElements.Icons.Remove(), 14, () =>
                                    {
                                        assembliesToAdd.Remove(assembly);
                                    });

                                    GUILayout.Label(assembly);
                                }, style: EditorStyles.helpBox);
                            }
                        });
                    }, options: new[] { GUILayout.Width(440) });

                    DDElements.Layout.Column(() =>
                    {
                        GUILayout.Label("Available Dependencies: ");
                        DDElements.Rendering.Line();
                        DDElements.Layout.ScrollView(ref includedScroll, () =>
                        {
                            for (int i = 0; i < assemblies.Count; i++)
                            {
                                string assembly = assemblies[i];

                                if (!string.IsNullOrWhiteSpace(tempQuery) && !assembly.Contains(tempQuery))
                                {
                                    continue;
                                }

                                if (assembliesToAdd.Contains(assembly) ||
                                    assembly == StringLibrary.ASSEMBLY_DEFINITION ||
                                    assembly == StringLibrary.ASSEMBLY_DEFINITION_EDITOR ||
                                    assembly == StringLibrary.ELEMENTS_ASSEMBLY_DEFINITION)
                                {
                                    continue;
                                }

                                DDElements.Layout.Row(() =>
                                {
                                    DDElements.Rendering.IconButton(DDElements.Icons.BluishPlus(), 14, () =>
                                    {
                                        assembliesToAdd.Add(assembly);
                                    });
                                    GUILayout.Label(assembly);
                                    DDElements.Layout.FlexibleSpace();
                                }, style: EditorStyles.helpBox);
                            }
                        }, options: new[] { GUILayout.Width(440) });
                    });

                    DDElements.Layout.FlexibleSpace();
                });
            });
            availableAssembliesScroll = availableScroll;
            includedAssembliesScroll = includedScroll;
            searchQuery = tempQuery;
        }
    }
}