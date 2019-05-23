using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    public float rotation;

    private void Start()
    {
        rotation = 0.05f;
    }

    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(new Vector3(rotation, 0, 0));
    }
}
