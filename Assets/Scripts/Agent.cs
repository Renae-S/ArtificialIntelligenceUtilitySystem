using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

namespace UtilityAI
{
    public class Agent : MonoBehaviour
    {
        // an array of all avaliable actions
        public Action[] actions;
        // an array of all the conditions that can be met
        public Condition[] conditions;
        // an array of all the UI need bars
        public Image[] bars;
        // the action we're currently carry out
        public Action currentAction;
        public Text currentActionText;
        public Dictionary<string, float> needs;
        public Dictionary<string, Image> needBars;
        public Light sun;

        public enum Needs
        {
            Hydration,
            Nourishment,
            BodyTemperature,
            Entertainment,
            Hygiene,
            Energy,
        };

        private float hydration;
        private float nourishment;
        private float bodyTemperature;
        private float entertainment;
        private float hygiene;
        private float energy;

        float GetNeed(Needs n)
        {
            return needs[n.ToString()];
        }

        void SetNeed(Needs n, float value)
        {
            needs[n.ToString()] = value;
        }

        public NavMeshAgent nav;
        public Animator animator;

        private void Start()
        {
            needs = new Dictionary<string, float>();
            needBars = new Dictionary<string, Image>();
            nav = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            foreach (string needName in Enum.GetNames(typeof(Needs)))
            {
                needs.Add(needName, 1);
            }

            int num = 0;
            foreach (string needName in Enum.GetNames(typeof(Needs)))
            {
                needBars.Add(needName, bars[num]);
                num++;
            }

            foreach (Condition condition in conditions)
            {
                condition.Awake();
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateNeeds();

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

            currentActionText.text = currentAction.name;
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

        void UpdateNeeds()
        {
            foreach (Condition condition in conditions)
            {
                if (!condition.CheckCondition(this))
                    condition.Exit(this);

                if (condition.CheckCondition(this))
                    condition.UpdateNeedsUI(this);
            }
        }
    }
}