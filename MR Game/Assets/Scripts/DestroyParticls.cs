using UnityEngine;
using System.Collections; // Required for IEnumerator

public class DestroyParticls : MonoBehaviour
{
    void Awake()
    {
        // Start the coroutine properly
        StartCoroutine(deathTimer());
    }

    // The coroutine should return IEnumerator
    private IEnumerator deathTimer()
    {
        // Wait for 4.5 seconds
        yield return new WaitForSeconds(4.5f);

        // Destroy the game object this script is attached to
        Destroy(gameObject);
    }
}
