using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public GameObject[] projectilePrefabs; // Array to hold different projectile prefabs
    public GameObject[] weaponModels; // Array to hold different weapon models
    private int currentWeaponIndex = 0; // Current weapon index
    private float lastButtonPressTime = 0f; // Time of the last primary button press
    public float doubleTapTime = 0.5f; // Time interval to detect double tap
    public float startingVelocity = 12f;

    private enum WeaponType
    {
        Blue, // Water
        Red,  // Fire
        Green // Grass
    }

    void Start()
    {
        // Initialize the weapon models (activate the first, deactivate others)
        foreach (GameObject model in weaponModels)
        {
            model.SetActive(false);
        }
        weaponModels[currentWeaponIndex].SetActive(true);
    }

    void Update()
    {
        HandleWeaponSwitch();
        HandleShooting();
    }

    void HandleWeaponSwitch()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (Time.time - lastButtonPressTime < doubleTapTime)
            {
                // Double tapped, switch weapon
                SwitchWeapon();
            }
            lastButtonPressTime = Time.time;
        }
    }

    void SwitchWeapon()
    {
        // Deactivate the current weapon model
        weaponModels[currentWeaponIndex].SetActive(false);

        // Switch to the next weapon
        currentWeaponIndex = (currentWeaponIndex + 1) % projectilePrefabs.Length;

        // Activate the new weapon model
        weaponModels[currentWeaponIndex].SetActive(true);
    }

    void HandleShooting()
    {
        //Secondary = righthand
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            GameObject projectilePrefab = projectilePrefabs[currentWeaponIndex];
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody projectilRB = projectile.GetComponent<Rigidbody>();
            projectilRB.velocity = transform.forward * startingVelocity;
        }
    }
}