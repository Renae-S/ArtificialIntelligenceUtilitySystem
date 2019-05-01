using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAnimalAI : MonoBehaviour {

    public GameObject target;
    private NavMeshAgent nav;
    public Rigidbody rigidbody;
    public float bobbingSpeed = 1.0f;

    // Use this for initialization
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        nav.baseOffset = Mathf.Sin(Time.deltaTime) * bobbingSpeed;

        // Moves the AI towards the target with animation
        Follow();
    }

    // Moves the AI towards the target with animation
    void Follow()
    {
        nav.SetDestination(target.transform.position);
    }
}
