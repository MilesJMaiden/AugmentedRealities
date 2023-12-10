using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public string targetTag;
    public Enemy enemy;
    public float baseDamage = 25f;

    private void OnCollisionEnter(Collision collision)
    {
        float damage = baseDamage;

        if (collision.gameObject.tag == targetTag)
        {
            damage *= 2;
        }

        enemy.TakeDamage(damage, collision.contacts[0].point);
    }

}
