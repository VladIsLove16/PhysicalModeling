using System;
using UnityEngine;

public class Wood : MonoBehaviour, IFloatingObject
{
    [SerializeField] private float density = 800f;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    float IFloatingObject.GetVolume()
    {
        return transform.localScale.x * transform.localScale.y * transform.localScale.z;
    }

    public float Density { get => density; set => density = value; }

    public Rigidbody Rigidbody => rb == null ? GetComponent<Rigidbody>() : rb;

    Transform IFloatingObject.transform { get => transform;  }
}