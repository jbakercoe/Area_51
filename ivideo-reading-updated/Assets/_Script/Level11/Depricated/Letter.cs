using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace LevelEleven
{
    [AddComponentMenu("LevelEleven_Reezoo/Letter")]
    public class Letter : MonoBehaviour
    {
        #region Public properties and Variables

        public GameObject AttachedGameObject
        {
            get { return (this.gameObject); }
        }
        [Header("Letter Settings : ")]
        [Tooltip("How Fast Letter will disappear"), Range(0f,1f), Space(10),]
        public float AppearigSpeed = 0.5f;
        [Tooltip("How Fast Letter will Appear"),Range(0f, 1f),Space(20)]
        public float DisappearingSpeed = 0.5f;
        [Tooltip("How much alpha will be reduced or increased over frame"), Range(5f,20f), Space(20)]
        public float AlphaFactor = 5f;
        [Tooltip("Name Of the letter "), Space(20)]
        public string NameOfTheLetter; //Example : "A" ,"B"
        [Tooltip("Position Where letter will appear for the first time "), Space(20)]
        public Vector3 SinglePositionTransform; //Position for the first time .
        [Tooltip("Position Where letter will appear for the Second time when moving to each other  "), Space(20)]
        public Vector3 CouplePositionTransform; //position for the next time

        /// <summary>
        /// Action Raised When Letter Disappear Complete .
        /// </summary>
        public Action OnLetterDisappear;

        /// <summary>
        /// Action Raised When Letter Appear Conplete .
        /// </summary>
        public Action OnLetterAppear;

        /// <summary>
        /// Action Raised When Sound Of current letter player multiple times .
        /// </summary>
        public Action OnMultipleSoundPlayComplete;

        /// <summary>
        /// Action Raised when Sound of current letter played once .
        /// </summary>
        public Action OnSoundPlayComplete;

        #endregion

        #region Private Field and properties

        private AudioClip LetterAudioClip
        {
            get
            {
                //UnityEngine.Debug.Log("Sound Clip Path "+ _audioLoadingPath + NameOfTheLetter + "Sound");
                return (AudioClip) Resources.Load(_audioLoadingPath + NameOfTheLetter, typeof(AudioClip));
            }
        }

        
        private readonly string _audioLoadingPath = "EggGame/Letter/";

        #endregion

        #region Unity Function

        private void Start()
        {
            //Set to default position
            //Normally the value is (0 , 0 , 0)
            AttachedGameObject.transform.localPosition = new Vector3(SinglePositionTransform.x,SinglePositionTransform.y,SinglePositionTransform.z);
        }

        #endregion

        #region Normal Function

        /// <summary>
        /// Activate  or  Deactivate the attached Gameobject
        /// </summary>
        /// <param name="activate"></param>
        public void SetActive(bool activate)
        {
            AttachedGameObject.SetActive(activate);
        }

        /// <summary>
        /// Hide The letter Instantly making Alpha zero .
        /// </summary>
        public void Hide()
        {
            //DeActivate Gameobject.
            this.gameObject.SetActive(false);
            //Get the Image component and make alpha Zero.
            Color imageColor = this.gameObject.GetComponent<Image>().color;
            imageColor.a = 0f;
            this.gameObject.GetComponent<Image>().color = imageColor;
        }

        /// <summary>
        /// Show The letter Instantly Making Alpha 1 .
        /// </summary>
        public void Show()
        {
            //DeActivate Gameobject.
            this.gameObject.SetActive(false);
            //Get the Image component and make alpha one
            Color imageColor = this.gameObject.GetComponent<Image>().color;
            imageColor.a = 1f;
            this.gameObject.GetComponent<Image>().color = imageColor;
        }

        public void MakeletterDisapper()
        {
            //Check the gameobject active or not
            //if not then first activate it .
            if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);

            //Start the coroutine that will make it appear
            IEnumerator letterDisappear = MakeTheLetterApperOrDisappear(AppearigSpeed, AlphaFactor, false);
            //start it
            StartCoroutine(letterDisappear);
        }

        /// <summary>
        /// make the letter apper in the Canvas .
        /// </summary>
        public void MakeletterAppear()
        {
            //Check the gameobject active or not
            //if not then first activate it .
            if (!this.gameObject.activeInHierarchy) this.gameObject.SetActive(true);

            //Start the coroutine that will make it appear
            IEnumerator letterApper = MakeTheLetterApperOrDisappear(DisappearingSpeed, AlphaFactor);
            //start it
            StartCoroutine(letterApper);
        }

        /// <summary>
        /// Make the letter Apper .
        /// </summary>
        /// <param name="delay">control over frames</param>
        /// <param name="howFastItWillAppear">control over color</param>
        /// <param name="appear"> True : appear , Flase : Disappear  </param>
        /// <returns></returns>
        private IEnumerator MakeTheLetterApperOrDisappear(float delay, float howFastItWillAppear, bool appear = true)
        {
            //Get the color of gameobject
            Color colorOfImage = this.gameObject.GetComponent<Image>().color;
            //Alpha value 
            float alpha = colorOfImage.a;
            //Alpha value range [0 to 1]

            //when appear
            if (appear)
            {
                while (alpha <= 1)
                {
                    //increase alpha over time 
                    alpha += Time.deltaTime * howFastItWillAppear;
                    //genarate a same color with different alpha
                    Color updatedColor = colorOfImage;
                    updatedColor.a = alpha;

                    //Set the color
                    this.gameObject.GetComponent<Image>().color = updatedColor;

                    //Ieterate over a delay 
                    yield return new WaitForSeconds(delay);
                }

                OnLetterAppear.Invoke();
            }

            //When Disappear 
            else
            {
                while (alpha >= 0)
                {
                    //decrease alpha over time 
                    alpha -= Time.deltaTime * howFastItWillAppear;
                    //genarate a same color with different alpha
                    Color updatedColor = colorOfImage;
                    updatedColor.a = alpha;

                    //Set the color
                    this.gameObject.GetComponent<Image>().color = updatedColor;

                    //Ieterate over a delay 
                    yield return new WaitForSeconds(delay);
                }

                OnLetterDisappear.Invoke();
            }
        }

        /// <summary>
        /// Play the sound .
        /// Able to play only the letter sound .
        /// </summary>
        /// <param name="numberOfTime"> number of time want to play the sound</param>
        /// <returns></returns>
        public IEnumerator PlayTheSoundOfCurrentLetter(int numberOfTime = 1)
        {
            //Only once 
            if (numberOfTime == 1)
            {
                Play();
                OnSoundPlayComplete.Invoke();
            }
            //More than once
            else
            {
                while (--numberOfTime >= 0)
                {
                    Play();
                    yield return new WaitForSeconds(LetterAudioClip.length);
                }

                OnMultipleSoundPlayComplete.Invoke();
            }
        }

        /// <summary>
        /// Play the sound of the letter .
        /// </summary>
        public void Play()
        {
            //check is there any audio listener attached 
            if (Camera.main.GetComponent<AudioListener>() != null)
            {
            }
            else
            {
                //Add a Audio listener 
                AttachedGameObject.AddComponent<AudioListener>();
            }

            //create A audio source At runtime 
            //It will be Autometically Attached with the Gameobject .
            //check if any AudioSource exist or not .
            if (AttachedGameObject.GetComponent<AudioSource>() == null)
            {
                //create Audio Source
                //AudioSource attachedaudioSource = new AudioSource ();
                //Attch the Audiosource 
                var attachedaudioSource = AttachedGameObject.AddComponent<AudioSource>();
                //Play on Awake 
                attachedaudioSource.playOnAwake = false;
                //Updated enabled 
                attachedaudioSource.enabled = true;
                //Attched clip
                attachedaudioSource.clip = LetterAudioClip;
                //Play the sound
                //TODO: "Sound Of the Letter like A T " sound is going to play Here . Pass through proper Audio Mixer .
                attachedaudioSource.Play();

                //no loop 
                attachedaudioSource.loop = false;
            }
            else
            {
                //no need to add audio source
                //AttachedGameObject.GetComponent<AudioSource>().clip = LetterAudioClip; 
                //Attching audio clip is redundant becaus it is already attched.
                AttachedGameObject.GetComponent<AudioSource>().Play();
                //AttachedGameObject.GetComponent<AudioSource>().loop = false; //Loop is allready made false
            }
        }



        
        #endregion
    }
}