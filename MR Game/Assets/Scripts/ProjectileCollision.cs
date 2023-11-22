using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public GameObject particleEffectPrefab; // Particle effect prefab
    public AudioClip collisionSound; // Sound to play on collision
    private bool hasCollided = false; // Flag to ensure collision logic only happens once

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return; // Prevent multiple collision handling
        hasCollided = true;

        // Instantiate the particle effect at the collision point
        if (particleEffectPrefab != null)
        {
            Instantiate(particleEffectPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        // Play the collision sound at the collision point
        if (collisionSound != null)
        {
            AudioSource.PlayClipAtPoint(collisionSound, collision.contacts[0].point);
        }

        // Disable the Mesh Renderer to make the projectile invisible
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) meshRenderer.enabled = false;

        // Disable the Collider to prevent further collision interactions
        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // Make the Rigidbody kinematic to stop it from moving
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Destroy the projectile GameObject after a delay
        Destroy(gameObject, collisionSound != null ? collisionSound.length : 0f);
    }
}