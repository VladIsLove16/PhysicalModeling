using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PointA : MonoBehaviour
{
    public Action pointReached;
    public void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        pointReached?.Invoke();
    }
}

