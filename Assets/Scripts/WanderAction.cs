using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Action", menuName = "Action/WanderAction", order = 1)]
    public class WanderAction : Action
    {
        public float radius = 100;
        public float timerLimit = 5;
        public float timer = 0;
        public Vector3 randomDirection;
        public Vector3 destinationPos;

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
                urgency = (1 - agent.GetNeedValue(need)) * Agent.Needs.GetNames(typeof(Agent.Needs)).Length;

                // Calculate recovery (need gained in ten seconds)
                foreach (Condition condition in agent.conditions)
                {
                    if (condition.CheckCondition(agent))
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
                                        if (actionCondition.multiplier < 0)
                                        {
                                            recovery = actionCondition.multiplier * 10;
                                            decrement = 0;
                                        }
                                        else if (actionCondition.multiplier > 0)
                                        {
                                            recovery = 0;
                                            decrement = actionCondition.multiplier * 10;
                                        }
                                        else
                                        {
                                            recovery = 0;
                                            decrement = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                evaluationValue += urgency * (recovery - decrement);
            }

            return evaluationValue;
        }
    

        public override void UpdateAction(Agent agent)
        {
            timer += Time.deltaTime;

            if (timer >= timerLimit)
            {
                destinationPos = RandomNavSphere(agent.transform.position, radius, -1);
                agent.nav.SetDestination(destinationPos);
                agent.nav.speed = 3.5f;
                agent.animator.SetFloat("MoveSpeed", 0.3f);
                timer = 0;
            }
        }

        public override void Enter(Agent agent)
        {
            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
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
