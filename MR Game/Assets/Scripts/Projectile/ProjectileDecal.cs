using System.Collections;
using UnityEngine;

public class ProjectileDecal : MonoBehaviour
{
    public void StartFadeOut(float fadeOutTime)
    {
        StartCoroutine(FadeOut(fadeOutTime));
    }

    IEnumerator FadeOut(float fadeOutTime)
    {
        // Fade out logic (e.g., reducing alpha over time) can be implemented here
        yield return new WaitForSeconds(fadeOutTime);
        Destroy(gameObject);
    }
}