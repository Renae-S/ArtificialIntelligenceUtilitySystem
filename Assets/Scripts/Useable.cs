using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Useable : MonoBehaviour
    {
        public Action action;       // The action the agent can do on this useable
        public float range;         // The range the agent has to be within to interact with the useable
        private TextMesh valueText; // The evaluation text above the Useable

        // Use this for initialization
        private void Start()
        {
            // Clone a copy of the action for this particular useable
            string nm = action.name;
            action = Instantiate(action);
            action.name = nm;
            valueText = GetComponentInChildren<TextMesh>();

            // Set the GameObject to this action to the GameObject this is attached to
            action.SetGameObject(gameObject);
        }

        // Sets the valueText to the value passed in
        public void updateEvaluationValue(float value)
        {
            valueText.text = value.ToString("0.00");
        }
    }
}