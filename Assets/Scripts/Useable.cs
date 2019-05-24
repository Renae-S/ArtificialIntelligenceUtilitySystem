using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Useable : MonoBehaviour
    {
        public Action action;
        public float range;
        private TextMesh valueText;

        private void Start()
        {
            // clone our own copy of the action
            string nm = action.name;
            action = Instantiate(action);
            action.name = nm;
            valueText = GetComponentInChildren<TextMesh>();

            action.SetGameObject(gameObject);
        }

        public void updateEvaluationValue(float value)
        {
            valueText.text = value.ToString("0.00");
        }
    }
}