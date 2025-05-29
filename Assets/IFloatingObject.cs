using UnityEngine;

public interface IFloatingObject
{
    public const float g = 9.81f;
    public float Density { get; set; }
    public Transform transform { get; }
    Rigidbody Rigidbody { get; }

    public float GetGravityForce()
    {
        return Density * IFloatingObject.g * GetVolume();
    }
    public float GetVolume();
}