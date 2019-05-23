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
        float FOV = Camera.main.fieldOfView;
        FOV += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        FOV = Mathf.Clamp(FOV, minDistance, maxDistance);
        Camera.main.fieldOfView = FOV;
    }
}
