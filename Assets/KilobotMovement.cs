using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Random = UnityEngine.Random;

public class KilobotMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public TextMesh tm;
    
    private float forwardForce = 0.5f;
    private float torqueMag = 2f;
    private float communicationRadius = 4f;
    public KilobotAgent Agent = new KilobotAgent();
    public KilobotMessage CurrentMessage;


    // Start is called before the first frame update
    void Start()
    {
        CurrentMessage = Agent.GetMessage();
    }

    // Update is called once per frame
    void Update()
    {   
        // Determine the position of the Kilobot
        Vector2 position = transform.position;
        
        // Communicate with surrounding Kilobots within the communication radius
        Collider2D[] circleHits = Physics2D.OverlapCircleAll(transform.position, communicationRadius);
        
        // Create a list of messages from other Kilobots and their distances
        List<Tuple<float, KilobotMessage>> messageList = new List<Tuple<float, KilobotMessage>>();
        foreach (var hit in circleHits)
        {
            if (!hit.CompareTag("Kilobot") | (hit.gameObject == transform.gameObject))
            {
                continue;
            }
            
            // Determine the position of the other Kilobot
            Vector2 otherPosition = hit.transform.position;
            
            // Draw a line to indicate the communication link
            Debug.DrawLine(position, otherPosition);
            
            // Determine the distance and get the message
            float distance = (otherPosition - position).magnitude;
            KilobotMessage message = hit.GetComponent<KilobotMovement>().CurrentMessage;
            
            // Add the distance and message to the list
            messageList.Add(new Tuple<float, KilobotMessage>(distance, message));
        }
        
        // Get an motion direction and a message from the agent
        (Vector2 direction, float torque, KilobotMessage newMessage) = Agent.Act(messageList);
        
        // Draw a line to indicate the deviation from the estimated and actual position
        if (Agent.PositionEstimated)
        {
            Debug.DrawLine(position, Agent.PositionEstimate, Color.red);
        }

        // Move in the specified direction
        rb.AddRelativeForce(direction.normalized * forwardForce);
        rb.AddTorque(torque * torqueMag);
        
        // Set the new message as current
        CurrentMessage = newMessage;
        
        // Update the color according to the state
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        sr.color = Agent.GetStateColor();
        tm.text = Agent.Gradient.ToString();
    }
}
