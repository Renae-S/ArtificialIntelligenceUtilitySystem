using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    /// <summary>
    /// An abstract class that represents an action of an agent. It is a ScriptableObject to allow it and derived actions to be created and made action specific.
    /// </summary>
    [CreateAssetMenu(fileName = "Action", menuName = "Action")]
    public abstract class Action : ScriptableObject
    {
        public bool withinRangeOfTarget;                // Whether the Agent is within the range of the usable or not
        public bool commitmentToAction;                 // Whether the Agent is currently performing this particular action or not

        public abstract float Evaluate(Agent agent);    // Evaluates all of the agents needs and calculates the urgency of the need with a float - a high value mean a high importance
                                                        // agent - the agent that has its needs evaluated
        public abstract void UpdateAction(Agent agent); // Updates the agents movement, needs, animation and destination
                                                        // agent - the agent that has its movement and needs updated
        public abstract void Enter(Agent agent);        // Intialises any variables in the class on entering the action
                                                        // agent - the agent that the action belongs to
        public abstract void Exit(Agent agent);         // Resets variables that were modified on exiting the action 
                                                        // agent - the agent that the action belongs to

        public virtual void SetGameObject(GameObject go) { }    // Sets the GameObject passed in as the target GameObject of an Action
    }
}