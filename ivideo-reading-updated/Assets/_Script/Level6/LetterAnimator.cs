using UnityEngine;

namespace Level6
{
    /// <summary>
    /// Used to Animate the letters
    /// </summary>
    public class LetterAnimator : MonoBehaviour
    {
        /// <summary>
        /// The animator attached to the GameObject
        /// </summary>
        private Animator letterAnimator;

        /// <summary>
        /// Called when this object is enabled
        /// </summary>
        void OnEnable()
        {
            letterAnimator = GetComponent<Animator>();
        }

        public void Animate()
        {
            letterAnimator.SetBool("isWordComplete", true);
        }

        public void StopAnimation()
        {
            letterAnimator.SetBool("isWordComplete", false);
        }
    }
}
