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
        public float changeInNeedUI;

        // Use this for initialization
        public abstract bool CheckCondition(Agent agent);
        // Update is called once per frame
        public abstract void UpdateNeedsUI(Agent agent);
        public abstract void Awake();
        public abstract void Exit(Agent agent);
    }
}

