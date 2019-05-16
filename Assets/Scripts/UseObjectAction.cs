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
        private float maxSpeed = 15.0f;

        public override float Evaluate(Agent agent)
        {
            float urgency = 0;
            float recovery = 0;
            float decrement = 0;
            float evaluationValue = 0;

            // Sum of needs urgency(i) * (recovery(i) * 10 - distance/speed * decrement(i)         
            for (int i = 0; i < Agent.Needs.GetNames(typeof(Agent.Needs)).Length; i++)
            {
                // Calculate urgency
                Agent.Needs need = agent.GetNeed(i);
                urgency = agent.GetNeedValue(need) * Agent.Needs.GetNames(typeof(Agent.Needs)).Length;

                // Calculate recovery (need gained in ten seconds)
                foreach (Condition condition in agent.conditions)
                {
                    if (condition.GetType() == typeof(ActionCondition))
                    {
                        ActionCondition actionCondition = (ActionCondition)condition;
                        if (actionCondition.action == this)
                        {
                            foreach (string needName in actionCondition.needsAffected)
                            {
                                if (agent.GetNeedName(i) == needName)
                                {
                                    if (actionCondition.multiplier > 0)
                                    {
                                        recovery = actionCondition.multiplier * 10;
                                        decrement = 0;
                                    }
                                    else if (actionCondition.multiplier < 0)
                                    {
                                        recovery = 0;
                                        decrement = actionCondition.multiplier * (distance / maxSpeed);
                                    }
                                    else
                                    {
                                        recovery = 0;
                                        decrement = 0;
                                    }
                                    evaluationValue += urgency * (recovery - decrement);
                                }
                            }
                        }
                    }
                }
            }

            return evaluationValue;
        }

        public override void UpdateAction(Agent agent)
        {
            // if the agent is too far away move to the target
            if (Vector3.Distance(agent.transform.position, agent.targetObject.transform.position) > agent.targetUseable.range)
            {
                float slowingDistance = 8.0f;
                float minSpeed = 5.0f;
                float maxSpeed = 15.0f;

                agent.nav.SetDestination(agent.targetObject.transform.position);
                agent.targetLight.transform.position = agent.targetObject.transform.position;
                agent.nav.speed = maxSpeed;
                agent.animator.SetFloat("MoveSpeed", 5.0f);

                distance = Vector3.Distance(agent.transform.position, agent.targetObject.transform.position);

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
                // otherwise play animation and get bonuses
                agent.transform.forward = new Vector3(agent.targetObject.transform.position.x - agent.transform.position.x, 0, agent.targetObject.transform.position.z - agent.transform.position.z);
                agent.nav.speed = 0.1f;
                agent.currentAction.withinRangeOfTarget = true;

                if (animation != "Idle")
                    agent.GetComponent<Animator>().SetTrigger(animation);
                
                else
                {
                    agent.animator.SetFloat("MoveSpeed", 0.0f);
                    if (agent.targetObject.name == "Tent")
                        agent.transform.Rotate(-90, 0, 0);
                }


            }
        }

        public override void Enter(Agent agent)
        {
            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
            withinRangeOfTarget = false;
            distance = 0;
        }

        public override void Exit(Agent agent)
        {
            withinRangeOfTarget = false;
        }
    }
}
