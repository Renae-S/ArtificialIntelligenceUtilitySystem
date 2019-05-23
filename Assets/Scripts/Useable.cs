using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class Useable : MonoBehaviour
    {
        public Action action;
        public float range;

        private void Start()
        {
            // clone our own copy of the action
            string nm = action.name;
            action = Instantiate(action);
            action.name = nm;

            action.SetGameObject(gameObject);
        }
    }
}