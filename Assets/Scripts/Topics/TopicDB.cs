using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TopicDB",menuName = "TopicDB")]
public class TopicDB : ScriptableObject
{
    [SerializeField] List<Topic> topics;
    public List<Topic> GetTopics()
    {
        return topics;
    }
    public Topic GetTopic(string name)
    {
        return topics.Find(p => p.TopicName == name);
    }

}

