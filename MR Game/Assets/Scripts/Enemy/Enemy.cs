using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public FireBulletOnActivate gun;
    public NavMeshAgent agent;
    public Animator animator;
    public Transform playerHead;
    public Transform playerTarget;
    public GameObject butterflys;
    public float stopDistance = 6f;
    public float health = 100f; 

    private Quaternion localRotationGun;

    private GameManager gameManager;

    void Awake()
    {
        // Find the OVRPlayer GameObject
        GameObject playerGameObject = GameObject.Find("OVRPlayer");
        if (playerGameObject != null)
        {
            playerTarget = playerGameObject.transform;

            // Find the OVRPlayerCameraRig as a child of OVRPlayer
            Transform cameraRigTransform = playerGameObject.transform.Find("OVRPlayerCameraRig");
            if (cameraRigTransform != null)
            {
                // Assign the camera rig itself as the player head
                playerHead = cameraRigTransform;

                // If additional confirmation is needed, log a message
                Debug.Log("OVRPlayerCameraRig assigned as player head");
            }
            else
            {
                Debug.LogError("OVRPlayerCameraRig not found as a child of OVRPlayer");
            }
        }
        else
        {
            Debug.LogError("OVRPlayer not found in the scene");
        }
    }

    void Start()
    {
        SetupRagdoll();

        localRotationGun = gun.bulletOrigin.transform.localRotation;
    }

    void Update()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            float distance = Vector3.Distance(playerTarget.position, transform.position);

            if (distance > stopDistance)
            {
                // Move towards the player and play running animation
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
                animator.SetBool("Run", true);
                animator.SetBool("Shoot", false);
            }
            else
            {
                // Stop and shoot, play shooting animation
                agent.isStopped = true;
                animator.SetBool("Run", false);
                animator.SetBool("Shoot", true);

                // Face the player
                FaceTarget();
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
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

    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        health -= damage;
        if (health <= 0)
        {
            Dead(hitPosition); 
        }
    }

    public void Dead(Vector3 hitPosition)
    {

        // Add a check to ensure the enemy is not already destroyed
        if (this == null)
            return;

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

        if (gameManager != null)
        {
            gameManager.EnemyKilled(gameObject);
        }

        ThrowGun();
        animator.enabled = false;
        agent.enabled = false;

        // Safely destroy the enemy
        SafeDestruction();

    }

    private IEnumerator butterflyExplosion()
    {
        GameObject butterflyParticles = Instantiate(butterflys, gameObject.transform);
        yield return new WaitForSeconds(3);
        Destroy(butterflyParticles);
    }

    public void SafeDestruction()
    {
        // Stop all coroutines or any ongoing operations
        StopAllCoroutines();

        // Finally, destroy the object
        Destroy(gameObject);
    }
}
