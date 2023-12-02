using System.Collections;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [Tooltip("Particle effect prefab to instantiate upon destruction.")]
    public GameObject particleEffectPrefab;

    [Tooltip("Sound to play upon destruction.")]
    public AudioClip collisionSound;

    [Tooltip("Number of collisions the projectile can withstand before being destroyed.")]
    public int maxCollisions = 3;

    [Tooltip("Prefab for the decal to instantiate upon collision.")]
    public GameObject decalPrefab;

    [Tooltip("Time in seconds for the decal to fade out.")]
    public float decalFadeOutTime = 5.0f;

    [Tooltip("Color of the decal based on the projectile type.")]
    public Color decalColor;

    // Counter for the number of collisions
    private int collisionCount = 0;

    // Flag to ensure collision logic only happens once per collision
    private bool hasCollided = false;

    void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple collision handling for the same collision event
        if (hasCollided) return;
        hasCollided = true;

        // Check for collision with an enemy
        if (IsEnemy(collision.gameObject))
        {
            HandleDestruction(collision.contacts[0].point);
            return;
        }

        // Increment collision count and check if max collisions are reached
        collisionCount++;
        if (collisionCount >= maxCollisions)
        {
            HandleDestruction(collision.contacts[0].point);
        }
        else
        {
            // Create a decal at the collision point
            CreateDecal(collision.contacts[0].point, collision.contacts[0].normal);
        }

        // Reset flag for next collision
        hasCollided = false;
    }

    private bool IsEnemy(GameObject obj)
    {
        // Check if the object has any of the enemy tags
        return obj.CompareTag("Enemy") || obj.CompareTag("Red Enemy") ||
               obj.CompareTag("Green Enemy") || obj.CompareTag("Blue Enemy");
    }

    private void HandleDestruction(Vector3 collisionPoint)
    {
        // Instantiate particle effect at the collision point
        if (particleEffectPrefab != null)
        {
            Instantiate(particleEffectPrefab, collisionPoint, Quaternion.identity);
        }

        // Play collision sound at the collision point
        if (collisionSound != null)
        {
            AudioSource.PlayClipAtPoint(collisionSound, collisionPoint);
        }

        // Destroy the parent GameObject or this GameObject, depending on hierarchy
        GameObject objectToDestroy = transform.parent != null ? transform.parent.gameObject : gameObject;
        Destroy(objectToDestroy, collisionSound != null ? collisionSound.length : 0f);
    }

    private void CreateDecal(Vector3 position, Vector3 normal)
    {
        if (decalPrefab != null)
        {
            GameObject decal = Instantiate(decalPrefab, position, Quaternion.LookRotation(normal));
            Renderer decalRenderer = decal.GetComponent<Renderer>();
            if (decalRenderer != null)
            {
                decalRenderer.material.color = decalColor; // Set the decal color
            }

            ProjectileDecal projectileDecal = decal.GetComponent<ProjectileDecal>();
            if (projectileDecal != null)
            {
                projectileDecal.StartFadeOut(decalFadeOutTime);
            }
        }
    }
}