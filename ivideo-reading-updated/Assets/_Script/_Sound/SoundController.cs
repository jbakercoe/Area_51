using UnityEngine;
    /// <summary>
    ///     controll the sound.
    /// </summary>
    public class SoundController : MonoBehaviour
    {
        #region public Variables

        /// <summary>
        ///     Music is playing by AudioPlayer .
        /// </summary>
        public bool IsMusicOnPlay => _reference.IsPlaylistPlaying;

        //Static reference
        public static SoundController Reference;

        #endregion

        #region Private Fields

        private AudioPlayer _reference;

        #endregion

        #region Unity functions

        /// <summary>
        ///     Awake The instance.
        /// </summary>
        public virtual void Awake()
        {
            Reference = this;
        //Call init
            Init();
    }


        /// <summary>
        ///     Start Unity Default.
        /// </summary>
        private void Start()
        {
        //Init has been moved to Awake Method .
        

        }

        #endregion

        #region Other Functions

        /// <summary>
        ///     This method initialize two Audio Source .
        ///     First Audio Source Play the Background Sound .
        ///     Second Audio Source Play the Othe sounds.
        /// </summary>
        private void Init()
        {
            //Set up the source .
            UpdatedAudioSource.Reference.AudioSourceSetting();
            //Create Audio Payer with Audio node
            var mp3Player = new AudioPlayer();
            //create the reference.
            _reference = mp3Player;
        }

        /// <summary>
        ///     Play multiple clips .
        ///     But do not call this method at Awake .
        ///     You can call this method in Start .
        /// </summary>
        /// <param name="clips"></param>
        public void Playclip(params AudioClip[] clips)
        {
            //Add songs to Audio player.
            _reference.CreatePlayList(clips);
            //Start playing the list
            _reference.StartPlayingList();
        }

        /// <summary>
        ///     Stop clip .
        /// </summary>
        public void StopClip()
        {
            _reference.StopPlayingList();
        }



        #endregion
    }