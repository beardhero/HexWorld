using UnityEngine;
namespace MalbersAnimations.Scriptables
{
    [System.Serializable]
    public class Vector3Reference
    {
        public bool UseConstant = true;
        public Vector3 ConstantValue = Vector3.zero;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public Vector3 DefaultValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public Vector3Var Variable;

        public Vector3Reference()
        {
            UseConstant = true;
            ConstantValue = Vector3.zero;
            DefaultValue = Vector3.zero;
        }

        public Vector3Reference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = Vector3.zero;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<Vector3Var>();
                Variable.Value =  Vector3.zero;
                Variable.DefaultValue = Vector3.zero;
            }
        }

        public Vector3Reference(Vector3 value)
        {
            Value = value;
        }

        public Vector3 Value
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
        public static implicit operator Vector3(Vector3Reference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}
