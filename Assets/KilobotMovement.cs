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

    private Dictionary<StateEnum, Color> stateColor = new Dictionary<StateEnum, Color>()
    {
        {StateEnum.Movement, Color.red},
        {StateEnum.FixedPosition, Color.green}
    };

    private static float forwardForce = 1f;
    private static float communicationRadius = 1f;
    private static float stickRadius = 0.25f;


    // Start is called before the first frame update
    void Start()
    {
        
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
            StateEnum kilobotState = hit.gameObject.GetComponent<KilobotMovement>().state;
            
            kilobotInfo.Add((distance, kilobotState, position));
        }
        
        // State specific behaviour
        switch (state)
        {
            case StateEnum.Movement:
                if (kilobotInfo.Any())
                {
                    foreach (var kilobot in kilobotInfo)
                    {
                        if (kilobot.state == StateEnum.FixedPosition)
                        {
                            if (kilobot.distance < stickRadius)
                            {
                                state = StateEnum.FixedPosition;
                                return;
                            }
                            rb.AddForce(kilobot.position.normalized * forwardForce);
                        }
                    }
                }
                rb.AddForce(Random.insideUnitCircle * forwardForce);
                break;
            case StateEnum.FixedPosition:
                break;
        }
    }

    void UpdateColor()
    {
        sr.color = stateColor[state];
    }
}
