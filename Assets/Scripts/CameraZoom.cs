using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    float minDistance;
    float maxDistance;
    public float sensitivity;

	// Use this for initialization
	void Start ()
    {
        minDistance = 15.0f;
        maxDistance = 90.0f;
        sensitivity = 10.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // If the middle mouse button scrolls in, then zoom in. Will stop zooming in at the minDistance
        // If the middle mouse button scrolls out, then zoom out. Will stop zooming out at the maxDistance
        float FOV = Camera.main.fieldOfView;
        FOV += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        FOV = Mathf.Clamp(FOV, minDistance, maxDistance);
        Camera.main.fieldOfView = FOV;
    }
}
