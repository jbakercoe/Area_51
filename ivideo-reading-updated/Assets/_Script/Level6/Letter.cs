using UnityEngine;

namespace Level6
{
    /// <summary>
    /// Holds information about a letter.
    /// </summary>
    [System.Serializable]
    public struct Letter
    {
        /// <summary>
        /// The gameobject for this letter
        /// </summary>
        public GameObject letter;
        /// <summary>
        /// The audio clip for this letter.
        /// </summary>
        public AudioClip clip;
    }
}
