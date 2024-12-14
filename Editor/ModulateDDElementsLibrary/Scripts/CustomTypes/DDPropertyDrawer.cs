using System.Collections.Generic;
using System.Linq;
using DandyDino.Modulate;
using UnityEditor;
using UnityEngine;

public class DDPropertyDrawer<T>: PropertyDrawer
{
    protected SerializedObject SerializedObject => _property.serializedObject;
    private bool _hasInitialized;
    
    protected T _target;
    protected SerializedProperty _property;
    protected GUIContent _label;
    protected Rect _position;
    protected int _positionOffset;

    protected string[] _properties;

    protected bool _ignoreAllProperties;
    protected string[] _propertiesToIgnore;

    protected virtual void Init(SerializedProperty property, GUIContent label)
    {
        if (_hasInitialized)
        {
            return;
        }
    
        _property = property;
        _label = label;
        
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            var managedReference = property.managedReferenceValue;
            if (managedReference != null)
            {
                _target = (T)managedReference;
            }
        }
    
        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = property.GetEndProperty();

        List<string> tempProperties = new List<string>();

        while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            if (tempProperties.Contains(iterator.name))
            {
                continue;
            }
            tempProperties.Add(iterator.name);
            _hasInitialized = true;
        }

        _properties = tempProperties.ToArray();
    }

    public SerializedProperty GetProperty(string str)
    {
        string result = _properties.Where(x => x == str).FirstOrDefault();
        if (result == null)
        {
            return null;
        }
        
        return _property.FindPropertyRelative(result);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property, label);
        return base.GetPropertyHeight(property, label);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Event.current.type == EventType.Repaint)
        {
            _position = position;
            _positionOffset = -Mathf.RoundToInt(_position.width);
        }
        
        
        DDElements.Layout.Row(() =>
        {
            DDElements.Layout.Space(_positionOffset);
            
            DDElements.Layout.Column(() =>
            {
                OnGUI();
            });
        });
    }

    public virtual void OnGUI()
    {
        if (_ignoreAllProperties)
        {
            return;
        }
        
        for (int i = 0; i < _properties.Length; i++)
        {
            string propertyName = _properties[i];
            if (_propertiesToIgnore != null && _propertiesToIgnore.Length > 0 && _propertiesToIgnore.Contains(propertyName))
            {
                continue;
            }
            
            SerializedProperty storedProperty = GetProperty(propertyName);
            if (storedProperty == null)
            {
                continue;
            }
            EditorGUILayout.PropertyField(GetProperty(propertyName));
        }
    }
}