using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// This is used when the collider is in a different gameObject and you need to check the Collider Events
    /// Create this component at runtime and subscribe to the UnityEvents
    /// </summary>
    public class TriggerProxy : MonoBehaviour
    {
        public ColliderEvent OnTrigger_Enter = new ColliderEvent();
        public ColliderEvent OnTrigger_Stay = new ColliderEvent();
        public ColliderEvent OnTrigger_Exit = new ColliderEvent();
        public CollisionEvent OnCollision_Enter = new CollisionEvent();


        void OnTriggerStay(Collider other)
        {
            OnTrigger_Stay.Invoke(other);
        }

        void OnTriggerEnter(Collider other)
        {
            OnTrigger_Enter.Invoke(other);
        }

        void OnTriggerExit(Collider other)
        {
            OnTrigger_Exit.Invoke(other);
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    OnCollision_Enter.Invoke(collision);
        //}
        //private void OnCollisionExit(Collision collision)
        //{
            
        //}
        //private void OnCollisionStay(Collision collision)
        //{
            
        //}

        private void Reset()
        {
            var collider = GetComponent<Collider>();

            if (collider)
            {
                collider.isTrigger = true;
            }
            else
            {
                Debug.LogError("This Script requires a Collider, please add any type of collider");
            }
        }
    }

    
}