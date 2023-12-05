using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public FireBulletOnActivate gun;

    private NavMeshAgent agent;
    private Animator animator;
    public Transform playerHead;
    public Transform playerTarget;

    public float stopDistance = 6f;

    private Quaternion localRotationGun;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); 
        SetupRagdoll();

        localRotationGun = gun.bulletOrigin.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTarget.position);

        float distance = Vector3.Distance(playerTarget.position, transform.position);  
        if (distance < stopDistance)
        {
            agent.isStopped = true;
            animator.SetBool("Shoot", true);
        }
    }

    public void ThrowGun()
    {
        gun.bulletOrigin.localRotation = localRotationGun;

        gun.transform.parent = null;
        //Maths Physics Method
        Rigidbody rb = gun.GetComponent<Rigidbody>();
        rb.velocity = BallisticVelocityVector(gun.transform.position, playerHead.position, 45f);
        rb.angularVelocity = Vector3.zero;
    }
    
    Vector3 BallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        //calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction;
    }

    public void ShootEnemy()
    {
        Vector3 playerHeadPosition = playerHead.position - Random.Range(0, 0.4f) * Vector3.up;
        gun.bulletOrigin.forward = (playerHeadPosition - gun.bulletOrigin.position).normalized;
        gun.FireBullet();
    }

    public void SetupRagdoll()
    {
        //loop through all limbs
        foreach (var item in GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = true;
        }
    }

    public void Dead(Vector3 hitPosition)
    {
        //loop through all limbs
        foreach (var item in GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = true;
        }

        //Gets all body components within range of hitposition 
        foreach (var item in Physics.OverlapSphere(hitPosition, 0.3f))
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {

                rb.AddExplosionForce(1000, hitPosition, 0.3f);
            }
        }


        ThrowGun();
        animator.enabled= false;
        agent.enabled= false;
        this.enabled= false;

    }
}
