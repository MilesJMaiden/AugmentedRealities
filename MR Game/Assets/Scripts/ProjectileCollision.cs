using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Get the name of the projectile
        string projectileName = gameObject.name;

        // Log the projectile name, the object it hit, and the collision position
        Debug.Log(projectileName + " Projectile has collided with " + collision.gameObject.name + " at position " + collision.contacts[0].point);

        // Optional: Destroy the projectile after collision
        Destroy(gameObject);
    }
}