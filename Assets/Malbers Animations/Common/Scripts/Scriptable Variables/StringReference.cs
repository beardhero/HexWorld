using UnityEngine;
namespace MalbersAnimations.Scriptables
{
    [System.Serializable]
    public class StringReference
    {
        public bool UseConstant = true;
        public string ConstantValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public string DefaultValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public StringVar Variable;

        public StringReference()
        {
            UseConstant = true;
            ConstantValue = string.Empty;
            DefaultValue = string.Empty;
        }

        public StringReference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = string.Empty;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<StringVar>();
                Variable.Value = string.Empty;
                Variable.DefaultValue = string.Empty;
            }
        }

        public StringReference(string value)
        {
            Value = value;
        }

        public string Value
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
        public static implicit operator string(StringReference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}