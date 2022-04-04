using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agents.PotentialMinimization
{
    public class PotentialMinimizationSpawner : MonoBehaviour
    {
        public GameObject kilobot;

        private static int NBots = 30;
        private static int NFixedBots = 25;
        private static float Radius = 10;

        void Start()
        {
            // Add the movement script to the kilobot
            PotentialMinimizationMovement movementScript = kilobot.AddComponent<PotentialMinimizationMovement>();
            movementScript.AddKilobotComponents(kilobot);
            
            // Spawn fixed bots
            for (int i = 0; i < NFixedBots; i++)
            {
                Vector2 position = Quaternion.AngleAxis(360 / NFixedBots * i, Vector3.forward) * Vector3.up * Radius;
                PotentialMinimizationAgent agent = new PotentialMinimizationAgent(initialState: PotentialMinimizationAgent.State.Fixed);

                GameObject newKilobot = Instantiate(kilobot, position, Quaternion.identity);
                KilobotMovement<PotentialMinimizationMessage> movement =
                    newKilobot.GetComponent<KilobotMovement<PotentialMinimizationMessage>>();
                movement.Agent = agent;
            }
            
            // Spawn moving kilobots
            for (int i = 0; i < NBots; i++)
            {
                Vector2 position = Random.insideUnitCircle * Radius * 0.9f;
                PotentialMinimizationAgent agent = new PotentialMinimizationAgent();

                GameObject newKilobot = Instantiate(kilobot, position, Quaternion.identity);
                KilobotMovement<PotentialMinimizationMessage> movement =
                    newKilobot.GetComponent<KilobotMovement<PotentialMinimizationMessage>>();
                movement.Agent = agent;
            }
            
            // Remove the initial kilobot
            Destroy(kilobot);
            
            // Set the timescale
            Time.timeScale = 10f;
        }
    }
}