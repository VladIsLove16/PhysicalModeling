using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PointA : MonoBehaviour
{
    public Action pointReached;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        pointReached?.Invoke();
        Debug.Log("point a reached");
    }
}

