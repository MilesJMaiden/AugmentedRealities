using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{

    public int health;
    public int attack;

    public void TakeDamage(int amount)
    {
        health -= amount;
    }
    public void DealDamage(GameObject target)
    {
        var atm = target.GetComponent<PlayerAttributes>();
        if (atm != null)
        {
            atm.TakeDamage(attack);
        }
    }
}
