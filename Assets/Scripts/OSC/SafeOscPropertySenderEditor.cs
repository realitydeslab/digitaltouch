using UnityEngine;
using UnityEditor;
using OscJack;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(OscPropertySender))]
public class SafeOscPropertySenderEditor : Editor
{
    private SerializedProperty connectionProperty;
    private SerializedProperty dataSourceProperty;
    private SerializedProperty propertyNameProperty;
    
    void OnEnable()
    {
        connectionProperty = serializedObject.FindProperty("_connection");
        dataSourceProperty = serializedObject.FindProperty("_dataSource");
        propertyNameProperty = serializedObject.FindProperty("_propertyName");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(connectionProperty);
        EditorGUILayout.PropertyField(dataSourceProperty);
        
        // Safe component property selection
        if (dataSourceProperty.objectReferenceValue != null)
        {
            var gameObject = dataSourceProperty.objectReferenceValue as GameObject;
            if (gameObject != null)
            {
                ShowSafeComponentSelector(gameObject);
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ShowSafeComponentSelector(GameObject gameObject)
    {
        var components = gameObject.GetComponents<Component>();
        var validComponents = new List<Component>();
        var componentNames = new List<string>();
        
        // Filter out null components safely
        foreach (var component in components)
        {
            if (component != null)
            {
                validComponents.Add(component);
                componentNames.Add(component.GetType().Name);
            }
        }
        
        if (validComponents.Count > 0)
        {
            var currentIndex = 0;
            var currentPropertyName = propertyNameProperty.stringValue;
            
            // Find current selection
            for (int i = 0; i < validComponents.Count; i++)
            {
                if (!string.IsNullOrEmpty(currentPropertyName) && componentNames[i] == currentPropertyName.Split('.')[0])
                {
                    currentIndex = i;
                    break;
                }
            }
            
            var newIndex = EditorGUILayout.Popup("Component", currentIndex, componentNames.ToArray());
            if (newIndex != currentIndex || string.IsNullOrEmpty(currentPropertyName))
            {
                var selectedComponent = validComponents[newIndex];
                ShowPropertySelector(selectedComponent);
            }
        }
    }
    
    private void ShowPropertySelector(Component component)
    {
        var properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && (p.PropertyType == typeof(float) || 
                                     p.PropertyType == typeof(int) || 
                                     p.PropertyType == typeof(string) ||
                                     p.PropertyType == typeof(Vector3) ||
                                     p.PropertyType == typeof(bool)))
            .ToArray();
            
        var propertyNames = properties.Select(p => p.Name).ToArray();
        
        if (propertyNames.Length > 0)
        {
            var selectedIndex = EditorGUILayout.Popup("Property", 0, propertyNames);
            propertyNameProperty.stringValue = $"{component.GetType().Name}.{propertyNames[selectedIndex]}";
        }
        else
        {
            EditorGUILayout.LabelField("Property", "No compatible properties found");
        }
    }
}