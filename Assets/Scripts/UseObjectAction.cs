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

        // Sets the GameObject passed in as the target GameObject of an Action
        public override void SetGameObject(GameObject go)
        {
            target = go;
        }

        // Evaluates all of the agents needs and calculates the urgency of the need with a float - a high value mean a high importance
        // agent - the agent that has its needs evaluated
        public override float Evaluate(Agent agent)
        {
            float finalEvaluation = 0;

            // Sum of needs urgency(i) * (recovery(i) * 10 - distance/speed * decrement(i)  
            
            // For every need of the agent
            for (int i = 0; i < Agent.Needs.GetNames(typeof(Agent.Needs)).Length; i++)
            {
                float urgency = 0;
                float recovery = 0;
                float decrement = 0;
                float evaluationValue = 0;

                // Calculate urgency

                // Get the current need
                Agent.Needs need = agent.GetNeed(i);

                // If the need value is zero or below, make the urgency a large value and increase the agent's maximum range
                if (agent.GetNeedValue(need) <= 0)
                {
                    urgency = 100000;
                    agent.maxRange = 400;
                }

                // If the value of the need is full then urgency is set to 0
                if (agent.GetNeedValue(need) >= 1)
                    urgency = 0;

                // If the value of the need neither 1 or 0, the urgency is the (1 - the value of the need) * the amount of needs the agent has
                if (agent.GetNeedValue(need) > 0 && agent.GetNeedValue(need) < 1)
                    urgency = (1 - agent.GetNeedValue(need)) * Agent.Needs.GetNames(typeof(Agent.Needs)).Length;

                // Calculate recovery and decrement (need gained in ten seconds)
                // For every condition of the agent
                foreach (Condition condition in agent.GetConditions())
                {
                    // If the condition is an action condition
                    if (condition.GetType() == typeof(ActionCondition))
                    {
                        // Create an action condition of the current condition
                        ActionCondition actionCondition = (ActionCondition)condition;
                        // If the action condition's action is the same as this action
                        if (actionCondition.action.name == this.name)
                        {
                            // For every need affected by this action
                            foreach (string needName in actionCondition.needsAffected)
                            {
                                // If this need is affected
                                if (agent.GetNeedName(i) == needName)
                                {
                                    // If the condition's multiplier is positive, calculate the recovery
                                    if (actionCondition.multiplier > 0)
                                    {
                                        recovery = actionCondition.multiplier * 10;
                                        decrement = 0;
                                    }
                                    // If the condition's multiplier is negative, calculate the decrement
                                    else if (actionCondition.multiplier < 0)
                                    {
                                        recovery = 0;
                                        decrement = actionCondition.multiplier * 10;
                                    }
                                    // If the condition's multiplier is 0, recovery and decrement is 0
                                    else
                                    {
                                        recovery = 0;
                                        decrement = 0;
                                    }
                                }
                            }
                        }
                    }
                    // Calculate recovery recieved for every time of day condition met over 10 seconds and decrement received over travel distance
                    // If the condition is an time of day condition
                    if (condition.GetType() == typeof(TimeOfDayCondition))
                    {
                        // Create an time of day condition of the current condition
                        TimeOfDayCondition timeOfDayCondition = (TimeOfDayCondition)condition;
                        // If the time of condition is currently being met
                        if (timeOfDayCondition.CheckCondition(agent))
                        {
                            // For every need affected by this condition
                            foreach (string needName in timeOfDayCondition.needsAffected)
                            {
                                // If this need is affected
                                if (agent.GetNeedName(i) == needName)
                                {
                                    // Calculate distance from agent to target
                                    distance = Vector3.Distance(agent.transform.position, target.transform.position);

                                    // If the condition's multiplier is positive, calculate the recovery
                                    if (timeOfDayCondition.multiplier > 0)
                                    {
                                        recovery = timeOfDayCondition.multiplier * 10;
                                        decrement = 0;
                                    }
                                    // If the condition's multiplier is negative, calculate the decrement
                                    if (timeOfDayCondition.multiplier < 0)
                                    {
                                        recovery = 0;
                                        decrement = timeOfDayCondition.multiplier * (distance / 15);
                                    }
                                    // If the condition's multiplier is 0, recovery and decrement is 0
                                    else
                                    {
                                        recovery = 0;
                                        decrement = 0;
                                    }
                                }
                            }
                        }
                    }
                    // Calculate evaluation value
                    evaluationValue += urgency * (recovery + decrement);
                    if (urgency == 100000 && evaluationValue < 0)
                        evaluationValue *= -1;
                }
                finalEvaluation += evaluationValue;
                if (commitmentToAction)
                    finalEvaluation += 3;
            }

            return finalEvaluation;
        }

        // Updates the agents movement, needs, animation and destination
        // agent - the agent that has its movement and needs updated
        public override void UpdateAction(Agent agent)
        {
            // If the agent is too far away, move to the target
            if (Vector3.Distance(agent.transform.position, target.transform.position) > agent.GetTargetUseable().range)
            {
                agent.nav.SetDestination(target.transform.position);
                agent.targetLight.transform.position = target.transform.position;
                agent.nav.speed = maxSpeed;
                agent.animator.SetFloat("MoveSpeed", 5.0f);

                distance = Vector3.Distance(agent.transform.position, target.transform.position);
            }
            // Otherwise play animation and get bonuses
            else
            {
                agent.transform.forward = new Vector3(target.transform.position.x - agent.transform.position.x, 0, target.transform.position.z - agent.transform.position.z);
                agent.currentAction.withinRangeOfTarget = true;
                commitmentToAction = true;
                agent.nav.velocity = Vector3.zero;

                // If the animation to be played is not an idle animation, then check if the current animation of the agent is not the animation passes in and set trigger for the animation if so
                if (animation != "Idle")
                {
                    if (!agent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName(animation))
                        agent.GetComponent<Animator>().SetTrigger(animation);
                }

                // If the agent is sleeping in the tent, rotate the agent to lay horizontally in the tent and stop its movement
                else
                {
                    agent.animator.SetFloat("MoveSpeed", 0.0f);
                    if (target.name == "Tent")
                        agent.transform.Rotate(-90, 0, 0);
                }
            }
        }

        // Intialises any variables in the class on entering the action
        // agent - the agent that the action belongs to
        public override void Enter(Agent agent)
        {
            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
            withinRangeOfTarget = false;
            maxSpeed = 15.0f;
            distance = Vector3.Distance(agent.transform.position, target.transform.position);
            commitmentToAction = false;
        }

        // Resets variables that were modified on exiting the action
        // agent - the agent that the action belongs to
        public override void Exit(Agent agent)
        {
            withinRangeOfTarget = false;
        }
    }
}
