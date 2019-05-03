using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    public class WanderAction : Action
    {
        public float radius = 100;
        public float timerLimit = 5;
        public float timer = 0;
        public Vector3 randomDirection;
        public Vector3 destinationPos;
        public NavMeshAgent nav;
        public Animator animator;
        public override float Evaluate(Agent a)
        {
            return 0.5f;
        }

        public override void UpdateAction(Agent agent)
        {
            timer += Time.deltaTime;

            if (timer >= timerLimit)
            {
                destinationPos = RandomNavSphere(transform.position, radius, -1);
                nav.SetDestination(destinationPos);
                animator.SetFloat("MoveSpeed", 0.3f);
                timer = 0;
            }
        }

        public override void Enter(Agent agent)
        {
            nav = agent.gameObject.GetComponent<NavMeshAgent>();
            animator = agent.gameObject.GetComponent<Animator>();
            timer = 0;
        }

        public override void Exit(Agent agent)
        {
        }

        public Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask)
        {
            randomDirection = Random.insideUnitSphere * distance;

            randomDirection += origin;

            UnityEngine.AI.NavMeshHit navHit;
            UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, radius, -1);

            return navHit.position;
        }
    }
}
