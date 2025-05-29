using System;
using UnityEngine;

public abstract class FloatingObjectBase : MonoBehaviour, IFloatingObject
{
    [SerializeField] private float density = 800f;
    private Rigidbody rb;
    private float editedVolume;
    private bool isVolumeEdited;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    float IFloatingObject.GetVolume()
    {
        if(isVolumeEdited)
            return editedVolume;
        else
            return transform.localScale.x * transform.localScale.y * transform.localScale.z;
    }

    internal void SetVolume(float value)
    {
        isVolumeEdited = true;
        editedVolume = value;
    }

    public float Density { get => density; set => density = value; }

    public Rigidbody Rigidbody => rb == null ? GetComponent<Rigidbody>() : rb;

    Transform IFloatingObject.transform { get => transform; }
}