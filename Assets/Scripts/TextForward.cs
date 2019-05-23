using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextForward : MonoBehaviour
{
    public Camera camera;
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.forward =  camera.transform.forward;
	}
}
