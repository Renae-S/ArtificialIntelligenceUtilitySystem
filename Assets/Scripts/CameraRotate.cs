using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {

    public Transform target;
    public float speed = 1;
    public float distance;

    float lastMouseX;
    float lastMouseY;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        // Rotate 
        if (Input.GetMouseButton(1))
        {
            float deltaX = Input.mousePosition.x - lastMouseX;
            float deltaY = Input.mousePosition.y - lastMouseY;

            Vector3 angles = transform.eulerAngles;
            angles.x += deltaY * Time.deltaTime * speed;

            if (angles.x > 180)
                angles.x -= 360;
            angles.x = Mathf.Clamp(angles.x, -70, 70);

            angles.y = angles.y + deltaX * Time.deltaTime * speed;
            transform.eulerAngles = angles;
        }
        transform.position = target.position - distance * transform.forward;

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;

        // Zoom
	}
}