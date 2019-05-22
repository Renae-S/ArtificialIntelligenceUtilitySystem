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
        public Dictionary<GameObject, Action> actionsOnUseables;
        public Action[] intrinsicActions;

        // an array of all the conditions that can be met
        public Condition[] conditions;
        // an array of all the UI need bars
        public Image[] bars;
        // the action we're currently carry out
        private Action best;
        public Action currentAction;
        public Text currentActionText;

        public Dictionary<string, Image> needBars;
        public Light sun;
        private float health;
        public Image healthBar;
        float changeInHealth;

        public GameObject targetObject = null;
        public Useable targetUseable = null;

        public float actionTimerMax = 500.0f;
        public float actionTimer;
        public GameObject targetLight;

        public float maxRange = 100.0f;

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


        public NavMeshAgent nav;
        public Animator animator;


        private void Start()
        {
            actionsOnUseables = new Dictionary<GameObject, Action>();
            needBars = new Dictionary<string, Image>();
            nav = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            changeInHealth = 1;
            best = null;
            actionTimer = 0;

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
            animator.SetTrigger("Wave");
        }

        // Update is called once per frame
        void Update()
        {
            UpdateNeeds();
            UpdateHealth();
            if (targetObject != null)
            {
                if (currentAction.withinRangeOfTarget)
                {
                    if (targetObject.GetComponent<BasicAnimalAI>() != null)
                        targetObject.GetComponent<BasicAnimalAI>().turnOffMovement = true;
                }
                    
            }

            if (actionTimer <= 0)
            {
                // find the best action each frame (TODO - not every frame)
                best = GetBestAction();
                actionTimer = actionTimerMax;
            }
            actionTimer--;

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

            if (currentAction != null)
                currentActionText.text = currentAction.name;
        }

        Action GetBestAction()
        {
            // do the physics overlapsphere and check every useable around you
            // and get its UseObjectAction
            actionsOnUseables.Clear();

            Collider[] items = Physics.OverlapSphere(transform.position, maxRange);

            foreach (Collider col in items)
            {
                Useable useable = col.GetComponent<Useable>();
                if (useable)
                {
                    if (col.gameObject.tag == "Useable")
                    {
                        actionsOnUseables.Add(col.gameObject, col.gameObject.GetComponent<Useable>().action);
                    }
                }
            }

            Action bestObjectAction = null;
            Action bestIntrinsicAction = null;
            float intrinsicValue = 0;
            float objectValue = 0;

            float bestValue = 0;

            foreach (Action intrinsicAction in intrinsicActions)
            {
                intrinsicValue = intrinsicAction.Evaluate(this);
                if (intrinsicAction == null || intrinsicValue > bestValue)
                {
                    bestIntrinsicAction = intrinsicAction;
                    bestValue = intrinsicValue;
                }
            }

            foreach (KeyValuePair<GameObject, Action> a in actionsOnUseables)
            {
                objectValue = a.Value.Evaluate(this);
                if (bestObjectAction == null || objectValue > bestValue)
                {
                    bestObjectAction = a.Value;
                    bestValue = objectValue;
                    targetObject = a.Key;
                    targetUseable = targetObject.GetComponent<Useable>();
                }
            }

            
            return intrinsicValue > objectValue ? bestIntrinsicAction : bestObjectAction;
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

        void UpdateHealth()
        {
            int NumOfLowNeedBars = 0;
            foreach (Image bar in bars)
            {
                if (bar.fillAmount <= 0)
                    NumOfLowNeedBars++;
            }

            changeInHealth += (NumOfLowNeedBars * -0.005f) * Time.deltaTime;
            healthBar.fillAmount = changeInHealth;

            if (changeInHealth <= 1)
            {
                if (NumOfLowNeedBars == 0)
                    changeInHealth += 0.005f * Time.deltaTime;
            }
        }

        public float GetNeedValue(Needs n)
        {
            switch (n)
            {
                case Needs.Hydration: return hydration;
                case Needs.Nourishment: return nourishment;
                case Needs.BodyTemperature: return bodyTemperature;
                case Needs.Entertainment: return entertainment;
                case Needs.Hygiene: return hygiene;
                case Needs.Energy: return energy;
            }
            return 0;
        }

        public string GetNeedName(int n)
        {
            switch (n)
            {
                case 0: return "Hydration";
                case 1: return "Nourishment";
                case 2: return "BodyTemperature";
                case 3: return "Entertainment";
                case 4: return "Hygiene";
                case 5: return "Energy";
            }
            return "";
        }

        public Needs GetNeed(string n)
        {
            switch (n)
            {
                case "Hydration": return Needs.Hydration;
                case "Nourishment": return Needs.Nourishment;
                case "BodyTemperature": return Needs.BodyTemperature;
                case "Entertainment": return Needs.Entertainment;
                case "Hygiene": return Needs.Hygiene;
                case "Energy": return Needs.Energy;
            }
            return 0;
        }

        public Needs GetNeed(int n)
        {
            switch (n)
            {
                case 0: return Needs.Hydration;
                case 1: return Needs.Nourishment;
                case 2: return Needs.BodyTemperature;
                case 3: return Needs.Entertainment;
                case 4: return Needs.Hygiene;
                case 5: return Needs.Energy;
            }
            return 0;
        }
        public void SetNeed(Needs n, float value)
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
    }
}