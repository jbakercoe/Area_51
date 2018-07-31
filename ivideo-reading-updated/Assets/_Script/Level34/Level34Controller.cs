using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace level34
{
    public class Level34Controller : MonoBehaviour
    {


        /// <summary>
        /// The visible letter .
        /// </summary>
        public GameObject WordHolder;

        /// <summary>
        /// The words .
        /// </summary>
        public Word[] Words;

        /// <summary>
        /// The current word that is we are using to spell .
        /// </summary>
        [SerializeField] private int _currentWordIndex = 0;

        /// <summary>
        /// Is input allowed .
        /// </summary>
        [SerializeField] private bool _isInputAllowed = false;

        /// <summary>
        /// On click received .
        /// </summary>
        private Action<string> _onclickReceived;


        /// <summary>
        /// On word complete
        /// </summary>
        private Action _onWordComplete;


        /// <summary>
        /// The letter count of the word .
        /// </summary>
        private int _letterCount = 0;


        /// <summary>
        /// The number of times user filed .
        /// </summary>
        private int _numberOfTimesFiled = 0;


        /// <summary>
        /// The number of time user can fail
        /// </summary>
        private const int _numberofTimeUserCanFail = 2;


        private void Awake()
        {
            //attach the button script.
            AttachButtonScript();
        }
        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            //Hook the Action
            _onclickReceived = s => ShowWord(s);
            _onWordComplete = () => WordComplete();
            //call init.
            Init();
        }
        /// <summary>
        /// Creates the word.
        /// </summary>
        private IEnumerator CreateWord()
        {
            //You are going to create word for the first time .
            if (_currentWordIndex == 0)
            {
                //TODO:Play the sound Spell the WordHolder.
                SoundController.Reference.Playclip(Level34SoundController.reference.SpellAudioclip,this.Words[_currentWordIndex].Clip);
            }
            else
            {
                //just play the sound spell and the word.
                SoundController.Reference.Playclip(Level34SoundController.reference.SpellAudioclip, this.Words[_currentWordIndex].Clip);
            }

            //Wait untill all the audio has been completed .
            while (SoundController.Reference.IsMusicOnPlay)
                yield return null;

            //oncce all the audio has been played.
            //Allow to receive Input from button.
            _isInputAllowed = true;

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Init()
        {
            //At the beginning make all letters Deactivate .
            for(var i = 0; i < WordHolder.transform.childCount; i++)
            {
                if (!WordHolder.transform.GetChild(i).gameObject.activeInHierarchy)
                    continue;
                WordHolder.transform.GetChild(i).gameObject.SetActive(false);
            }


        }


        /// <summary>
        /// Called when [letter click].
        /// </summary>
        public void OnLetterClick()
        {
           //Do not take input when animation is running and input is not allowed.
            if (!_isInputAllowed && WordAnimation.AnimationForWord.IsAnimationRunning) return;
            //The button you have Clicked .
            var selectedGameObject = EventSystem.current.currentSelectedGameObject;
            //fire the Action.
            _onclickReceived(selectedGameObject.name);
        }

        /// <summary>
        /// Shows the word.
        /// </summary>
        private void ShowWord(string letter)
        {
            //Convert to lower.
            letter = letter.ToLower();
            //check the length of the current letter first .
            if (_letterCount >= Words[_currentWordIndex].NameOfWord.Length )
                return;
            //get the letter text .
            var lettertext = WordHolder.transform.GetChild(_currentWordIndex).gameObject.GetComponentInChildren<Text>();
            //change the letter .
            lettertext.text = letter;
            //make the letter active .
            WordHolder.transform.GetChild(_letterCount).gameObject.SetActive(true);
            //increment the letterr count .
            _letterCount++;

            //when the letter is complete .
            if (_letterCount == Words[_currentWordIndex].NameOfWord.Length)
            {
                
                _onWordComplete();
            }

        }

        private void AttachButtonScript()
        {
            //Find Gameobjects Button.
            var pool = GameObject.FindGameObjectsWithTag("Button");
            //Get the component of button
            for (int i = 0; i < pool.Length; i++)
            {
                //first remove the listener.
                pool[i].GetComponent<Button>().onClick.RemoveAllListeners();
                //then attach the listener.
                pool[i].GetComponent<Button>().onClick.AddListener(OnLetterClick);
            }
        }

        /// <summary>
        /// This method run when user have completed creating Words.
        /// </summary>
        private void WordComplete()
        {
            string createdWord = string.Empty;
            //check the created word
            for(int i = 0; i < WordHolder.transform.childCount; i++)
            {
                createdWord += WordHolder.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text.ToString();
            }
            //convert it into lowercase.
            createdWord = createdWord.ToLower();

            //check the actual word that need to create 
           if( Words[_currentWordIndex].NameOfWord.ToLower() == createdWord)
            {
                //If user have created the correct word .
                Debug.Log("Correct word created");
            }
            else
            {
                Debug.Log("InCorrect word created");
                //increment the user failure count.
                _numberOfTimesFiled += 1;

                //if user have reached  the limit of failure.
                if(_numberOfTimesFiled <= _numberofTimeUserCanFail)
                {
                    //Throw the wrong word .
                    Debug.Log("Explode");
                    WordAnimation.AnimationForWord.Explode(onAnimationComplete:FailedTocreateWord);
                }
                else
                {
                    
                }
            }

        }


        private void FailedTocreateWord()
        {
                //reset letter count
                _letterCount = 0;
        }


        private void ShowCorrectWord()
        {
            _isInputAllowed = false;

        }

    }

}

