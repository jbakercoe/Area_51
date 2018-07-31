using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace level34
{
    /// <summary>
    /// Class Level34SoundController. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="SoundController" />
    public sealed class Level34SoundController : SoundController
    {

        /// <summary>
        /// The spell audioclip.
        /// </summary>
        public AudioClip SpellAudioclip;

        /// <summary>
        ///  spell the word.
        /// </summary>
        public AudioClip SpellThewordAudioClip;


        /// <summary>
        /// The button click sound .
        /// </summary>
        public AudioClip ButtonClickSound;



        /// <summary>
        /// The reward clip.
        /// </summary>
        public AudioClip[] RewardClip;


        /// <summary>
        /// Try again clip.
        /// </summary>
        public AudioClip TryAgain;



        /// <summary>
        /// The reference.
        /// </summary>
        public static Level34SoundController reference;

        /// <inheritdoc />
        /// <summary>
        /// Awake The instance.
        /// </summary>
        public override void Awake()
        {
            
            if(reference!=null)
            reference = this;
        }

        


    }
    
}

