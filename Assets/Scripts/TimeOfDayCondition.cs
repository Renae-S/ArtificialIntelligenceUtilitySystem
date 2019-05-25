using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "ActionCondition", menuName = "Condition/TimeOfDayCondition", order = 1)]
    public class TimeOfDayCondition : Condition
    {
        public Light sun;
        public bool night;
        public bool day;

        // Checks whether the sun is at a day rotation or night rotation, returns true if the bools set in the inspector are the same as the current time of day, false otherwise
        // agent - the agent used to access the sun
        public override bool CheckCondition(Agent agent)
        {
            sun = agent.sun;

            // If the sun's rotation is below the terrain, set isNight to true
            bool isNight = false;
            if (sun.transform.eulerAngles.x <= 0.0f || sun.transform.eulerAngles.x >= 180.0f)
                isNight = true;

            // If the sun's rotation is above the terrain, set isNight to true
            bool isDay = false;
            if (sun.transform.eulerAngles.x > 0.0f && sun.transform.eulerAngles.x < 180.0f)
                isDay = true;

            // If the night and day variables match the current time of day, then return true
            if (night == isNight || day == isDay)
                return true;

            // Otherwise return false
            return false;
        }

        // Updates the UI bars for the agent's needs and the actual need values if the agent's currently
        // agent - the agent that has its emotions and UI for needs updated
        public override void UpdateUI(Agent agent)
        {
            // For every need affected by this condition, adjust the changeInNeedUI to the current value of the need's UI bar fill amount
            foreach (string need in needsAffected)
            {
                changeInNeedUI = agent.needBars[need].fillAmount;   // Adjust the changeInNeedUI to the current value of the need's UI bar fill amount
                agent.SetNeed(agent.GetNeed(need), changeInNeedUI); // Set this need of the agent to the value of the changeInNeedUI

                // If the agent has a needBar with the string name of the need passed in through inspector
                if (agent.needBars.ContainsKey(need))
                {
                    changeInNeedUI += multiplier * Time.deltaTime;      // Set value of changeInNeedUI to be the multiplier by time passed from previous frame
                    agent.needBars[need].fillAmount = changeInNeedUI;   // Set value of the need bar UI fill amount to changeInNeedUI
                    agent.SetNeed(agent.GetNeed(need), changeInNeedUI); // Set value of need in agent to changeInNeedUI
                }
            }
        }

        // Awake allows variables to be initialised when the application begins
        public override void Awake()
        {
            changeInNeedUI = 1;
            changeInEmotionUI = 1;
        }

        // Allows for variable resetting or adjustments to condition made upon exiting the condition
        public override void Exit(Agent agent) { }
    }
}
