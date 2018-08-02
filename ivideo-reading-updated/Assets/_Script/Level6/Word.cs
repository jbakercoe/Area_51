using UnityEngine;

namespace Level6
{
    /// <summary>
    /// Holds information about a word.
    /// </summary>
    [System.Serializable]
    public struct Word
    {
        /// <summary>
        /// The letters in this word.
        /// </summary>
        public Letter[] letters;
        /// <summary>
        /// The audio clips for this word.
        /// </summary>
        public AudioClip[] clips;
    }
}
