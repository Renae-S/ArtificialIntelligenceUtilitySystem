using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UtilityAI
{
    public class NeedCondition : MonoBehaviour
    {

        [CreateAssetMenu(fileName = "NeedCondition", menuName = "Condition/NeedCondition", order = 1)]
        public class ActionCondition : Condition
        {
            public string need;
            public float value;
            private Image needBar;

            // Use this for initialization
            public override bool CheckCondition(Agent agent)
            {

                if (agent.needBars.ContainsKey(need))
                    needBar = agent.needBars[need];

                if (needBar.fillAmount <= value)
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
}

