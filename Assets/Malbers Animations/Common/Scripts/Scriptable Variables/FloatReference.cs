using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MalbersAnimations.Scriptables
{
    [System.Serializable]
    public class FloatReference
    {
        public bool UseConstant = true;
        public float ConstantValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public float DefaultValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public FloatVar Variable;

        public FloatReference()
        {
            UseConstant = true;
            ConstantValue = 0;
            DefaultValue = 0;
        }

        public FloatReference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = 0;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<FloatVar>();
                Variable.Value = 0;
                Variable.DefaultValue = 0;
            }
        }

        public FloatReference(float value)
        {
            Value = value;
        }

        public float Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
            set
            {
                if (UseConstant)
                    ConstantValue = value;
                else
                    Variable.Value = value;
            }
        }

        /// <summary>
        /// Reset the current value to the Default value
        /// </summary>
        public virtual void Reset()
        {
            if (UseConstant)
            {
                Value = DefaultValue;
            }
            Value = UseConstant ? DefaultValue : Variable.DefaultValue;
        }

        #region Operators
        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}