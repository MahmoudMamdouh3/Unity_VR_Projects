using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class InspectorSearcher : EditorWindow
{
    [MenuItem("Tools/Inspector Searcher")]
    public static void ShowWindow() => GetWindow<InspectorSearcher>("Inspector Search");

    private string _searchQuery = "";
    private Vector2 _scrollPos;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _searchQuery = EditorGUILayout.TextField("Search Properties/Components", _searchQuery, EditorStyles.toolbarSearchField);
        
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorGUILayout.HelpBox("Select a GameObject in the Hierarchy to begin searching.", MessageType.Info);
            return;
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        foreach (var comp in selected.GetComponents<Component>())
        {
            if (comp == null) continue;

            string compName = comp.GetType().Name;
            SerializedObject so = new SerializedObject(comp);
            SerializedProperty prop = so.GetIterator();
            List<string> matchingProps = new List<string>();

            // If we have a query, filter properties
            if (!string.IsNullOrEmpty(_searchQuery))
            {
                while (prop.NextVisible(true))
                {
                    if (prop.displayName.ToLower().Contains(_searchQuery.ToLower()))
                    {
                        matchingProps.Add(prop.displayName);
                    }
                }
            }

            // Draw if component name matches OR property matches
            bool compNameMatches = compName.ToLower().Contains(_searchQuery.ToLower());
            if (compNameMatches || matchingProps.Count > 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(compName, EditorStyles.boldLabel);
                if (GUILayout.Button("Focus", GUILayout.Width(50)))
                {
                    // Pings the component in the actual Inspector
                    EditorGUIUtility.PingObject(comp);
                }
                EditorGUILayout.EndHorizontal();

                foreach (var pName in matchingProps)
                {
                    EditorGUILayout.LabelField($"   • {pName}", EditorStyles.miniLabel);
                }

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    // Auto-refresh when you click a different object
    private void OnSelectionChange() => Repaint();
}