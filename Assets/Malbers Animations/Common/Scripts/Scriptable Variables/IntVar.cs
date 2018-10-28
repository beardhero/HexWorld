using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>
    /// Int Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Int Var")]
    public class IntVar : ScriptableObject
    {
        //  public bool Clone = false;

        /// <summary>
        /// The current value
        /// </summary>
        [SerializeField]
        private int value = 0;
        /// <summary>
        /// The default float value to return to
        /// </summary>
        [SerializeField]
        private int defaultValue = 0;

#if UNITY_EDITOR
        [TextArea(3, 20)]
        public string Description = "";
#endif
        /// <summary>
        /// When active OnValue changed will ve used every time the value changes (you can subscribe only at runtime .?)
        /// </summary>
        public bool UseEvent = true;

        /// <summary>
        /// Invoked when the value changes
        /// </summary>
        public Events.IntEvent OnValueChanged = new Events.IntEvent();

        /// <summary>
        /// Value of the Float Scriptable variable
        /// </summary>
        public virtual int Value
        {
            get { return value; }
            set
            {
                if (this.value != value)                                //If the value is diferent change it
                {
                    this.value = value;
                    if (UseEvent) OnValueChanged.Invoke(value);         //If we are using OnChange event Invoked
                }
            }
        }

        /// <summary>
        /// The default float value to return to
        /// </summary>
        public virtual int DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// Reset the Float Value to the Default value
        /// </summary>
        public virtual void ResetValue() { Value = DefaultValue; }

        public virtual void SetValue(IntVar var)
        {
            Value = var.Value;
            DefaultValue = var.DefaultValue;
        }

        public static implicit operator int(IntVar reference)
        {
            return reference.Value;
        }
    }
}