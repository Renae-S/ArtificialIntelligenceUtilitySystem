using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class EvaluationToggle : MonoBehaviour
    {
        private List<GameObject> evalNumbers;   // List used to store all evaluation values

        // Use this for initialization
        void Start()
        {
            evalNumbers = new List<GameObject>();

            // Physics overlapsphere and check every useable around the centre point
            Collider[] items = Physics.OverlapSphere(transform.position, 500);

            // For every collider hit in the centre point's range
            foreach (Collider col in items)
            {
                // If the GameObject is a evaluation text
                Useable useable = col.GetComponent<Useable>();
                if (useable)
                {
                    TextMesh text = col.gameObject.GetComponentInChildren<TextMesh>();
                    if (text.gameObject.tag == "Text")
                    {
                        // Add the GameObject and the evaluation text to the list
                        evalNumbers.Add(text.gameObject);
                    }
                }
            }
        }

        // Toggles the evaluation text in the list either on or off
        public void Toggle()
        {
            foreach (GameObject go in evalNumbers)
            {
                go.SetActive(!go.activeSelf);
            }
        }
    }
}
