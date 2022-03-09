using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KilobotSpawner : MonoBehaviour
{
    public Object kilobot;
    private int n_bots = 150;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < n_bots; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            Instantiate(kilobot, Random.insideUnitCircle * 4, rotation);
        }
    }
}
