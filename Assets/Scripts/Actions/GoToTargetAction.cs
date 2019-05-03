using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    public class GoToTargetAction : Action
    {
        public GameObject target;
        public NavMeshAgent nav;
        public Animator animator;
        public float maxSpeed = 15.0f;
        public float distance;
        public float stopDistance = 4.0f;
        public float slowingDistance = 12.0f;
        public float minSpeed = 5.0f;

        public override float Evaluate(Agent a)
        {
            return Input.GetKey(KeyCode.P) ? 1 : 0;
        }

        public override void UpdateAction(Agent agent)
        {
            nav.SetDestination(target.transform.position);
            nav.speed = 15.0f;
            animator.SetFloat("MoveSpeed", 5.0f);

            distance = Vector3.Distance(this.transform.position, target.transform.position);

            if (distance <= stopDistance)
            {
                transform.forward = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                animator.SetFloat("MoveSpeed", 0.0f);
            }   

            if (nav.isStopped != true && distance <= slowingDistance)
            {
                if (nav.speed > minSpeed)
                    nav.speed = nav.speed * Time.deltaTime * maxSpeed;
                else
                    nav.speed = minSpeed;

            }
        }

        public override void Enter(Agent agent)
        {
            nav = agent.gameObject.GetComponent<NavMeshAgent>();
            animator = agent.gameObject.GetComponent<Animator>();
            nav.stoppingDistance = stopDistance;
        }

        public override void Exit(Agent agent)
        {
        }
    }
}
