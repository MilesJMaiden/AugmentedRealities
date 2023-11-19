using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectOnGround : MonoBehaviour
{
    public Transform followTarget;
    public float raycastDistance = 2f;
    public LayerMask layerMask;
    public float spherecastRadius = 0.2f;
    public float verticalOffset = 0.02f;

    void Update()
    {
        RaycastHit raycastHit;

        bool hasHit = Physics.SphereCast(followTarget.position, spherecastRadius, Vector3.down, out raycastHit, raycastDistance, layerMask);

        if(hasHit)
        {
            transform.position = new Vector3(followTarget.position.x, raycastHit.point.y + verticalOffset, followTarget.position.z);
        }
    }
}
