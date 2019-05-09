using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "MovementAction", menuName = "Action/MovementAction", order = 1)]
    public class MovementAction : Action
    {
        public override float Evaluate(Agent agent)
        {
            return 0;
        }

        public override void UpdateAction(Agent agent)
        {
        }

        public override void Enter(Agent agent)
        {
        }

        public override void Exit(Agent agent)
        {
        }
    }
}
