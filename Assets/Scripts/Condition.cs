using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition")]
    public abstract class Condition : ScriptableObject
    {
        public float multiplier;
        public string[] needsAffected;
        public string[] emotionsAffected;
        public float changeInNeedUI;
        public float changeInEmotionUI;


        public abstract bool CheckCondition(Agent agent);    // Checks whether the current action of the agent is the same as this condition's action and sets the action's agent commitment accordingly, 
                                                             // returns true if the actions are the same, false otherwise
                                                             // agent - the agent used to check if current action is this action
        public abstract void UpdateUI(Agent agent);          // Updates the UI bars for the agent's needs and the actual need values if the agent is performing the action currently
                                                             // agent - the agent that has its needs and UI for needs updated
        public abstract void Awake();                        // Awake allows variables to be initialised when the application begins
        public abstract void Exit(Agent agent);              // Allows for variable resetting or adjustments to condition made upon exiting the condition
    }
}

