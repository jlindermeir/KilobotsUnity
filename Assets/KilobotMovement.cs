using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KilobotMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    private float forwardForce = 1f;
    private float rayDistance = 0.5f;
    private float radius = 0.251f;
    private float torque = 0.2f;

    private static List<float> rayAngles = new List<float>()
    {
        -15, 0, 15
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(transform.up * forwardForce);
        
        // Raycast forward
        var hitList = new List<(bool hasHit, float distance)>();
        
        foreach (float angle in rayAngles)
        {
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;
            Vector3 origin = transform.position + (transform.up * radius);
        
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance);
            Color rayColor = (hit.collider != null) ? Color.red : Color.green;
            Debug.DrawRay(origin, direction * rayDistance, rayColor);

            if (hit.collider != null)
            {
                hitList.Add((true, hit.distance));
            }
            else
            {
                hitList.Add((false, float.PositiveInfinity));
            }
        }

        if (hitList[1].hasHit)
        {
            float signedTorque = (hitList[0].distance > hitList[2].distance) ? -torque : torque;
            rb.AddTorque(signedTorque);
        }
    }
}
