using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KilobotSpawner : MonoBehaviour
{
    public Object kilobot;
    private const int NBots = 10;
    private const float InitialSpacing = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawn fixed Kilobots
        float[,] fixedPositions =
        {
            {0, 0},
            {-InitialSpacing, InitialSpacing},
            {InitialSpacing, InitialSpacing},
            {0, 2 * InitialSpacing}
        };
        for (int i = 0; i < fixedPositions.GetLength(0); i++)
        {
            // Create the kilobot at the specified location
            Vector2 position = new Vector2(fixedPositions[i, 0], fixedPositions[i, 1]);
            GameObject fixedKilobot = Instantiate(kilobot, position, Quaternion.identity) as GameObject;
            KilobotAgent agent = fixedKilobot.GetComponent<KilobotMovement>().Agent;
            
            // Set the kilobot as final
            agent.CurrentState = KilobotAgent.State.JoinedShape;
            agent.PositionEstimate = position;
            agent.PositionSeed = true;

            if (i == 0)
            {
                agent.GradientSeed = true;
            }
        }
        
        // Spawn other Kilobots
        for (int i = 0; i < NBots; i++)
        {
            Instantiate(kilobot, Random.insideUnitCircle * 5f, Quaternion.identity);
        }
    }
}
