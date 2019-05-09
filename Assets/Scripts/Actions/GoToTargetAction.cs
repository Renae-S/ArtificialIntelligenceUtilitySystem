using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "MovementAction", menuName = "Action/MovementAction/GoToTargetAction", order = 2)]
    public class GoToTargetAction : MovementAction
    {
        public string targetTag;
        private GameObject[] targets;
        public GameObject target;
        public float maxSpeed = 15.0f;
        public float distance;
        public float targetDistance = 1000000.0f;
        public float stopDistance = 4.0f;
        public float slowingDistance = 12.0f;
        public float minSpeed = 5.0f;

        public override float Evaluate(Agent agent)
        {
            return Input.GetKey(KeyCode.P) ? 1 : 0;
        }

        public override void UpdateAction(Agent agent)
        {
            agent.nav.SetDestination(target.transform.position);
            agent.nav.speed = 15.0f;
            agent.animator.SetFloat("MoveSpeed", 5.0f);

            distance = Vector3.Distance(agent.transform.position, target.transform.position);

            if (distance <= stopDistance)
            {
                agent.transform.forward = new Vector3(target.transform.position.x - agent.transform.position.x, 0, target.transform.position.z - agent.transform.position.z);
                agent.animator.SetFloat("MoveSpeed", 0.0f);
            }

            if (agent.nav.isStopped != true && distance <= slowingDistance)
            {
                if (agent.nav.speed > minSpeed)
                    agent.nav.speed = agent.nav.speed * Time.deltaTime * maxSpeed;
                else
                    agent.nav.speed = minSpeed;

            }
        }

        public override void Enter(Agent agent)
        {
            

        }

        public override void Exit(Agent agent)
        {
        }
    }
}
