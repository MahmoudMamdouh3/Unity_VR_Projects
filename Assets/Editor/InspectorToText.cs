using UnityEngine;
using UnityEditor;
using System.Text;

public class InspectorToText : EditorWindow
{
    private string _outputText = "";
    private Vector2 _scrollPos;
    private bool _includeChildren = false;

    [MenuItem("Tools/LLM Context Exporter")]
    public static void ShowWindow()
    {
        GetWindow<InspectorToText>("LLM Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Export Selected Objects to Text", EditorStyles.boldLabel);
        _includeChildren = EditorGUILayout.Toggle("Include Children?", _includeChildren);

        if (GUILayout.Button("Generate Text from Selection", GUILayout.Height(30)))
        {
            GenerateText();
        }

        if (!string.IsNullOrEmpty(_outputText))
        {
            if (GUILayout.Button("Copy to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = _outputText;
                Debug.Log("Inspector data copied to clipboard!");
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.TextArea(_outputText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    private void GenerateText()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            _outputText = "No GameObjects selected.";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("UNITY INSPECTOR DATA DUMP");
        sb.AppendLine("==========================");

        foreach (GameObject go in selectedObjects)
        {
            ProcessGameObject(go, sb, 0);
        }

        _outputText = sb.ToString();
    }

    private void ProcessGameObject(GameObject go, StringBuilder sb, int indent)
    {
        string space = new string(' ', indent * 4);
        sb.AppendLine($"{space}OBJECT NAME: {go.name} (Tag: {go.tag}, Layer: {LayerMask.LayerToName(go.layer)})");
        sb.AppendLine($"{space}--------------------------------------------------");

        Component[] components = go.GetComponents<Component>();
        foreach (var comp in components)
        {
            if (comp == null) continue;

            sb.AppendLine($"{space}[Component: {comp.GetType().Name}]");
            
            SerializedObject so = new SerializedObject(comp);
            SerializedProperty prop = so.GetIterator();
            
            // Move to first property
            if (prop.NextVisible(true))
            {
                do
                {
                    // Skip the internal 'm_Script' field to keep it clean
                    if (prop.name == "m_Script") continue;

                    string valueStr = GetPropertyValue(prop);
                    sb.AppendLine($"{space}  - {prop.name}: {valueStr}");
                } 
                while (prop.NextVisible(false)); // false = don't descend deep into every sub-property
            }
            sb.AppendLine("");
        }

        if (_includeChildren)
        {
            foreach (Transform child in go.transform)
            {
                sb.AppendLine("");
                ProcessGameObject(child.gameObject, sb, indent + 1);
            }
        }
    }

    private string GetPropertyValue(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
            case SerializedPropertyType.Float: return prop.floatValue.ToString();
            case SerializedPropertyType.String: return prop.stringValue;
            case SerializedPropertyType.Color: return prop.colorValue.ToString();
            case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue != null ? prop.objectReferenceValue.name : "None/Null";
            case SerializedPropertyType.Vector2: return prop.vector2Value.ToString();
            case SerializedPropertyType.Vector3: return prop.vector3Value.ToString();
            case SerializedPropertyType.Rect: return prop.rectValue.ToString();
            case SerializedPropertyType.Enum: return prop.enumNames[prop.enumValueIndex];
            case SerializedPropertyType.AnimationCurve: return "AnimationCurve Data";
            default: return "(Unsupported Type)";
        }
    }
}