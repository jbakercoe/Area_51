using System;
using UnityEngine;

namespace Level6
{
    [Serializable]
    public class WordStructure : MonoBehaviour
    {
        #region Public variables

        [Header("WordHolder name ")] public string WordName;

        [Header("Sound that this word makes")] [Space(5f)]
        public AudioClip WordAudioClip;

        [Header("Sound Of the pronounced word")] [Space(5f)]
        public AudioClip WordPronouncedAudioClip;

        #endregion

        #region function region

        public void AddAlphabet()
        {
            gameObject.AddComponent<AlphabetModule>();
        }

        #endregion
    }
}