using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "Action", menuName = "Action/UseObjectAction", order = 1)]
    public class UseObjectAction : Action
    {
        public Useable target;
        public string animation;

        public float distance;

        public float hydration;
        public float nourishment;
        public float bodyTemperature;
        public float entertainment;
        public float hygiene;
        public float energy;

        float GetNeed(Agent.Needs n)
        {
            switch (n)
            {
                case Agent.Needs.Hydration: return hydration; 
                case Agent.Needs.Nourishment: return nourishment; 
                case Agent.Needs.BodyTemperature: return bodyTemperature; 
                case Agent.Needs.Entertainment: return entertainment; 
                case Agent.Needs.Hygiene: return hygiene; 
                case Agent.Needs.Energy: return energy; 
            }
            return 0;
        }


        void SetNeed(Agent.Needs n, float value)
        {
            switch (n)
            {
                case Agent.Needs.Hydration: hydration = value; break;
                case Agent.Needs.Nourishment: nourishment = value; break;
                case Agent.Needs.BodyTemperature: bodyTemperature = value; break;
                case Agent.Needs.Entertainment: entertainment = value; break;
                case Agent.Needs.Hygiene: hygiene = value; break;
                case Agent.Needs.Energy: energy = value; break;
            }
        }




        public override float Evaluate(Agent agent)
        {
            return 0;
        }

        public override void UpdateAction(Agent agent)
        {
            // if the agent is too far away move to the target
            if (Vector3.Distance(agent.transform.position, target.transform.position) > target.range)
            {
                float slowingDistance = 12.0f;
                float minSpeed = 5.0f;
                float maxSpeed = 15.0f;

                agent.nav.SetDestination(target.transform.position);
                agent.nav.speed = 15.0f;
                agent.animator.SetFloat("MoveSpeed", 5.0f);

                distance = Vector3.Distance(agent.transform.position, target.transform.position);

                if (distance <= agent.nav.stoppingDistance)
                {
                    agent.transform.forward = new Vector3(target.transform.position.x - agent.transform.position.x, 0, target.transform.position.z - agent.transform.position.z);
                    agent.animator.SetFloat("MoveSpeed", 0.0f);
                }

                if (agent.nav.isStopped != true && distance <= slowingDistance)
                {
                    if (agent.nav.speed > minSpeed)
                        agent.nav.speed = agent.nav.speed * Time.deltaTime * maxSpeed;
                    else
                        agent.nav.speed = minSpeed;

                }
            }
            else
            {
                // otherwise play animation and get bonuses

            }
        }

        public override void Enter(Agent agent)
        {
            float stopDistance = 4.0f;

            agent.nav = agent.gameObject.GetComponent<NavMeshAgent>();
            agent.animator = agent.gameObject.GetComponent<Animator>();
            agent.nav.stoppingDistance = stopDistance;

            float maxRange = 30.0f;
            Collider[] items = Physics.OverlapSphere(agent.transform.position, maxRange);

            foreach (Collider col in items)
            {
                Useable useable = col.GetComponent<Useable>();
                if (useable)
                {
                    if (col.gameObject.tag == target.tag)
                    {

                    }
                }
            }
        }

        public override void Exit(Agent agent)
        {
        }
    }
}
