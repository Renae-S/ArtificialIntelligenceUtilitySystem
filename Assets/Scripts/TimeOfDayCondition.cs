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

        // Use this for initialization
        public override bool CheckCondition(Agent agent)
        {
            sun = agent.sun;

            bool isNight = false;
            if (sun.transform.eulerAngles.x <= 0.0f || sun.transform.eulerAngles.x >= 180.0f)
                isNight = true;

            bool isDay = false;
            if (sun.transform.eulerAngles.x > 0.0f && sun.transform.eulerAngles.x < 180.0f)
                isDay = true;

            if (night == isNight || day == isDay)
                return true;

            return false;
        }

        // Update is called once per frame
        public override void UpdateNeedsUI(Agent agent)
        {
            foreach (string need in needsAffected)
            {
                changeInNeedUI = agent.needBars[need].fillAmount;
                agent.SetNeed(agent.GetNeed(need), changeInNeedUI);
                if (agent.needBars.ContainsKey(need))
                {
                    changeInNeedUI += multiplier * Time.deltaTime;
                    agent.needBars[need].fillAmount = changeInNeedUI;
                    agent.SetNeed(agent.GetNeed(need), changeInNeedUI);
                }
            }
        }

        public override void Awake()
        {
            changeInNeedUI = 1;
        }

        public override void Exit(Agent agent)
        {
        }
    }
}
