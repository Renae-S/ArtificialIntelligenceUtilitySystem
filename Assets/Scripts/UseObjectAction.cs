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
        private float distance;
        public float maxSpeed;

        GameObject target;

        public override void SetGameObject(GameObject go)
        {
            target = go;
        }

        public override float Evaluate(Agent agent)
        {
            float finalEvaluation = 0;

            // Sum of needs urgency(i) * (recovery(i) * 10 - distance/speed * decrement(i)         
            for (int i = 0; i < Agent.Needs.GetNames(typeof(Agent.Needs)).Length; i++)
            {
                float urgency = 0;
                float recovery = 0;
                float decrement = 0;
                float evaluationValue = 0;

                // Calculate urgency
                Agent.Needs need = agent.GetNeed(i);
                if (agent.GetNeedValue(need) <= 0)
                {
                    urgency = 100000;
                    agent.maxRange = 200;
                }

                // If the value of the need is full then urgency is set to 0
                if (agent.GetNeedValue(need) >= 1)
                    urgency = 0;

                // If the value of the need neither 1 or 0
                if (agent.GetNeedValue(need) > 0 && agent.GetNeedValue(need) < 1)
                    urgency = (1 - agent.GetNeedValue(need)) * Agent.Needs.GetNames(typeof(Agent.Needs)).Length;

                // Calculate recovery (need gained in ten seconds)
                foreach (Condition condition in agent.conditions)
                {
                    if (condition.GetType() == typeof(ActionCondition))
                    {
                        ActionCondition actionCondition = (ActionCondition)condition;
                        if (actionCondition.action.name == this.name)
                        {
                            foreach (string needName in actionCondition.needsAffected)
                            {
                                if (agent.GetNeedName(i) == needName)
                                {
                                    distance = Vector3.Distance(agent.transform.position, target.transform.position);
                                    if (actionCondition.multiplier > 0)
                                    {
                                        recovery = actionCondition.multiplier * 10;
                                        decrement = 0;
                                    }
                                    else if (actionCondition.multiplier < 0)
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
                    if (condition.GetType() == typeof(TimeOfDayCondition))
                    {
                        TimeOfDayCondition timeOfDayCondition = (TimeOfDayCondition)condition;

                        if (timeOfDayCondition.CheckCondition(agent))
                        {
                            foreach (string needName in timeOfDayCondition.needsAffected)
                            {
                                if (agent.GetNeedName(i) == needName)
                                {
                                    distance = Vector3.Distance(agent.transform.position, target.transform.position);
                                    if (timeOfDayCondition.multiplier > 0)
                                    {
                                        recovery = timeOfDayCondition.multiplier * 10;
                                        decrement = 0;
                                    }
                                    if (timeOfDayCondition.multiplier < 0)
                                    {
                                        recovery = 0;
                                        decrement = timeOfDayCondition.multiplier * (distance / 15);
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
                    if (commitmentToAction == true) //does nothing!!!
                        evaluationValue += 5;
                    evaluationValue = urgency * (recovery + decrement);
                }
                finalEvaluation += evaluationValue;
            }

            return finalEvaluation;
        }

        public override void UpdateAction(Agent agent)
        {
            // if the agent is too far away move to the target
            if (Vector3.Distance(agent.transform.position, target.transform.position) > agent.targetUseable.range)
            {
                agent.nav.SetDestination(target.transform.position);
                agent.targetLight.transform.position = target.transform.position;
                agent.nav.speed = maxSpeed;
                agent.animator.SetFloat("MoveSpeed", 5.0f);

                distance = Vector3.Distance(agent.transform.position, target.transform.position);
            }
            else
            {
                // otherwise play animation and get bonuses
                agent.transform.forward = new Vector3(target.transform.position.x - agent.transform.position.x, 0, target.transform.position.z - agent.transform.position.z);
                agent.currentAction.withinRangeOfTarget = true;
                commitmentToAction = true;
                agent.nav.velocity = Vector3.zero;

                if (animation != "Idle")
                {
                    if (!agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName(animation))
                        agent.GetComponent<Animator>().SetTrigger(animation);
                }

                else
                {
                    agent.animator.SetFloat("MoveSpeed", 0.0f);
                    if (target.name == "Tent")
                        agent.transform.Rotate(-90, 0, 0);
                }
            }
        }

        public override void Enter(Agent agent)
        {
            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
            withinRangeOfTarget = false;
            maxSpeed = 15.0f;
            distance = Vector3.Distance(agent.transform.position, target.transform.position);
            commitmentToAction = false;
        }

        public override void Exit(Agent agent)
        {
            withinRangeOfTarget = false;
        }
    }
}
