using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;
using Object = UnityEngine.Object ;

namespace LevelEleven
{
    [AddComponentMenu("LevelEleven_Reezoo/WordManager")]
    public class WordManager : MonoBehaviour
    {
        #region public variables and properties

        //Hold Base letters 
        public List<Letter> BaseletterList;

        [Header("WordHolder Settings :")] [Range(300f, 600f), Tooltip("Distance between letters"), Space(10)]
        public float LetterSpace = 400f;

        [Range(10f, 50f), Tooltip("Distace traveled by letter in single frame"), Space(10)]
        public float LetterDistance = 20f;

        [Range(0f, 5f), Tooltip("How frequent they move"), Space(10)]
        public float LetterSpeed = 0.5f;

        [Range(0f, 20f), Tooltip("How intimate the letters when they come togather . Do not mess with this Variable ."),
         Space(10)]
        public float CoupleLetterSpacing = 0f;

        [Tooltip("Audio clip for current clip "), Space(20)]
        public AudioClip CurrentGroupAudioClip;

        [Tooltip("Audio clip Now you say it "), Space(20)]
        public AudioClip NowYouSayit;

        
        /// <summary>
        /// Get reward Audio at runtime.
        /// </summary>
        private AudioClip GetSuperLativeAudioClip
        {
            get
            {
                //Pool all reward audio clip as object 
                Object[] poolAudioClips = Resources.LoadAll(_audioLoadingPath, typeof(AudioClip));
                //Find out random audio clip
                var rand = new Random(new System.DateTime().Millisecond); //Seed
                var index = rand.Next(0, poolAudioClips.Length - 1);
                return ((AudioClip) poolAudioClips[index]);
            }
        }

        #endregion

        #region private variables

        private Action _onLetterCollideEachOther;
        private Action _onGroupSoundPlayingComplete;
        private Animator _animator;
        private readonly string _audioLoadingPath = "EggGame/Superlative/";

        #endregion

        #region unity Function

        private void Awake()
        {
            //Iterate over the letters and make sure all letters all leters are deactive in heirarchy
            if (BaseletterList != null)
            {
                foreach (var item in BaseletterList)
                {
                    if (item.AttachedGameObject.activeInHierarchy) item.AttachedGameObject.SetActive(false);
                }
            }
            else
            {
                throw new NullReferenceException("Add letters to the list");
            }

            //Set the animator 
            _animator = gameObject.GetComponent<Animator>();
            //Make it diable
            _animator.enabled = false;
           
        }

        private void OnEnable()
        {
            //Attach the Actions.
            _onGroupSoundPlayingComplete = () =>
            {
                //play the sound Now you say it.
                gameObject.GetComponent<AudioSource>().clip = NowYouSayit;
                //TODO: "Now you say it" sound is going to play Here . Pass through proper Audio Mixer .
                gameObject.GetComponent<AudioSource>().Play();
                _animator.enabled = true;
                //wait for the audio to end and wait certain time(1.5f) to play the rewars audio.
                PlayRewardAudio(withdelay:1.5f); 

                
            };

            //When letter collide each other play the sound of the group 3 times .
            _onLetterCollideEachOther = () =>
            {
                //PlayGroupSound the group sound Three times.
                IEnumerator playEnumerator = PlayGroupSound(numberOfTime:3);
                StartCoroutine(playEnumerator);
            };
        }

        public void Start()
        {
            //Initilize the Base letter .
            InitializeBaseLetter(BaseletterList[0], BaseletterList[1]);
        }

        #endregion

        #region Normal Function

        /// <summary>
        /// Initialize Any number of Base letter .
        /// </summary>
        /// <param name="baseletterList"></param>
        private void InitializeBaseLetter(params Letter[] baseletterList)
        {
            int letterNum = 0;

            //Hide the letter
            baseletterList[letterNum].Hide();
            //Activate game object
            if (!baseletterList[letterNum].AttachedGameObject.activeInHierarchy)
                baseletterList[letterNum].SetActive(true);
            //Appear the letter over frame.
            baseletterList[letterNum].MakeletterAppear();
            //PlayGroupSound the sound of the letter 3 time .
            var letter = baseletterList[0];
            baseletterList[letterNum].OnLetterAppear = () =>
            {
                //PlayGroupSound the sound three times
                var plyingcCurrentLetter = letter.PlayTheSoundOfCurrentLetter(3);
                StartCoroutine(plyingcCurrentLetter);
            };
            //Disappear the method once it is shown and sound part was done
            baseletterList[letterNum].OnMultipleSoundPlayComplete = baseletterList[letterNum].MakeletterDisapper;
            //Make letter disappear .
            var templetter = baseletterList[letterNum];
            baseletterList[letterNum].OnLetterDisappear = () =>
            {
                //Deactivate the letter 
                templetter.SetActive(false);
                //increment the letter index
                letterNum++;
                //Check Other letter present Or not
                if (letterNum <= baseletterList.Length - 1)
                {
                    //Initiate Other Base letters .
                    //Recursion....call
                    InitializeBaseLetter(baseletterList[letterNum]);
                }
                else
                {
                    //Letter shown complete .

                    //Bring letter close to each other 
                    BringLettersCloser(BaseletterList[0], BaseletterList[1]);
                    return;
                }
            };
        }

        private void BringLettersCloser(params Letter[] baseletterList)
        {
            if (baseletterList.Length > 2)
            {
                throw new Exception("Unable to handle more than two letters ");
            }
            else
            {
                //First Make a difference between Two letters .
                var distancefromCenter = LetterSpace / 2;
                //Moving to the left/Right
                baseletterList[0].AttachedGameObject.transform.localPosition = new Vector3(-distancefromCenter, 0f, 0f);
                //Moving to the left/Right
                baseletterList[1].AttachedGameObject.transform.localPosition = new Vector3(+distancefromCenter, 0f, 0f);

                //if letter alpha is zero you should make it one to make it visible
                baseletterList[0].Show();
                baseletterList[1].Show();

                //Activate both of them
                baseletterList[0].AttachedGameObject.SetActive(true);
                baseletterList[1].AttachedGameObject.SetActive(true);

                //move the letter 
                IEnumerator momveletterEnumerator =
                    MoveLetter(LetterDistance, LetterSpeed, baseletterList[0], baseletterList[1]);
                StartCoroutine(momveletterEnumerator);
            }
        }

        private IEnumerator MoveLetter(float distance, float time, params Letter[] baseletterList)
        {
            //bool that will toogle
            var toogle = true;
            while (Vector3.Distance(baseletterList[0].AttachedGameObject.transform.localPosition,
                       BaseletterList[0].CouplePositionTransform) >= CoupleLetterSpacing ||
                   Vector3.Distance(baseletterList[1].AttachedGameObject.transform.localPosition,
                       BaseletterList[1].CouplePositionTransform) >= CoupleLetterSpacing)
            {
                if (toogle)
                {
                    baseletterList[0].AttachedGameObject.transform.localPosition =
                        new Vector3(baseletterList[0].AttachedGameObject.transform.localPosition.x + distance, 0f, 0f);
                    toogle = false;
                    //PlayGroupSound the sound of letter 
                    BaseletterList[0].Play();
                    yield return new WaitForSeconds(time + baseletterList[0].GetComponent<AudioSource>().clip.length);
                }
                else
                {
                    baseletterList[1].AttachedGameObject.transform.localPosition =
                        new Vector3(baseletterList[1].AttachedGameObject.transform.localPosition.x - distance, 0f, 0f);
                    toogle = true;
                    //PlayGroupSound the sound of letter 
                    BaseletterList[1].Play();
                    yield return new WaitForSeconds(time + baseletterList[1].GetComponent<AudioSource>().clip.length);
                }

                yield return new WaitForEndOfFrame();
            }

            _onLetterCollideEachOther.Invoke();
        }

        /// <summary>
        /// Play Group Sound of the current group
        /// </summary>
        /// <param name="numberOfTime"></param>
        /// <returns></returns>
        private IEnumerator PlayGroupSound(int numberOfTime)
        {
            //check is there any audio listener attached 
            if (Camera.main.GetComponent<AudioListener>() != null)
            {
            }
            else
            {
                //Add a Audio listener 
                gameObject.AddComponent<AudioListener>();
            }

            //Only once
            if (numberOfTime == 1)
            {
                //Attch the audio source .
                AudioSource attacheAudioSource = gameObject.AddComponent<AudioSource>();

                //Do  not loop .
                attacheAudioSource.loop = false;

                //Do not play on awake .
                attacheAudioSource.playOnAwake = false;

                //Load the clip .
                attacheAudioSource.clip = CurrentGroupAudioClip;

                //play the clip .
                attacheAudioSource.Play();

                //Do not do anything unless the sound is complete
                while (attacheAudioSource.isPlaying)
                {
                    yield return null;
                }

                //Raise an event That says sound playing is complete .
                _onGroupSoundPlayingComplete.Invoke();
            }
            //More than once
            else
            {
                while (--numberOfTime >= 0)
                {
                    if (gameObject.GetComponent<AudioSource>() == null)
                    {
                        //Attch the audio source .
                        AudioSource attacheAudioSource = gameObject.AddComponent<AudioSource>();

                        //Do  not loop .
                        attacheAudioSource.loop = false;

                        //Do not play on awake .
                        attacheAudioSource.playOnAwake = false;

                        //Load the clip .
                        attacheAudioSource.clip = CurrentGroupAudioClip;

                        //play the clip .
                        attacheAudioSource.Play();
                    }
                    else
                    {
                        //Get The Attch the audio source .
                        AudioSource attacheAudioSource = gameObject.GetComponent<AudioSource>();
                        //Do not need to set the clip it will be set in the if once
                        //play the clip
                        //TODO: "Group sound AT " sound is going to play Here . Pass through proper Audio Mixer .
                        attacheAudioSource.Play();
                    }

                    //You can add some value as offset with "CurrentGroupAudioClip.length" .
                    yield return new WaitForSeconds(CurrentGroupAudioClip.length);
                }

                //Raise an event That says sound playing is complete .
                _onGroupSoundPlayingComplete.Invoke();
            }
        }
        /// <summary>
        /// Play the reward Audio
        /// </summary>
        /// <param name="withdelay"></param>
        private void PlayRewardAudio(float withdelay)
        {
            var playSuperlativEnumerator = PlaySuperlative(withdelay);
            StartCoroutine(playSuperlativEnumerator);
        }
        /// <summary>
        /// play the superlative audio .
        /// If withdelay parameter is not overloaded ,
        /// then it plays the sound immidiately provided
        /// there is no audio is in process .
        /// </summary>
        /// <param name="withdelay"></param>
        /// <returns></returns>
        private IEnumerator PlaySuperlative(float withdelay = 0)
        {
            //if any audio is in process
            while (gameObject.GetComponent<AudioSource>().isPlaying)
            {
                yield return null;
            }

            //wait for desire time
            yield return new WaitForSeconds(withdelay);

            //check for attched audio source
            gameObject.GetComponent<AudioSource>().clip = GetSuperLativeAudioClip;

            //play
            gameObject.GetComponent<AudioSource>().Play();
        }




        #endregion
    }
}