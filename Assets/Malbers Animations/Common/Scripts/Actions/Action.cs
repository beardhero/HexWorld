using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    [CreateAssetMenu(menuName = "Malbers Animations/Actions")]
    public class Action : ScriptableObject
    {
        [Tooltip("Value for the transitions IDAction on the Animator in order to Execute the desirable animation clip")]
        public int ID;

        public static implicit operator int(Action reference)
        {
            return reference.ID;
        }
    }


    //public struct ActionSet
    //{
    //    public enum ActionInterruption { None, Movement, Time, Release }
    //    /// <summary>
    //    /// Name of the ActionEmotion
    //    /// </summary>
    //    public string name;
    //    public int ID;
    //    public ActionInterruption interruption;
    //    public float time;
    //    public float loops;
    //}
}