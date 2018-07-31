using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Level6
{
    public enum Enviornment
    {
        EnviornmentOne = 0,
        EnviornmentTwo = 1,
        EnviornmentThree = 3,
        EnviornmentFour = 4,
        EnviornmentFive = 5
    }

    public class SoundController : MonoBehaviour
    {
        #region public fields

        [Space(5f)] [Header("Audio Source For Enviorment")]
        public AudioSource AudioSourceForEnviornment;

        [Space(5f)] [Header("Audio clips For Different Enviornment/Exercise")]
        public List<AudioClip> AudioclipForEnvironments;

        [Space(5f)] [Header("Currently Running Exercise")] [Space(10)]
        public Enviornment CurrentEnviornment;

        [Space(5f)] [Header("Lets Make WordHolder Audio clip")]
        public AudioClip LetsMakeWordAudioClip;

        [Space(5f)] [Header("What Sound Do you Here")]
        public AudioClip WhatSoundDoYouHere;

        [Space(5f)] [Header("Click on the sound That you here")]
        public AudioClip ClickOnTheSoundThatYouHere;

        [Space(10f)] [Header("Superlative Sounds")]
        public AudioClip[] Superlatives;

       

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
        private void Awake()
        {
            Reference = this;
        }

        /// <summary>
        ///     Start Unity Default.
        /// </summary>
        private void Start()
        {
            //Call init
            Init();
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
            //Set the enviornment clip .
            AudioSourceForEnviornment.clip = AudioclipForEnvironments[(int) CurrentEnviornment];
            //It will loop and play on Awake .
            AudioSourceForEnviornment.loop = true;
            AudioSourceForEnviornment.playOnAwake = true;
            //play the particular Enviornment Song .
            AudioSourceForEnviornment.Play();
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

    /// <summary>
    ///     Audio player class maintain a qeue to play the multiple  audio.
    ///     Qeue is maintained with the help of Linked List.
    /// </summary>
    public class AudioPlayer
    {
        //Audio node keep track of your Start Node.
        private AudioNode _startAudionode;

        //Check the AudioPlayer have completed it's all songs or yet to finish.
        public bool IsPlaylistPlaying;

        public AudioPlayer()
        {
            //Initial stage make it null .
            _startAudionode = null;
            //Connect to the event
            UpdatedAudioSource.AudioHasBeenPlayed += PlaySongOverList;
            //Default value is true .
            IsPlaylistPlaying = false;
        }

        /// <summary>
        ///     Create An Audio Player List.
        ///     Parameter Sequence is important .
        ///     Audio Clip will be played In same Sequence .
        /// </summary>
        public void CreatePlayList(params AudioClip[] clips)
        {
            foreach (var item in clips) AddSong(item);
        }

        /// <summary>
        ///     Delete the qeue .
        /// </summary>
        private void DeletePlaylist()
        {
            //just release the reference .
            _startAudionode = null;
        }

        /// <summary>
        ///     stop the play list .
        /// </summary>
        public void StopPlayingList()
        {
            //if start audio node is  empty
            if (_startAudionode == null) return;
            //check the song is playing or not .
            if (UpdatedAudioSource.Reference.IsAudioPlaying)
            {
                //if playing then stop it
                UpdatedAudioSource.Reference.Stop();
            }
            else
            {
                //empty the node
                DeletePlaylist();
                return;
            }

            //empty the node .
            DeletePlaylist();
        }

        /// <summary>
        ///     Add Node At the End of the list
        /// </summary>
        private void AddSong(AudioClip clip)
        {
            //Take a pointer.
            //Assign pointer rear to start node .
            var rear = _startAudionode;
            //Check start Node 
            if (_startAudionode == null)
            {
                //create a node
                var node = new AudioNode(clip);

                //Point start node to new node .
                _startAudionode = node;
                Debug.Log("First Song added Song name " + node.AudioNodeName);
            }
            else
            {
                //Iterate to the last node .
                while (rear.NextAudioNode != null) rear = rear.NextAudioNode;

                //create the node .
                var node = new AudioNode(clip);
                //pointer p now connecting the node .
                rear.NextAudioNode = node;
                Debug.Log(" Song added at the end " + node.AudioNodeName);
            }

            //Print Qeue
            //IterateOverNodes();
        }

        /// <summary>
        ///     Node will be removed only from the begining
        /// </summary>
        private void RemoveSong()
        {
            //If start node is null
            if (_startAudionode == null) throw new OperationCanceledException("No node is here to remove");

            //Take a pointer.
            //Assign pointer front to start node .
            var front = _startAudionode;
            //Get the reference of Next to Next Node .
            Debug.Log("Start node currently pointing to before deletion : " + _startAudionode.AudioNodeName);
            if (front.NextAudioNode != null)
            {
                _startAudionode = front.NextAudioNode;
                Debug.Log("Start node currently pointing to after deletion : " + _startAudionode.AudioNodeName);
            }
            else
            {
                _startAudionode = null;
                Debug.Log("Empty Play list");
                IsPlaylistPlaying = false;
            }

            //print Qeue
            //IterateOverNodes();
        }

        /// <summary>
        ///     Play the song.
        /// </summary>
        public void StartPlayingList()
        {
            //play list just started
            IsPlaylistPlaying = true;
            //if there is something to play .
            if (_startAudionode == null) return;
            //When Audio is not playing.
            if (UpdatedAudioSource.Reference.IsAudioPlaying) return;
            //Attch the clip.
            UpdatedAudioSource.Reference.AttachedAudioClip = _startAudionode.DataAudioClip;
            //play the clip.
            UpdatedAudioSource.Reference.Play();
            //return.
        }

        /// <summary>
        ///     Play song of the play list After first song is completed .
        /// </summary>
        private void PlaySongOverList(object source, AudioEventArgs audioEventArgs)
        {
            //Audio clip currently attached and just played must be same
            if (audioEventArgs.NameOfTheAudioClip != _startAudionode.DataAudioClip.name) return;
            //De attach clip.
            UpdatedAudioSource.Reference.DeAttachAudioClip(_startAudionode.DataAudioClip.name);
            //Delete first node
            RemoveSong();
            //if there is nothing to play .
            if (_startAudionode == null) return;
            //When Audio is  playing.
            if (UpdatedAudioSource.Reference.IsAudioPlaying) return;
            //Attch the clip.
            UpdatedAudioSource.Reference.AttachedAudioClip = _startAudionode.DataAudioClip;
            //play the clip.
            UpdatedAudioSource.Reference.Play();
        }

        /// <summary>
        ///     Ussed for debuging.
        /// </summary>
        private void IterateOverNodes()
        {
            var tracker = _startAudionode;
            Debug.Log("\n\n\n");
            if (tracker == null)
            {
                Debug.Log("<color=red> No Node present </color>");
                return;
            }

            while (tracker != null)
            {
                Debug.Log("<color=red>Node Name :</color> " + tracker.AudioNodeName);
                tracker = tracker.NextAudioNode;
            }

            Debug.Log("\n\n\n");
        }
    }

    /// <summary>
    ///     Node of the linked list
    /// </summary>
    public sealed class AudioNode
    {
        #region Public Field

        public readonly AudioClip DataAudioClip;
        public AudioNode NextAudioNode;
        public string AudioNodeName;

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor Derived
        /// </summary>
        /// <param name="clip"></param>
        public AudioNode(AudioClip clip)
        {
            DataAudioClip = clip;
            NextAudioNode = null;
            AudioNodeName = clip.name;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public AudioNode()
        {
            DataAudioClip = null;
            NextAudioNode = null;
        }

        #endregion
    }
}