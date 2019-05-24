using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicAnimalAI : MonoBehaviour {

    public Vector3 target;
    private NavMeshAgent nav;
    public float bobbingFrequency;
    public float bobbingAmplitude;
    public GameObject playArea;
    public bool turnOffMovement;

    // Use this for initialization
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        target = generatePosition();
        bobbingFrequency = 1.0f;
        bobbingAmplitude = 0.1f;
        turnOffMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!turnOffMovement)
        {
            nav.baseOffset = bobbingAmplitude * Mathf.Sin(bobbingFrequency * Time.time);

            if (GetComponent<Collider>().bounds.Contains(target))
            {
                target = generatePosition();
            }

            // Moves the AI towards the target with animation
            Follow();
        }
    }

    // Moves the AI towards the target with animation
    void Follow()
    {
        nav.SetDestination(target);
    }

    public Vector3 generatePosition()
    {
        Vector3 pos = playArea.transform.position + new Vector3(Random.Range(-playArea.transform.localScale.x / 2, playArea.transform.localScale.x / 2), 0, 
            Random.Range(-playArea.transform.localScale.z / 2, playArea.transform.localScale.z / 2));
        return pos;
    }
}
