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
        // These needs can be altered to fit the requirements to a particular application
        public enum Needs
        {
            Hydration,
            Nourishment,
            BodyTemperature,
            Entertainment,
            Hygiene,
            Energy,
        };

        // These needs can be altered to fit the requirements to a particular application
        private float hydration;        // Hydration value
        private float nourishment;      // Nourishment value
        private float bodyTemperature;  // Body Temperature value
        private float entertainment;    // Entertainment value
        private float hygiene;          // Hygiene value
        private float energy;           // Energy value

        public Dictionary<GameObject, Action> actionsOnUseables;    // A dictionary of all avaliable actions on useable GameObjects
        public Action[] intrinsicActions;                           // An array of all the intrinsic actions

        public List<ConditionCollection> conditionCollections;      // A list of all the behaviour condition that will apply to this agent
        protected List<Condition> conditions;                       // An array of all the conditions that can be met

        public Image[] nBars;                                       // An array of all the UI need bars
        public Dictionary<string, Image> needBars;                  // A dictionary of all the UI need bars and their names
        public Image[] eBars;                                       // An array of all the UI emotion bars
        public Dictionary<string, Image> emotionBars;               // A dictionary of all the UI emotion bars and their names


        private Action best;                                        // The best possible action the agent can carry out due to it's circumstances
        public Action currentAction;                                // The action we're currently carry out
        public Text currentActionText;                              // The UI text that shows the current action
        public Text currentEmotionText;                             // The UI text that shows the current emotion
        private float changeInHappiness;                            // The change in happiness value
        public float actionTimerMax;                                // The time in seconds between action decisions
        public float actionTimer;                                   // The timer that decreases per frame
        public GameObject targetLight;                              // A light the shines on the target for visual information

        public Light sun;                                           // The directional light representing the sun

        private float health;                                       // The value of health the agent has
        private float changeInHealth;                               // The change in health value 
        public float healthMultiplier;                              // The multiplier value that is multiplied by time between frames with to get the changeInHealth
        public Image healthBar;                                     // The UI health bar image

        private GameObject targetObject;                            // The target GameObject
        private Useable targetUseable;                              // The target Useable
        public float maxRange;                                      // The max radius around the agent for finding actions it can do on objects

        public NavMeshAgent nav;                                    // The agent's NavMeshAgent component
        public Animator animator;                                   // The agent's Animator component

        // Use this for initialization
        private void Start()
        {
            actionsOnUseables   = new Dictionary<GameObject, Action>();
            conditions          = new List<Condition>();
            needBars            = new Dictionary<string, Image>();
            emotionBars         = new Dictionary<string, Image>();
            best                = null;
            currentAction       = null;
            actionTimerMax      = 10.0f;
            actionTimer         = 0;
            health              = 1;
            changeInHealth      = health;
            healthMultiplier    = 0.005f;
            targetObject        = null;
            targetUseable       = null;
            nav                 = GetComponent<NavMeshAgent>();
            animator            = GetComponent<Animator>();

            // For every need in Needs, add the name of the need and the bar image that represents that need's value
            for (int i = 0; i < Enum.GetNames(typeof(Needs)).Length; i++)
                needBars.Add(GetNeedName(i), nBars[i]);

            // For every emotion in Needs, add the name of the need and the bar image that represents that need's value
            for (int i = 0; i < eBars.Length; i++)
                emotionBars.Add(eBars[i].name, eBars[i]);

            // Set up all action based conditions to Agent
            FindAllActionConditions();

            // For each condition in conditions, call Awake() to initialise condition variables
            foreach (ConditionCollection collection in conditionCollections)
            {
                foreach (Condition condition in collection.conditions)
                {
                    conditions.Add(condition);
                    condition.Awake();
                }
            }

            for (int i = 0; i < Enum.GetNames(typeof(Needs)).Length; i++)
                SetNeed(GetNeed(i), 1);

            // Wave at the start of the application to give the agent time to evaluate and act on action
            animator.SetTrigger("Wave");
        }

        // Update is called once per frame - updates the needs, UI, health, current action, target, and action timer
        void Update()
        {
            // If the actionTimer has reached 0
            if (actionTimer <= 0 || currentAction == null)
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

            if (!currentAction)
                currentActionText.text = "No Action";

            // Update current emotion text
            currentEmotionText.text = GetHighestEmotion();

            // Check if targetObject is set to anything
            if (targetObject != null)
            {
                // If the agent is within range of the current action's target
                if (currentAction && currentAction.withinRangeOfTarget)
                {
                    // If the targetObject is an animal, then turn off its movement to allow for collection of the object
                    if (targetObject.GetComponent<BasicAnimalAI>() != null)
                        targetObject.GetComponent<BasicAnimalAI>().turnOffMovement = true;
                }
            }

            UpdateNeeds();      // Updates agent's need values including UI values
            UpdateHealth();     // Updates agent's health value including UI value
            UpdateHappiness();  // Check if any negative emotions are high and apply it to the agent's happiness

        }

        // Returns the name of the highest emotion value of the agent
        string GetHighestEmotion()
        {
            Image highestEmotion = null;
            float emotionValue = 0;

            float bestValue = 0;

            // For every emotion of the agent, check it and determine the highest value emotion and return the generic name of the highest value emotion
            foreach (Image bar in eBars)
            {
                emotionValue = bar.fillAmount;
                if (highestEmotion == null || emotionValue > bestValue)
                {
                    highestEmotion = bar;
                    bestValue = emotionValue;
                }
            }

            switch (highestEmotion.name)
            {
                case ("Happiness"):
                    return "Happy";
                case ("Sadness"):
                    return "Sad";
                case ("Fear"):
                    return "Scared";
                case ("Disgust"):
                    return "Disgusted";
                case ("Anger"):
                    return "Angry";
                case ("Surprise"):
                    return "Surprised";
            }
            return "";
        }

        private void UpdateHappiness()
        {
            int NumOfLowEmotionBars = 0;

            // For each of the emotion bars of the agent
            foreach (Image bar in eBars)
            {
                // If the bar is empty, then increment the number of low need bars 
                if (bar.fillAmount <= 0)
                    NumOfLowEmotionBars++;
            }

            // Apply the changes of the agents health according to the number of low need bars multiplied by a negative multiplier and the change in time
            changeInHappiness += (NumOfLowEmotionBars * -0.05f) * Time.deltaTime;
            eBars[0].fillAmount = changeInHealth;

            // If the agent's happiness is less than full and there are no need bars that are empty, increase the health by the multiplier multiplied by the change in time
            if (changeInHappiness < 1)
            {
                if (NumOfLowEmotionBars == 0)
                    changeInHappiness += 0.05f * Time.deltaTime;
            }
        }

        // Returns the action (within the agent's radius) with the best evaluation value 
        Action GetBestAction()
        {
            // Clear the previous actions from the last decision
            actionsOnUseables.Clear();

            // Physics overlapsphere and check every useable around the agent
            Collider[] items = Physics.OverlapSphere(transform.position, maxRange);

            // For every collider hit in the agents range
            foreach (Collider col in items)
            {
                // If the GameObject is a useable
                Useable useable = col.GetComponent<Useable>();
                if (useable)
                {
                    if (col.gameObject.tag == "Useable")
                    {
                        // Add the GameObject and the action associated to the actionsOnUseables dictionary
                        actionsOnUseables.Add(col.gameObject, col.gameObject.GetComponent<Useable>().action);
                    }
                }
            }

            Action bestObjectAction = null;
            Action bestIntrinsicAction = null;
            float intrinsicValue = -10000;
            float objectValue = -10000;

            float bestIntrinsicValue = -10000;
            float bestObjectValue = -10000;

            // For every intrisic action of the agent, evaluate it and determine the highest value action and keep it set as the bestIntrinsicAction variable
            foreach (Action intrinsicAction in intrinsicActions)
            {
                intrinsicValue = intrinsicAction.Evaluate(this);
                if (intrinsicAction == null || intrinsicValue > bestIntrinsicValue)
                {
                    bestIntrinsicAction = intrinsicAction;
                    bestIntrinsicValue = intrinsicValue;
                }
            }

            if (bestIntrinsicAction == null)
                bestIntrinsicAction = intrinsicActions[0];

            // For every useable action of the agent, evaluate it and determine the highest value action and keep it set as the bestObjectAction variable
            // Set the target object to the object that carries this action
            foreach (KeyValuePair<GameObject, Action> a in actionsOnUseables)
            {
                objectValue = a.Value.Evaluate(this);
                a.Key.GetComponent<Useable>().updateEvaluationValue(objectValue);
                if (bestObjectAction == null || objectValue > bestObjectValue)
                {
                    bestObjectAction = a.Value;
                    bestObjectValue = objectValue;
                    targetObject = a.Key;
                    targetUseable = targetObject.GetComponent<Useable>();
                }
            }

            // Determine the action with the highest value out of the object and intrinsic actions, and return the action with the higher value
            return (bestIntrinsicValue > bestObjectValue ? bestIntrinsicAction : bestObjectAction);
        }

        // Checks every condition in the agent and if the condition is true, it updates the need values associated with the condition
        void UpdateNeeds()
        {
            // For every condition in the agent
            foreach (Condition condition in conditions)
            {
                // If the condition is not currently met, then exit the condition
                if (!condition.CheckCondition(this))
                    condition.Exit(this);

                // If the condition is currently met, then update the condition needs
                if (condition.CheckCondition(this))
                    condition.UpdateUI(this);
            }
        }

        // Updates the value of the agents health according to the number of need bars that are empty
        void UpdateHealth()
        {
            int NumOfLowNeedBars = 0;

            // For each of the need bars of the agent
            foreach (Image bar in nBars)
            {
                // If the bar is empty, then increment the number of low need bars 
                if (bar.fillAmount <= 0)
                    NumOfLowNeedBars++;
            }

            // Apply the changes of the agents health according to the number of low need bars multiplied by a negative multiplier and the change in time
            changeInHealth += (NumOfLowNeedBars * -0.005f) * Time.deltaTime;
            healthBar.fillAmount = changeInHealth;

            // If the agent's health is less than full and there are no need bars that are empty, increase the health by the multiplier multiplied by the change in time
            if (changeInHealth < 1)
            {
                if (NumOfLowNeedBars == 0)
                    changeInHealth += 0.005f * Time.deltaTime;
            }
        }

        // FindAllActionConditions adds all action-based conditions to the agent's conditions list - should be called once in Start
        public void FindAllActionConditions()
        {
            foreach (Action a in intrinsicActions)
            {
                foreach (Condition c in a.conditions)
                    conditions.Add(c);
            }

            // Physics overlapsphere and check every useable around the agent
            Collider[] items = Physics.OverlapSphere(transform.position, 1000);

            // For every collider hit in the agents range
            foreach (Collider col in items)
            {
                // If the GameObject is a useable
                Useable useable = col.GetComponent<Useable>();
                if (useable)
                {
                    if (col.gameObject.tag == "Useable")
                    {
                        foreach (Condition c in useable.action.conditions)
                            if (!conditions.Contains(c))
                                conditions.Add(c);
                    }
                }
            }
        }

        // GetConditions returns the list of conditions that apply to the agent
        public List<Condition> GetConditions()
        {
            return conditions;
        }

        // GetTargetUseable returns a Useable which is the current targetUseable of the agent
        public Useable GetTargetUseable()
        {
            return targetUseable;
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