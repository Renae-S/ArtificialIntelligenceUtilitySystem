using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    public float rotation;

    // Initialise default values
    private void Start()
    {
        rotation = 0.05f;
    }

    // Update is called once per frame
    // Rotates the suns on the x axis to simulate sun movement for day and night cycles
    void Update ()
    {
        transform.Rotate(new Vector3(rotation, 0, 0));
    }
}
