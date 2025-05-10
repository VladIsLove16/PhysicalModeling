using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] float bounceAcceleration = 1.1f;
    
   public void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity, normal);
            rb.linearVelocity = -reflectDir * (1 + bounceAcceleration); // Ускорение (на 20%)
        }
    }
}


