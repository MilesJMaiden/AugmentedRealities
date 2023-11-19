using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTest : MonoBehaviour
{
    public GameObject prefab;
    public float startingVelocity = 12f;

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            GameObject projectile = Instantiate(prefab, transform.position, Quaternion.identity);
            Rigidbody projectilRB = projectile.GetComponent<Rigidbody>();
            projectilRB.velocity = transform.forward * startingVelocity;
        }
    }
}
