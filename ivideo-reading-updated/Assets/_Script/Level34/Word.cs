using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace level34
{
    /// <summary>
    /// Class WordHolder.
    /// It contains the sounds that will be played .
    /// </summary>
    [Serializable]
    public struct Word
    {
        /// <summary>
        /// The name of word .
        /// </summary>
        [SerializeField]
        public string NameOfWord;
        /// <summary>
        /// The clip.
        /// The audio for word.
        /// </summary>
        [Space(5f)]
        [SerializeField]
        public AudioClip Clip;
        /// <summary>
        /// Check audio clip has been played.
        /// </summary>
        [Space(5f)]
        [SerializeField]
        public bool IsAudioClipPlayed;
        /// <summary>
        /// Check audio clip has been spelled.
        /// </summary>
        [Space(5f)]
        [SerializeField]
        public bool IsAudioClipSpelled;
    }
}
