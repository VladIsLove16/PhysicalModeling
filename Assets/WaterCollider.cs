using UnityEngine;

public class WaterCollider : MonoBehaviour
{
    private const float g = 9.81f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float waterDensity = 1000f;
    [SerializeField] ForceMode ForceMode = ForceMode.Acceleration;

    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, layerMask);
        foreach (Collider hit in hits)
        {
            IFloatingObject floatingObject = hit.GetComponent<IFloatingObject>();
            if (floatingObject == null)
            {
                Debug.LogAssertion("collider " + hit.gameObject.name + " is in layerMask " + layerMask.ToString() + " but is not IFloatingObject");
                continue;
            }
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
        float ArchimedForceMagnitude = GetArchimedForce(floatingObject) * Time.fixedDeltaTime;
        float gravityForceMagnitude = floatingObject.GetGravityForce() * Time.fixedDeltaTime;
        Vector3 appliyngForce = Vector3.up * (ArchimedForceMagnitude - gravityForceMagnitude);
        Debug.Log(ArchimedForceMagnitude + " - " + gravityForceMagnitude + "  = " + appliyngForce);
        rigidbody.AddForce(appliyngForce, ForceMode);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
       
    }

    private float GetArchimedForce(IFloatingObject floatingObject)
    {
        float divePercent = CalculateDivePercent(floatingObject.transform);
        float diveVolume = floatingObject.GetVolume() * divePercent;
        float force = CalculateArchimedesForce(waterDensity, diveVolume);
        return force;
    }

    //public float CalculateObjectVolume(Vector3 size)
    //{
    //    return size.x*size.y*size.z;
    //}

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
        //Vector3 size = objTransform.localScale;
        //Vector3 position = objTransform.localPosition;
        float y = position.y - size.y/2;
        return y;
    }
}
