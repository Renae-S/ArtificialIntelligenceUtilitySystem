using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    public class WanderAction : Action
    {
        public float distance = 50;
        public float timerLimit = 5;
        public float timer = 0;
        Vector3 randomDirection;
        public override float Evaluate(Agent a)
        {
            return 0.5f;
        }

        public override void UpdateAction(Agent agent)
        {
            timer += Time.deltaTime;

            if (timer >= timerLimit)
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * distance;
            }
      
            randomDirection += agent.transform.position;

            UnityEngine.AI.NavMeshHit navHit;
            UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

            agent.gameObject.GetComponent<NavMeshAgent>().SetDestination(navHit.position);
        }

        public override void Enter(Agent agent)
        {
            randomDirection = UnityEngine.Random.insideUnitSphere * distance;
            timer = 0;
        }

        public override void Exit(Agent agent)
        {
        }
    }
}
