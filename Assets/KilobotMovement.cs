using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class KilobotMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    
    public float forwardForce = 0.3f;
    public float communicationRadius = 0.3f;
    public float stickRadius = 0.15f;
    public KilobotAgent Agent = new KilobotAgent();


    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(Random.insideUnitCircle * forwardForce * 10);
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateColor();
        
        // Communicate with surrounding kilobots
        Collider2D[] circleHits = Physics2D.OverlapCircleAll(transform.position, communicationRadius);

        foreach (var hit in circleHits)
        {
            if (!hit.CompareTag("Kilobot"))
            {
                continue;
            }
            
            Debug.DrawLine(transform.position, hit.transform.position);

            Vector2 position = hit.transform.position - transform.position;
            float distance = position.magnitude;
        }
    }

    void UpdateColor()
    {
        sr.color = Agent.GetStateColor();
    }
}
