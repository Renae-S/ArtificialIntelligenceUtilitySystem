using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Action", menuName = "Action")]
    public abstract class Action : ScriptableObject
    {
        public abstract float Evaluate(Agent agent);
        public abstract void UpdateAction(Agent agent);
        public abstract void Enter(Agent agent);
        public abstract void Exit(Agent agent);
    }
}