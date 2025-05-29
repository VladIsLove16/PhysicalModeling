using UnityEngine;

public interface IFloatingObject
{
    public const float g = 9.81f;
    public float Density { get; set; }
    public Transform transform { get; }
    Rigidbody Rigidbody { get; }

    public float GetGravityForce()
    {
        return Mass * IFloatingObject.g;
    }
    public float GetVolume();
    float Mass
    {
        get
        {
            return Density * GetVolume();
        }
    }
}