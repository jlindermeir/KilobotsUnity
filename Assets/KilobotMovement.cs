using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class KilobotMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public TextMesh tm;
    
    public float forwardForce = 0.3f;
    public float communicationRadius = 0.3f;
    public KilobotAgent Agent = new KilobotAgent();
    public KilobotMessage CurrentMessage = KilobotMessage.Empty();


    // Start is called before the first frame update
    void Start()
    {
        
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
            if (!hit.CompareTag("Kilobot"))
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
        (Vector2 direction, KilobotMessage newMessage) = Agent.Act(messageList);
        
        // Move in the specified direction
        rb.AddForce(direction.normalized * forwardForce);
        
        // Set the new message as current
        CurrentMessage = newMessage;
        
        // Update the color according to the state
        UpdateSprite();
    }

    void UpdateSprite()
    {
        sr.color = Agent.GetStateColor();
        tm.text = Agent.Gradient.ToString();
    }
}
