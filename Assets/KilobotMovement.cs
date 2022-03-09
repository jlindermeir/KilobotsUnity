using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class KilobotMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    
    public enum StateEnum
    {
        Movement,
        FixedPosition
    }
    public StateEnum state = StateEnum.Movement;
    public Vector2 position = new Vector2(float.NaN, float.NaN);

    private Dictionary<StateEnum, Color> stateColor = new Dictionary<StateEnum, Color>()
    {
        {StateEnum.Movement, Color.red},
        {StateEnum.FixedPosition, Color.green}
    };

    public float forwardForce = 0.3f;
    public float communicationRadius = 0.3f;
    public float stickRadius = 0.15f;


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

        var kilobotInfo = new List<(float distance, StateEnum state, Vector2 position)>();
        foreach (var hit in circleHits)
        {
            if (!hit.CompareTag("Kilobot"))
            {
                continue;
            }
            
            Debug.DrawLine(transform.position, hit.transform.position);

            Vector2 position = hit.transform.position - transform.position;
            float distance = position.magnitude;
            KilobotMovement km = hit.gameObject.GetComponent<KilobotMovement>();
            kilobotInfo.Add((distance, km.state, position));
        }
        
        // Estimate position
        // TODO: actually implement this distributively
        Vector2 estimatedPosition = transform.position;
        
        // State specific behaviour
        switch (state)
        {
            case StateEnum.Movement:
                foreach (var kilobot in kilobotInfo)
                {
                    float force = stickRadius - kilobot.distance;
                    rb.AddForce(-kilobot.position.normalized * force);
                }
                    
                break;
            case StateEnum.FixedPosition:
                break;
        }
        
        rb.AddForce(Random.insideUnitCircle * forwardForce);
    }

    void UpdateColor()
    {
        sr.color = stateColor[state];
    }
}
