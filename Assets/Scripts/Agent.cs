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
        // An enum of needs that represent the values of the agent's needs
        public enum Needs
        {
            Hydration,
            Nourishment,
            BodyTemperature,
            Entertainment,
            Hygiene,
            Energy,
        };

        private float hydration;        // Hydration value
        private float nourishment;      // Nourishment value
        private float bodyTemperature;  // Body Temperature value
        private float entertainment;    // Entertainment value
        private float hygiene;          // Hygiene value
        private float energy;           // Energy value

        public Dictionary<GameObject, Action> actionsOnUseables;    // A dictionary of all avaliable actions on useable GameObjects
        public Action[] intrinsicActions;                           // An array of all the intrinsic actions

        public Condition[] conditions;                              // An array of all the conditions that can be met

        public Image[] bars;                                        // An array of all the UI need bars
        public Dictionary<string, Image> needBars;                  // A dictionary of all the UI need bars and their names

        private Action best;                                        // The best possible action the agent can carry out due to it's circumstances
        public Action currentAction;                                // The action we're currently carry out
        public Text currentActionText;                              // The UI text that shows the current action
        public float actionTimerMax;                                // The time in seconds between action decisions
        public float actionTimer;                                   // The timer that decreases per frame
        public GameObject targetLight;                              // A light the shines on the target for visual information

        public Light sun;                                           // The directional light representing the sun

        private float health;                                       // The value of health the agent has
        private float changeInHealth;                               // The change in health value 
        public float healthMultiplier;                              // The multiplier value that is multiplied by time between frames with to get the changeInHealth
        public Image healthBar;                                     // The UI health bar image

        public GameObject targetObject;                             // The target GameObject
        public Useable targetUseable;                               // The target Useable
        public float maxRange;                                      // The max radius around the agent for finding actions it can do on objects

        public NavMeshAgent nav;                                    // The agent's NavMeshAgent component
        public Animator animator;                                   // The agent's Animator component

        // Use this for initialization
        private void Start()
        {
            actionsOnUseables   = new Dictionary<GameObject, Action>();
            needBars            = new Dictionary<string, Image>();
            best                = null;
            currentAction       = null;
            actionTimerMax      = 10.0f;
            actionTimer         = 0;
            health              = 1;
            changeInHealth      = health;
            healthMultiplier    = 0.005f;
            targetObject        = null;
            targetUseable       = null;
            maxRange            = 100.0f;
            nav                 = GetComponent<NavMeshAgent>();
            animator            = GetComponent<Animator>();

            // For every need in Needs, add the name of the need and the bar image that represents that need's value
            for (int i = 0; i < Enum.GetNames(typeof(Needs)).Length; i++)
                needBars.Add(GetNeedName(i), bars[i]);

            // For each condition in conditions, call Awake() to initialise condition variables
            foreach (Condition condition in conditions)
                condition.Awake();

            // Wave at the start of the application to give the agent time to evaluate and act on action
            animator.SetTrigger("Wave");
        }

        // Update is called once per frame - updates the needs, UI, health, current action, target, and action timer
        void Update()
        {
            UpdateNeeds();  // Updates agent's need values including UI values
            UpdateHealth(); // Updates agent's health value including UI value

            // Check if targetObject is set to anything
            if (targetObject != null)
            {
                // If the agent is within range of the current action's target
                if (currentAction.withinRangeOfTarget)
                {
                    // If the targetObject is an animal, then turn off its movement to allow for collection of the object
                    if (targetObject.GetComponent<BasicAnimalAI>() != null)
                        targetObject.GetComponent<BasicAnimalAI>().turnOffMovement = true;
                }      
            }

            // If the actionTimer has reached 0
            if (actionTimer <= 0)
            {
                best = GetBestAction();         // Find the best action
                actionTimer = actionTimerMax;   // Reset the actionTimer
            }
            actionTimer -= Time.deltaTime;      // Reduce the actionTimer by the change in time per frame - to go down in seconds

            // If it is different from what the agent is currently doing, switch the finite state machine
            if (best != currentAction)
            {
                if (currentAction)
                    currentAction.Exit(this);
                currentAction = best;
                if (currentAction)
                    currentAction.Enter(this);
            }

            // If the currentAction is set, update the current action and the text describing the action
            if (currentAction)
            {
                currentAction.UpdateAction(this);
                currentActionText.text = currentAction.name;
            }
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
                if (a.Key.GetComponent<BasicAnimalAI>() != null)
                    a.Key.GetComponent<BasicAnimalAI>().updateEvaluationValue(objectValue);
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

        // GetNeedValue takes a string for the need name and returns the corresponding need value
        public float GetNeedValue(Needs need)
        {
            switch (need)
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

        // GetNeedValue takes a string for the need name and returns the corresponding need value
        public float GetNeedValue(string needName)
        {
            switch (needName)
            {
                case "Hydration": return hydration;
                case "Nourishment": return nourishment;
                case "BodyTemperature": return bodyTemperature;
                case "Entertainment": return entertainment;
                case "Hygiene": return hygiene;
                case "Energy": return energy;
            }
            return 0;
        }

        // GetNeedName takes an int representing the index and returns the corresponding need name
        public string GetNeedName(int index)
        {
            switch (index)
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

        // GetNeed takes a string for the need name and returns the corresponding Need
        public Needs GetNeed(string needName)
        {
            switch (needName)
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

        // GetNeed takes an int representing the index and returns the corresponding Need
        public Needs GetNeed(int index)
        {
            switch (index)
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

        // SetNeed takes a need and a value and according to which need that is passed in, its value is set to the value passed in
        public void SetNeed(Needs need, float value)
        {
            switch (need)
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