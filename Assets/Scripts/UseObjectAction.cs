using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Action", menuName = "Action/UseObjectAction", order = 1)]
    public class UseObjectAction : Action
    {
        public string animation;

        public float distance;

        public float hydration;
        public float nourishment;
        public float bodyTemperature;
        public float entertainment;
        public float hygiene;
        public float energy;

        public override float Evaluate(Agent agent)
        {
            // Sum of needs urgency(i) * (recovery(i) * 10 - distance/speed * decrement(i)         
            for (int i = 0; i < Agent.Needs.GetNames(typeof(Agent.Needs)).Length; i++)
            {
                Agent.Needs need = agent.GetNeed(i);
                // TODO!!!
            }
            return 0;
        }

        public override void UpdateAction(Agent agent)
        {
            // if the agent is too far away move to the target
            if (Vector3.Distance(agent.transform.position, agent.target.transform.position) > agent.target.range)
            {
                float slowingDistance = 12.0f;
                float minSpeed = 5.0f;
                float maxSpeed = 15.0f;

                agent.nav.SetDestination(agent.target.transform.position);
                agent.nav.speed = 15.0f;
                agent.animator.SetFloat("MoveSpeed", 5.0f);

                distance = Vector3.Distance(agent.transform.position, agent.target.transform.position);

                if (distance <= agent.nav.stoppingDistance)
                {
                    agent.transform.forward = new Vector3(agent.target.transform.position.x - agent.transform.position.x, 0, agent.target.transform.position.z - agent.transform.position.z);
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
            else
            {
                agent.currentAction.withinRangeOfTarget = true;
                // otherwise play animation and get bonuses
                agent.GetComponent<Animator>().SetTrigger("Pickup");
            }
        }

        public override void Enter(Agent agent)
        {
            float stopDistance = 4.0f;

            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
            agent.nav.stoppingDistance = stopDistance;
        }

        public override void Exit(Agent agent)
        {
            agent.currentAction.withinRangeOfTarget = false;
        }
    }
}
