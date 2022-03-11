using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KilobotSpawner : MonoBehaviour
{
    public Object kilobot;
    private int n_bots = 25;
    
    // Start is called before the first frame update
    void Start()
    {
        // Spawn moving Kilobots
        for (int i = 0; i < n_bots; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            Instantiate(kilobot, Random.insideUnitCircle * 4, rotation);
        }
        
        // Spawn fixed Kilobot
        GameObject fixedKilobot = Instantiate(kilobot, Vector3.zero, Quaternion.identity) as GameObject;
    }
}
