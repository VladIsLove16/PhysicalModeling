using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public class TopicFields
{
    [SerializeField] private List<TopicField> fields;

    private Dictionary<ParamName, TopicField> fieldLookup;
    private Dictionary<ParamName, TopicField> FieldLookup
    {
        get 
        {
            if(fieldLookup == null)
            {
                fieldLookup = BuildLookup();
            }
            return fieldLookup; 
        }
    }

    public List<TopicField> Fields => fields;
    public Dictionary<ParamName, TopicField> BuildLookup()
    {
       return fields.ToDictionary(f => f.ParamName, f => f);
    }

    public bool IsReadOnly(ParamName paramName)
    {
        return FieldLookup.TryGetValue(paramName, out var field) && field.IsReadOnly;
    }

    public FieldType GetFieldType(ParamName paramName)
    {
        return FieldLookup.TryGetValue(paramName, out var field) ? field.Type : FieldType.Float;
    }


}
