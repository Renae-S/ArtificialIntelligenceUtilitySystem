using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "NeedCondition", menuName = "Condition/NeedCondition", order = 1)]
    public class NeedCondition : Condition
    {
        public string need;
        public float value;
        public bool ifAboveValue;   // If true, will check if emotion is at value or above. If false, will check if emotion is at value or below.
        private Image needBar;

        // Checks whether there is a need of the agent that is the same as this condition's need and sets the action's agent commitment accordingly, returns true if the needs 
        // are the same, false otherwise
        // agent - the agent used to check if it has a need the same as this need
        public override bool CheckCondition(Agent agent)
        {
            // If the condition's need is a need of the agent, set the bar image of this condition to the agent's
            if (agent.needBars.ContainsKey(need))
                needBar = agent.needBars[need];

            // If the need value reaches the value of this condition, then return true
            if (needBar.fillAmount <= value && !ifAboveValue)
                return true;

            // If the need value reaches the value of this condition, then return true
            if (needBar.fillAmount >= value && ifAboveValue)
                return true;

            // Otherwise return false
            return false;
        }

        // Updates the UI bars for the agent's emotions
        // agent - the agent that has its emotions and UI for needs updated
        public override void UpdateUI(Agent agent)
        {
            // For every emotion affected by this condition, adjust the changeInEmotionUI to the current value of the emotion's UI bar fill amount
            foreach (string emotion in emotionsAffected)
            {
                changeInEmotionUI = agent.emotionBars[emotion].fillAmount;
                // If the agent has an emotionBar with the string name of the emotion passed in through inspector
                if (agent.emotionBars.ContainsKey(emotion))
                {
                    changeInEmotionUI += multiplier * Time.deltaTime;           // Set value of changeEmotionUI to be the multiplier by time passed from previous frame
                    agent.emotionBars[emotion].fillAmount = changeInEmotionUI;  // Set value of the emotion bar UI fill amount to changeEmotionUI
                }
            }
        }

        // Awake allows variables to be initialised when the application begins
        public override void Awake()
        {
            changeInNeedUI = 1;
            changeInEmotionUI = 0;
        }

        // Allows for variable resetting or adjustments to condition made upon exiting the condition
        public override void Exit(Agent agent) { }
    }
}
 

