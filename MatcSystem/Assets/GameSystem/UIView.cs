using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class UIView : MonoBehaviour
{
    public void LoadUI() {
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
            if (field.GetCustomAttribute(typeof(UIAttribute), true) is UIAttribute attr)
                attr.Set(this, field);
    }
}

public class UIAttribute : Attribute {
    public string path;
    public UIAttribute(string path)
    {
        this.path = path;
    }
    public void Set(MonoBehaviour mono, FieldInfo field)
    {
        string typeName = field.FieldType.FullName.Replace("[]", string.Empty);
        Type type = field.FieldType.Assembly.GetType(typeName);
        var found = mono.gameObject.transform.Find(path);
        var com = found.GetComponent(type);
        field.SetValue(mono, com);
    }
}


