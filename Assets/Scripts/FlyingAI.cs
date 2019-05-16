using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAI : MonoBehaviour {

    public Vector3 target;
    public float targDistance;
    public GameObject playArea;
    public float speed = 50.0f;

    // Use this for initialization
    void Start () {
        target = generatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        targDistance = Vector3.Distance(new Vector3(this.transform.position.x, 0, this.transform.position.z), new Vector3(target.x, 0, target.y));

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
        // Gets a vector that points from the bird/butterfly's position to the target's.
        var heading = target - transform.position;

        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.

        transform.forward = direction;
        transform.position += (direction * speed * Time.deltaTime);
    }

    public Vector3 generatePosition()
    {
        Vector3 pos = playArea.transform.position + new Vector3(Random.Range(-playArea.transform.localScale.x / 2, playArea.transform.localScale.x / 2), 0,
            Random.Range(-playArea.transform.localScale.z / 2, playArea.transform.localScale.z / 2));
        return pos;
    }
}
