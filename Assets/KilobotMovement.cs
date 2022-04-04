using System;
using System.Collections.Generic;
using Agents;
using UnityEngine;

public class KilobotMovement<TMessage> : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public TextMesh tm;

    private const float ForwardForce = 0.5f;
    private const float TorqueMag = 2f;
    private const float CommunicationRadius = 4f;
    public IAgentInterface<TMessage> Agent;
    public TMessage CurrentMessage;


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
        Collider2D[] circleHits = Physics2D.OverlapCircleAll(position, CommunicationRadius);
        
        // Create a list of messages from other Kilobots and their distances
        List<Tuple<float, TMessage>> messageList = new List<Tuple<float, TMessage>>();
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
            TMessage message = hit.GetComponent<KilobotMovement<TMessage>>().CurrentMessage;
            
            // Add the distance and message to the list
            messageList.Add(new Tuple<float, TMessage>(distance, message));
        }
        
        // Get an motion direction and a message from the agent
        (Vector2 direction, float torque, TMessage newMessage) = Agent.Act(messageList);
        
        // Draw a line to indicate the deviation from the estimated and actual position
        if (Agent.PositionEstimated)
        {
            Debug.DrawLine(position, Agent.PositionEstimate, Color.red);
        }

        // Move in the specified direction
        rb.AddRelativeForce(direction.normalized * ForwardForce);
        rb.AddTorque(torque * TorqueMag);
        
        // Set the new message as current
        CurrentMessage = newMessage;
        
        // Update the color according to the state
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        sr.color = Agent.GetStateColor();
        tm.text = Agent.GetDisplayText();
    }

    public void AddKilobotComponents(GameObject kilobot)
    {
        rb = kilobot.GetComponent<Rigidbody2D>();
        sr = kilobot.GetComponent<SpriteRenderer>();
        tm = kilobot.GetComponentInChildren<TextMesh>();
    }
}
