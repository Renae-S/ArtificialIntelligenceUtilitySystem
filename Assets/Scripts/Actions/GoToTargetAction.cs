using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class GoToTargetAction : Action
    {
        public override float Evaluate(Agent a)
        {
            return Input.GetKey(KeyCode.P) ? 1 : 0;
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
