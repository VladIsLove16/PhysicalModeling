using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WaterCollider : MonoBehaviour
{
    private const float g = 9.81f;
    [SerializeField] float waterDensity = 1000f;
    [SerializeField] float dampingCoefficientLinear = 0.5f;
    [SerializeField] float dampingCoefficientQuadratic = 0.2f;
    //[SerializeField] ForceMode ForceMode = ForceMode.Acceleration;
    [SerializeField] List<FloatingObjectBase> floatingObjects;
    private void FixedUpdate()
    {
        Simulate();
    }

    public Vector3 GetVelocity(int index)
    {
        if (floatingObjects.Count <= index)
            throw new ArgumentException("floatingObjects.Count <= index is not available");
        return floatingObjects[index].Rigidbody.linearVelocity;
    }
    public void SetVolume(int index, float value)
    {
        if (floatingObjects.Count <= index)
            throw new ArgumentException("floatingObjects.Count <= index is not available");
        floatingObjects[index].SetVolume(value);
    }

    private void Simulate()
    {
        foreach (FloatingObjectBase floatingObject in floatingObjects)
        {
            ApplyTotalForce(floatingObject);
        }
    }

    private void ApplyTotalForce(IFloatingObject floatingObject)
    {
        Rigidbody rigidbody = floatingObject.Rigidbody;
        if (rigidbody == null)
        {
            Debug.LogAssertion("floatingObject.Rigidbody is null on " + floatingObject.transform.name);
            return;
        }

        float objVolume = floatingObject.GetVolume();
        float archimedesForce = GetArchimedForce(floatingObject);
        float gravityForce = floatingObject.GetGravityForce();
        float totalForce = archimedesForce - gravityForce;
        totalForce = AddDamping(totalForce, floatingObject);
        
        float acceleration = totalForce / floatingObject.Mass;
        Vector3 applyingAcceleration = Vector3.up * acceleration;
        rigidbody.AddForce(applyingAcceleration, ForceMode.Acceleration);
    }

    private float AddDamping(float totalForce, IFloatingObject floatingObject)
    {
        Vector3 linearVelocity = floatingObject.Rigidbody.linearVelocity;
        float verticalVelocity = Vector3.Dot(linearVelocity, Vector3.up); 
        float dampingForce = -verticalVelocity * dampingCoefficientLinear
                     - Mathf.Sign(verticalVelocity) * verticalVelocity * verticalVelocity * dampingCoefficientQuadratic;
        dampingForce *= floatingObject.Mass;   
        totalForce += dampingForce;
        return totalForce;
    }

    private float GetArchimedForce(IFloatingObject floatingObject)
    {
        float divePercent = CalculateDivePercent(floatingObject.transform);
        float diveVolume = floatingObject.GetVolume() * divePercent;
        float force = CalculateArchimedesForce(waterDensity, diveVolume);
        return force;
    }

    private float CalculateArchimedesForce(float density, float volume)
    {
        float force = density * g * volume;
        return force;
    }

    private float CalculateDivePercent(Transform objTransform)
    {
        return   CalculateDivePercent(objTransform.localScale, objTransform.position);
    }

    private float CalculateDivePercent(Vector3 objectSize,Vector3 objectPosition)
    {
        float objectHeight = objectSize.y;
        if (objectHeight == 0)
            return 0f;
        float waterSurfaceHighestPoint = CalculateSurfaceHighestPoint(transform.localScale,transform.localPosition);
        float objectSurfaceLowestPoint = CalculateSurfaceLowestPoint(objectSize, objectPosition);
        float heightDifference = waterSurfaceHighestPoint - objectSurfaceLowestPoint;
        float divePercent = Mathf.Clamp((heightDifference / objectHeight), 0, 1);
        return divePercent;
    }
    private float CalculateSurfaceHighestPoint(Vector3 size, Vector3 position)
    {
        //Vector3 size = objTransform.localScale;
        //Vector3 position = objTransform.localPosition;
        float y = position.y + size.y/2;
        return y;
    }
    private float CalculateSurfaceLowestPoint(Vector3 size, Vector3 position)
    {
        float y = position.y - size.y/2;
        return y;
    }
}
