using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Action", menuName = "Action/WanderAction", order = 1)]
    public class WanderAction : Action
    {
        public float radius;
        public float timerLimit;
        public float timer;
        public Vector3 randomDirection;
        public Vector3 destinationPos;

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

                // If the need value is zero or below, make the urgency a large value
                if (agent.GetNeedValue(need) <= 0)
                    urgency = 100000;

                // If the value of the need is full then urgency is set to 0
                if (agent.GetNeedValue(need) >= 1)
                    urgency = 0;

                // If the value of the need neither 1 or 0, the urgency is the (1 - the value of the need) * the amount of needs the agent has
                if (agent.GetNeedValue(need) > 0 && agent.GetNeedValue(need) < 1)
                    urgency = (1 - agent.GetNeedValue(need)) * Agent.Needs.GetNames(typeof(Agent.Needs)).Length;

                // Calculate recovery and decrement (need gained in ten seconds)
                // For every condition of the agent
                foreach (Condition condition in agent.conditions)
                {
                    // If the condition is an action condition
                    if (condition.GetType() == typeof(ActionCondition))
                    {
                        // Create an action condition of the current condition
                        ActionCondition actionCondition = (ActionCondition)condition;
                        // If the action condition's action is the same as this action
                        if (actionCondition.action == this)
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
                                        decrement = timeOfDayCondition.multiplier * 10;
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
                }
                finalEvaluation += evaluationValue;
            }

            return finalEvaluation;
        }

        // Updates the agents movement, needs, animation and destination
        // agent - the agent that has its movement and needs updated
        public override void UpdateAction(Agent agent)
        {
            withinRangeOfTarget = true;

            if (timer >= timerLimit)
            {
                destinationPos = RandomNavSphere(agent.transform.position, radius, -1);
                agent.nav.SetDestination(destinationPos);
                agent.nav.speed = 3.5f;
                agent.animator.SetFloat("MoveSpeed", 0.3f);
                timer = 0;
            }
            timer += Time.deltaTime;
        }

        // Intialises any variables in the class on entering the action
        // agent - the agent that the action belongs to
        public override void Enter(Agent agent)
        {
            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
            timerLimit = 5;
            timer = timerLimit;
            radius = 100;
        }

        // Resets variables that were modified on exiting the action
        // agent - the agent that the action belongs to
        public override void Exit(Agent agent)
        {
            withinRangeOfTarget = false;
        }

        // Returns a random point on the NavMesh within a given distance to the origin.
        // origin - AI agent position
        // distance - a maximum distance the agent will move between finding a new position to move to 
        // layerMask - typically -1 for all layers
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
