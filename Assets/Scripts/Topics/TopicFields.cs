using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class TopicFields 
{
    [SerializeField] public List<TopicField> Fields;

    public TopicFields(List<TopicField> fields)
    {
        Fields = fields;
    }
}