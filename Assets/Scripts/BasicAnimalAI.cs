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
        bobbingAmplitude = 0.1f;
        turnOffMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If the agent is not within range of this animal
        if (!turnOffMovement)
        {
            // Make the animal bob up and down by changing the nav.baseOffset
            nav.baseOffset = bobbingAmplitude * Mathf.Sin(bobbingFrequency * Time.time);

            // If the animal reaches its target position, then generate a new target position
            if (GetComponent<Collider>().bounds.Contains(target))
                target = generatePosition();

            // Moves the AI towards the target with animation
            Follow();
        }
    }

    // Moves the AI towards the target with animation
    void Follow()
    {
        nav.SetDestination(target);
    }

    // Generates a random position within the playArea of the animal and returns the position
    public Vector3 generatePosition()
    {
        Vector3 pos = playArea.transform.position + new Vector3(Random.Range(-playArea.transform.localScale.x / 2, playArea.transform.localScale.x / 2), 0, 
            Random.Range(-playArea.transform.localScale.z / 2, playArea.transform.localScale.z / 2));
        return pos;
    }
}
