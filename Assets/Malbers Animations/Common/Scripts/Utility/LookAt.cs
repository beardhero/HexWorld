﻿using UnityEngine;
using System;

namespace MalbersAnimations.Utilities
{
    [Serializable]
    public class BoneRotation
    {
        public Transform bone;                                          //The bone
        public Vector3 offset = new Vector3(0, -90, -90);               //The offset for the look At
        [Range(0, 1)]
        public float weight = 1;                                        //the Weight of the look a
        internal Quaternion initialRotation;
    }

    

    public class LookAt : MonoBehaviour, IAnimatorListener       //This is for sending messages from the animator
    {
        [Tooltip("Global LookAt Activation")]
        [SerializeField]
        private bool active = true;                      //For Activating and Deactivating the HeadTrack
        [Tooltip("The Animations allows the LookAt to be enable/disabled")]
        public bool AnimationActive = true;             //Means the Animator has deactivate it

        public bool UseCamera;                          //Use the camera ForwardDirection instead a Target
        public Transform Target;                        //Use a target to look At

        [Space]
        public float LimitAngle = 80f;                  //Max Angle to LookAt
        public float Smoothness = 5f;                   //Smoothness between Enabled and Disable
        public Vector3 UpVector = Vector3.up;


        float currentSmoothness;

        [Space]
        public BoneRotation[] Bones;                    //Bone Chain   

        private Transform cam;                          //Reference for the camera
        protected float angle;                          //Angle created between the transform.Forward and the LookAt Point    
        protected Vector3 direction;
        public bool debug = true;
        bool hasTarget;

        private RaycastHit aimHit;

        public Vector3 Direction
        {
            set { direction = value; }
            get { return direction; }
        }

        /// <summary>
        /// Check if is on the Angle of Aiming
        /// </summary>
        public bool IsAiming
        {
            get { return angle < LimitAngle && Active && AnimationActive && hasTarget; }
        }

        /// <summary>
        /// Last Raycast stored for calculating the Aim
        /// </summary>
        public RaycastHit AimHit
        {
            get { return aimHit; }
            set { aimHit = value; }
        }

        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }

        bool AnimatorOnAnimatePhysics;

        void Awake()
        {
            if (Camera.main != null) cam = Camera.main.transform;               //Get the main camera

            var Anim = GetComponent<Animator>();

            AnimatorOnAnimatePhysics = (Anim && Anim.updateMode == AnimatorUpdateMode.AnimatePhysics); //Check if the animator is on Animate Physics

            if (AnimatorOnAnimatePhysics) return;

            foreach (var bone in Bones)                                         //Stores the Initial Rotation
            {
                bone.initialRotation = bone.bone.transform.localRotation;
            }
        }


        void LateUpdate()
        {
            foreach (var bone in Bones)                                         //Stores the Initial Rotation
            {
                bone.initialRotation = bone.bone.transform.localRotation;
            }
            LookAtBoneSet();            //Rotate the bones
        }

        /// <summary>
        /// Enable or Disable this script functionality by the Animator
        /// </summary>
        public void EnableLookAt(bool value)
        {
            AnimationActive = value;
        }

        /// <summary>
        /// Rotates the bones to the Look direction (Only works when the animator is set to Update Mode : Normal)
        /// </summary>
        void LookAtBoneSet()
        {
            hasTarget = false;
            if (UseCamera || Target) hasTarget = true;


            angle = Vector3.Angle(transform.forward, direction);                                                    //Find the angle for the current bone


            currentSmoothness = Mathf.Lerp(currentSmoothness, IsAiming ? 1 : 0, Time.deltaTime * Smoothness);

            if (currentSmoothness > 0.9999f) currentSmoothness = 1;
            if (currentSmoothness < 0.0001f) currentSmoothness = 0;

            for (int i = 0; i < Bones.Length; i++)
            {
                var bone = Bones[i];

                if (!bone.bone) continue;   //if There's no bone skip

                Vector3 dir = transform.forward;


                if (UseCamera)
                {
                    dir = cam.forward;

                    aimHit = MalbersTools.RayCastHitToCenter(bone.bone);        //Calculate the Direction from the Bone

                    if (aimHit.collider)                                        //if something was hit 
                    {
                        dir = MalbersTools.DirectionTarget(bone.bone.position, aimHit.point);
                    }
                }

                if (Target) dir = MalbersTools.DirectionTarget(bone.bone, Target);

                direction = Vector3.Lerp(direction, dir, Time.deltaTime * Smoothness);

                if (currentSmoothness == 0) return;                                         //Skip all next stuffs


                if (debug && i == Bones.Length - 1)
                {
                    Debug.DrawRay(bone.bone.position, direction * 15, Color.green);
                }


                var final = Quaternion.LookRotation(direction, UpVector) * Quaternion.Euler(bone.offset);
                var next = Quaternion.Lerp(bone.bone.rotation, final, bone.weight * currentSmoothness);
                bone.bone.rotation = next;
            }
        }


        /// <summary>
        /// Set the Target to Null
        /// </summary>
        public virtual void NoTarget()
        {
            Target = null;
        }

        /// <summary>
        /// This is used to listen the Animator asociated to this gameObject
        /// </summary>
        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }

    }
}
















