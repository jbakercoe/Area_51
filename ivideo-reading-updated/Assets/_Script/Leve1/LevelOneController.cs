using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary> Level one controller. </summary>
namespace Level1
{
    /// <summary> This method is responsible for controlling level one. </summary>
    public class LevelOneController : MonoBehaviour
    {
        #region for public fields
        /// <summary> The exercise gameobject list. This list holds all exercise gameobjects. </summary>
        public List<GameObject> Exercises = new List<GameObject>();

        /// <summary> Gets the attached audio clip. </summary>
        public AudioClip TraceWatch, NowYouDoIt;

        /// <summary> The reward audio clip(s). </summary>
        public AudioClip[] RewardAudioclip = new AudioClip[5];

        /// <summary> Exercise groups. </summary>
        public enum ExerciseGroup : int
        {
            ExerciseOne = 1,
            ExerciseTwo = 2,
            ExerciseThree = 3,
            ExerciseFour = 4,
            ExerciseFive = 5,
            ExerciseSix = 6
        }

        /// <summary> The current exercise group. </summary>
        [Header("Exercise Group Information")]
        public ExerciseGroup CurrentExerciseGroup = ExerciseGroup.ExerciseOne;

        /// <summary> Gets the path of the audio clip. </summary>
        public readonly string PathForAudioAsset = "Letter/";

        /// <summary> Gets the current letter in the form of the child index. </summary>
        /// <value> The current letter. </value>
        public int CurrentLetter
        {
            get { return CurrentVisibleLetterIndex; }
            set { CurrentVisibleLetterIndex = value; }
        }
        #endregion

        #region for private fields
        /// <summary> The letter that will be visible at the start. </summary>
        [SerializeField]
        private int CurrentVisibleLetterIndex = 1;
        #endregion

        #region for unity message system        
        #endregion

        #region for functions
        #endregion
    }
}