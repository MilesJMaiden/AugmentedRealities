using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public GameObject[] projectilePrefabs; // Array to hold different projectile prefabs
    public GameObject[] weaponModels; // Array to hold different weapon model prefabs
    public Color[] projectileColors;
    private GameObject currentWeapon; // Current weapon GameObject
    public GameObject weaponAnchorPoint; // Anchor point for instantiating weapons
    public Transform projectileSpawnPoint; // Transform of the spawn point for projectiles
    private int currentWeaponIndex = 0; // Current weapon index
    private float lastKeyPressTime = 0f; // Time of the last Q key press
    public float doubleTapTime = 0.5f; // Time interval to detect double tap
    public float startingVelocity = 12f;

    void Start()
    {
        // Instantiate the first weapon at the start
        currentWeapon = Instantiate(weaponModels[currentWeaponIndex], weaponAnchorPoint.transform.position, weaponAnchorPoint.transform.rotation);
        currentWeapon.transform.SetParent(weaponAnchorPoint.transform, worldPositionStays: false);
    }

    void Update()
    {
        HandleWeaponSwitch();
        HandleShooting();

        // Debug controls
        HandleDebugInput();
    }

    void HandleWeaponSwitch()
    {
        if (OVRInput.GetDown(OVRInput.Button.One) || (Input.GetKeyDown(KeyCode.Q) && Time.time - lastKeyPressTime < doubleTapTime))
        {
            // Double tapped, switch weapon and projectile
            SwitchWeaponAndProjectile();
            lastKeyPressTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            lastKeyPressTime = Time.time;
        }
    }

    void SwitchWeaponAndProjectile()
    {
        // Destroy the current weapon model
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Switch to the next weapon and projectile
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponModels.Length;

        // Instantiate the new weapon model at the anchor point's position
        currentWeapon = Instantiate(weaponModels[currentWeaponIndex], weaponAnchorPoint.transform.position, weaponAnchorPoint.transform.rotation);

        // Set the parent of the weapon to the anchor point
        currentWeapon.transform.SetParent(weaponAnchorPoint.transform);
    }

    void HandleShooting()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || Input.GetMouseButtonDown(0))
        {
            GameObject projectilePrefab = projectilePrefabs[currentWeaponIndex];
            // Instantiate the projectile at the spawn point position and rotation
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            // Pass the color information to the projectile
            ProjectileCollision projectileCollision = projectile.GetComponent<ProjectileCollision>();
            if (projectileCollision != null)
            {
                projectileCollision.decalColor = projectileColors[currentWeaponIndex]; // Set color based on weapon type
            }

            // Search for Rigidbody in the children of the projectile
            Rigidbody projectileRB = projectile.GetComponentInChildren<Rigidbody>();

            if (projectileRB != null)
            {
                projectileRB.velocity = projectileSpawnPoint.forward * startingVelocity;
            }
            else
            {
                Debug.LogWarning("Rigidbody not found in the children of the projectile prefab.");
            }
        }
    }

    void HandleDebugInput()
    {
        // Add any additional keyboard/mouse debug controls here if needed
    }
}