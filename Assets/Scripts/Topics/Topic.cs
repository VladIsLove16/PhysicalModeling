using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Topic", menuName ="Topic")]
public class Topic : ScriptableObject
{
    [SerializeField] private string _topicName;
    public string TopicName { get { return _topicName; } }

    public MovementType movementType;
    public TopicFields TopicFields;

    internal TopicFields GetTopicFields()
    {
        throw new NotImplementedException();
    }
}

public enum MovementType
{
    UniformMotion, FreeFall
}
