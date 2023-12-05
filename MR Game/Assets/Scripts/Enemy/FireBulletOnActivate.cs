using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireBulletOnActivate : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bullet;
    public Transform bulletOrigin;
    public float fireSpeed = 20f;

    public AudioSource source;
    public AudioClip clip;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireBullet()
    {
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.position = bulletOrigin.position;
        spawnedBullet.GetComponentInChildren<Rigidbody>().velocity = bulletOrigin.forward * fireSpeed;
        Destroy(spawnedBullet, 5f);
    }
}
