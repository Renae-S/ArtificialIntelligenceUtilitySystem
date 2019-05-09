using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    public class Agent : MonoBehaviour
    {
        // an array of all avaliable actions
        public Action[] actions;
        // the action we're currently carry out
        public Action currentAction;

        public enum Needs
        {
            Hydration,
            Nourishment,
            BodyTemperature,
            Entertainment,
            Hygiene,
            Energy,
        };

        public float hydration;
        public float nourishment;
        public float bodyTemperature;
        public float entertainment;
        public float hygiene;
        public float energy;

        float GetNeed(Needs n)
        {
            switch (n)
            {
                case Needs.Hydration: return hydration; break;
                case Needs.Nourishment: return nourishment; break;
                case Needs.BodyTemperature: return bodyTemperature; break;
                case Needs.Entertainment: return entertainment; break;
                case Needs.Hygiene: return hygiene; break;
                case Needs.Energy: return energy; break;
            }
            return 0;
        }


        void SetNeed(Needs n, float value)
        {
            switch (n)
            {
                case Needs.Hydration: hydration = value; break;
                case Needs.Nourishment: nourishment = value; break;
                case Needs.BodyTemperature: bodyTemperature = value; break;
                case Needs.Entertainment: entertainment = value; break;
                case Needs.Hygiene: hygiene = value; break;
                case Needs.Energy: energy = value; break;
            }
        }

        public NavMeshAgent nav;
        public Animator animator;

        private void Start()
        {
            nav = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
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