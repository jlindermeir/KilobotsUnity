using Agents;
using UnityEngine;

public class KilobotSpawner : MonoBehaviour
{
    public GameObject kilobot;
    public Collider2D targetShape;
    private const int NRows = 2;
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
            ShapeAssemblyAgent agent = new ShapeAssemblyAgent(targetShape, position, true, i == 0);
            InitializeKilobot(agent, position);
        }
        
        // Spawn other Kilobots
        Vector2 spawnPosition = new Vector2(0, -InitialSpacing);
        for (int i = 0; i < NColumns; i++)
        {   
            for (int j = 0; j < NRows; j++)
            {
                Vector2 position = spawnPosition + new Vector2(i * InitialSpacing, -j * InitialSpacing);
                ShapeAssemblyAgent agent = new ShapeAssemblyAgent(targetShape, Vector2.zero);
                InitializeKilobot(agent, position);
            }
        }
        
        // Set the time scale
        Time.timeScale = 10f;
        
        // Remove the initial kilobot
        Destroy(kilobot);
    }

    private void InitializeKilobot(ShapeAssemblyAgent agent, Vector2 position)
    {
        // Create a new Kilobot at the specified position
        GameObject newKilobot = Instantiate(kilobot, position, Quaternion.identity);
        
        // Assign the agent
        KilobotMovement kilobotMovement = newKilobot.GetComponent<KilobotMovement>();
        kilobotMovement.Agent = agent;
    }
}
