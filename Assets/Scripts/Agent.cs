using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Agent : MonoBehaviour
    {
        // an array of all avaliable actions
        Action[] actions;

        // the action we're currently carry out
        public Action currentAction;

        // Use this for initialization
        void Start()
        {
            // get all the action-derived classes that are siblings of us
            actions = GetComponents<Action>();
        }

        // Update is called once per frame
        void Update()
        {
            // find the best action each frame (TODO - not every frame)
            Action best = GetBestAction();

            // if it is different from what we were doing, switch the finite state machine
            if (best != currentAction)
            {
                if (currentAction)
                    currentAction.Exit(this);
                currentAction = best;
                if (currentAction)
                    currentAction.Enter(this);
            }

            // update the current action
            if (currentAction)
                currentAction.UpdateAction(this);
        }

        Action GetBestAction()
        {
            Action action = null;
            float bestValue = 0;

            foreach (Action a in actions)
            {
                float value = a.Evaluate(this);
                if (action == null || value > bestValue)
                {
                    action = a;
                    bestValue = value;
                }
            }
            return action;
        }
    }
}