using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject ball;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    public Vector3 size;

    // Use this for initialization
    void Start()
    {
        // Repeatedly spawn timed spheres at random positions within this object 
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    // Repeatedly spawn timed spheres at random positions within this object 
    public void SpawnObject()
    {
        Vector3 pos = transform.position + new Vector3(Random.Range(-size.x / 2, size.x /2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        Instantiate(ball, pos, transform.rotation);
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
}
