using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAnimalAI : MonoBehaviour {

    public Vector3 target;
    private NavMeshAgent nav;
    public float bobbingFrequency= 1.0f;
    public float bobbingAmplitude= 1.0f;
    public GameObject playArea;

    // Use this for initialization
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        target = generatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        nav.baseOffset = bobbingAmplitude * Mathf.Sin(bobbingFrequency * Time.time);

        if (GetComponent<Collider>().bounds.Contains(target))
        {
            target = generatePosition();
        }
      
        // Moves the AI towards the target with animation
        Follow();
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
