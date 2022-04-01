using Agents;
using UnityEngine;

public class KilobotSpawner : MonoBehaviour
{
    public GameObject kilobot;
    public Collider2D targetShape;
    private const int NRows = 4;
    private const int NColumns = 7;
    private const float InitialSpacing = 1.5f;
    
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
            // Create a new agent at the specified location
            Vector2 position = new Vector2(fixedPositions[i, 0], fixedPositions[i, 1]);
            ShapeAssemblyAgent agent = InitializeKilobot(position, ShapeAssemblyAgent.State.JoinedShape);
            
            // Set the kilobot as part of the coordinate system
            agent.PositionEstimate = position;
            agent.PositionSeed = true;

            if (i == 0)
            {
                agent.GradientSeed = true;
            }
        }
        
        // Spawn other Kilobots
        Vector2 spawnPosition = new Vector2(0, -InitialSpacing);
        for (int i = 0; i < NColumns; i++)
        {   
            for (int j = 0; j < NRows; j++)
            {
                Vector2 offset = new Vector2(i * InitialSpacing, -j * InitialSpacing);
                InitializeKilobot(spawnPosition + offset);
            }
        }
        
        // Set the time scale
        Time.timeScale = 10f;
        
        // Remove the initial kilobot
        Destroy(kilobot);
    }

    private ShapeAssemblyAgent InitializeKilobot(Vector2 position, ShapeAssemblyAgent.State initialState = ShapeAssemblyAgent.State.Start)
    {
        // Create a new Kilobot at the specified position
        GameObject newKilobot = Instantiate(kilobot, position, Quaternion.identity);
        
        // Assign the agent
        ShapeAssemblyAgent agent = new ShapeAssemblyAgent(initialState);
        KilobotMovement kilobotMovement = newKilobot.GetComponent<KilobotMovement>();
        kilobotMovement.Agent = agent;
        
        // Set the target shape
        agent.TargetShape = targetShape;
        return agent;
    }
}
