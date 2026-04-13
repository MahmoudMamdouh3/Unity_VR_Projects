using UnityEngine;
using UnityEditor;
using System.Text;

public class InspectorExporterWindow : EditorWindow
{
    private string exportedText = "Select one or more GameObjects in the Hierarchy, then click 'Export'.";
    private Vector2 scrollPosition;

    // This creates a new menu item at the top of your Unity screen
    [MenuItem("Tools/Export Inspector Details")]
    public static void ShowWindow()
    {
        GetWindow<InspectorExporterWindow>("Inspector Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Extremist Detail Exporter", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Selected Objects", GUILayout.Height(40)))
        {
            GenerateExportText();
        }

        EditorGUILayout.Space();
        
        // Creates a scrollable text area to easily copy the data
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        exportedText = EditorGUILayout.TextArea(exportedText, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private void GenerateExportText()
    {
        StringBuilder sb = new StringBuilder();
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            exportedText = "ERROR: No GameObjects selected in the Hierarchy.";
            return;
        }

        foreach (GameObject go in selectedObjects)
        {
            sb.AppendLine("==================================================");
            sb.AppendLine($"GAMEOBJECT: {go.name}");
            sb.AppendLine($"Active: {go.activeSelf} | Tag: {go.tag} | Layer: {LayerMask.LayerToName(go.layer)}");
            sb.AppendLine("==================================================");

            Component[] components = go.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp == null) 
                {
                    sb.AppendLine("\n--- COMPONENT: [MISSING SCRIPT] ---");
                    continue; 
                }

                sb.AppendLine($"\n--- COMPONENT: {comp.GetType().Name} ---");
                
                // Read the exact serialized data the Inspector uses
                SerializedObject so = new SerializedObject(comp);
                SerializedProperty prop = so.GetIterator();
                
                bool enterChildren = true;
                while (prop.NextVisible(enterChildren))
                {
                    enterChildren = false; // Prevents dumping thousands of lines of deep nested Unity code
                    
                    if (prop.name == "m_Script") continue; // Skip the script reference line

                    string val = GetPropertyValueAsString(prop);
                    sb.AppendLine($"  {prop.name}: {val}");
                }
            }
            sb.AppendLine("\n\n");
        }

        exportedText = sb.ToString();
    }

    private string GetPropertyValueAsString(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
            case SerializedPropertyType.Float: return prop.floatValue.ToString();
            case SerializedPropertyType.String: return string.IsNullOrEmpty(prop.stringValue) ? "Empty" : prop.stringValue;
            case SerializedPropertyType.Color: return prop.colorValue.ToString();
            case SerializedPropertyType.ObjectReference: 
                return prop.objectReferenceValue != null ? $"{prop.objectReferenceValue.name} ({prop.objectReferenceValue.GetType().Name})" : "NULL";
            case SerializedPropertyType.LayerMask: return prop.intValue.ToString();
            case SerializedPropertyType.Enum: 
                return (prop.enumNames.Length > 0 && prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumNames.Length) 
                    ? prop.enumNames[prop.enumValueIndex] : prop.enumValueIndex.ToString();
            case SerializedPropertyType.Vector2: return prop.vector2Value.ToString();
            case SerializedPropertyType.Vector3: return prop.vector3Value.ToString();
            case SerializedPropertyType.Vector4: return prop.vector4Value.ToString();
            case SerializedPropertyType.Rect: return prop.rectValue.ToString();
            case SerializedPropertyType.ArraySize: return $"Array Size: {prop.intValue}";
            default: return "[Nested Data/Array]";
        }
    }
}