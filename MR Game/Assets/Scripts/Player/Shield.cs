using UnityEngine;
using System.Collections;
using System.Linq;

public class Shield : MonoBehaviour
{
    public string projectileTag = "Projectile"; // Tag to identify projectiles
    public GameObject[] shieldParts; // Publicly referenced shield parts
    public float rechargeTime = 10.0f; // Time to recharge the shield

    private int hitCount = 0;
    private IEnumerator currentCoroutine;
    private bool shieldEnabled = true;

    void Start()
    {
        // Assuming shieldParts are already assigned in the inspector
        // Initialize all parts as active
        ResetShield();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(projectileTag))
        {
            TakeHit();
        }
    }

    public void TakeHit()
    {
        //if (!shieldEnabled) { statManageer.PlayerShot(); return; } //if shield isnt currently active, player takes damage instead
        
        
        hitCount++; //if player didnt take damage (shield is active) then do damage to shield
        if (hitCount >= shieldParts.Length)
        {
            DisableShield();
            shieldEnabled = false;
            hitCount = 0; // Reset hit count after all parts are disabled
        }
        else
        {
            DisableNextShieldPart();
        }

        // Reset or start the coroutine for re-enabling shield parts
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = ReEnableShield();
        StartCoroutine(currentCoroutine);
    }

    private void DisableNextShieldPart()
    {
        foreach (var part in shieldParts)
        {
            if (part.activeSelf)
            {
                part.SetActive(false);
                break;
            }
        }
    }

    private IEnumerator ReEnableShield()
    {
        yield return new WaitForSeconds(rechargeTime);
        ResetShield();
    }

    private void ResetShield()
    {
        foreach (var part in shieldParts)
        {
            part.SetActive(true);
        }
        shieldEnabled = true;
    }

    private void DisableShield()
    {
        foreach (var part in shieldParts)
        {
            part.SetActive(false);
        }
    }
}
