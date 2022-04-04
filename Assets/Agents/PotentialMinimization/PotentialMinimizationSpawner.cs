using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents.PotentialMinimization
{
    public class PotentialMinimizationSpawner : MonoBehaviour
    {
        public GameObject kilobot;

        private static int NBots = 30;
        private static float Radius = 10;

        void Start()
        {
            // Add the movement script to the kilobot
            PotentialMinimizationMovement movementScript = kilobot.AddComponent<PotentialMinimizationMovement>();
            movementScript.AddKilobotComponents(kilobot);
            
            // Spawn kilobots
            for (int i = 0; i < NBots; i++)
            {
                Vector2 position = Random.insideUnitCircle * Radius;
                PotentialMinimizationAgent agent = new PotentialMinimizationAgent();

                GameObject newKilobot = Instantiate(kilobot, position, Quaternion.identity);
                KilobotMovement<PotentialMinimizationMessage> movement =
                    newKilobot.GetComponent<KilobotMovement<PotentialMinimizationMessage>>();
                movement.Agent = agent;
            }
            
            // Remove the initial kilobot
            Destroy(kilobot);
        }
    }
}