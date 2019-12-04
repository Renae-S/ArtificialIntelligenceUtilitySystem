using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Condition Collection", menuName = "Condition Collection")]
    public class ConditionCollection : ScriptableObject
    {
        public List<Condition> conditions;

        // Use this for initialization
        void Start()
        {
            conditions = new List<Condition>();
        }
    }
}
