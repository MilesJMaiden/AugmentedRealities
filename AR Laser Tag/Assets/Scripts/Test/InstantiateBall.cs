using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBall : MonoBehaviour
{
    public GameObject ball;
    public float instantiateSpeed = 5.0f; //Forward speed upon instantiation

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) //Secondary refers to right hand, Index Trigger is the main trigger button.
        {
            GameObject instantiatedBall = Instantiate(ball, transform.position, Quaternion.identity);
            Rigidbody instantiatedBallRB = instantiatedBall.GetComponent<Rigidbody>();
            instantiatedBallRB.velocity = transform.forward * instantiateSpeed;
        }
    }
}
