using UnityEngine;

public class Accelerator : MonoBehaviour
{
    [SerializeField] float acceleration = 1.1f;
    
   public void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity, normal);
            rb.linearVelocity = reflectDir * acceleration; // Ускорение (на 20%)
        }
    }
}


