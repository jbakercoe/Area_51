using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level6
{
    /// <summary>
    ///     Controll Over Alphabet .
    ///     Controll the alphabet to create word .
    /// </summary>
    public class AlphabetController : MonoBehaviour
    {
        #region Public Variables

        [Header("List Of Words For Group ")] public List<GameObject> WordListForGroupOne;

        [Header("List Of Alphabets For Group 1")]
        public List<GameObject> Alphabets;

        [Header("Click Idle Time in seconds")] [Range(20f, 50f)]
        public float AllowedIdleTime = 20f;

        [Header("Next Exercise")] [Tooltip("Next Exercise after complete this exercise")]
        public GameObject NextExercise;

        //Static reference.
        public static AlphabetController Referenece;

        //property
        public int CurrentWordListIndex { get; private set; }

        /// <summary>
        ///     Check if any alphabet is moving or not.
        /// </summary>
        public bool IsAnyAlphabetIsMoving
        {
            get
            {
                var i = 1;
                foreach (var item in Alphabets)
                    if (item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                        Alphabet.AlphabetTransitionStatus.TransitionInProgress)
                        i +=1;
                    else
                        i +=0;

                return i > 1;
            }
        }

        /// <summary>
        ///     All alphabet is reached to there desired location .
        ///     Next thing will be animating the alphabets .
        /// </summary>
        public bool IsAllAlphabetReached
        {
            get
            {
                var i = 1;
                foreach (var item in _selectedGameobjectForWord)
                    if (item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                        Alphabet.AlphabetTransitionStatus.TransitionCompleteed)
                        i *= 1;
                    else
                        i *= 0;

                return i == 1;
            }
        }

        #endregion

        #region private Field

        private int _wrongClickCount;
        [SerializeField] private List<GameObject> _selectedGameobjectForWord;

        private bool IsAllAlphabetAnimationCompleted
        {
            get
            {
                var i = 1;
                foreach (var item in _selectedGameobjectForWord)
                    if (item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                        Alphabet.AlphabetTransitionStatus.AnimationCompleted)
                        i *= 1;
                    else
                        i *= 0;

                return i == 1;
            }
        }

        #endregion

        #region Unity Functions

        public void Awake()
        {
            //Initialize the reference.
            Referenece = this;
            //Call for init.
            Init();
        }

        private void Start()
        {
            PlaySoundForWord(CurrentWordListIndex, true);
        }

        private void OnEnable()
        {
            //attach the event
            Alphabet.Buttonclicked += WhenAlphabetClicked;
        }

        private void OnDisable()
        {
            // de attach the event.
            Alphabet.Buttonclicked -= WhenAlphabetClicked;
        }

        #endregion

        #region Functions

        private void Init()
        {
            //Make all alphabets Wait to complete the sound.
            //So they will not receive any click action
            foreach (var item in Alphabets)
            {
                Debug.Log(item.GetComponent<Alphabet>().NameOfTheAlphabet);
                //In this state if you click on alphabet noyhing happens at all .
                item.GetComponent<Alphabet>().CurrentAlphabetStatus = Alphabet.AlphabetStatus.WaitingForInitializarion;
            }
        }

        /// <summary>
        ///     Play the initial sound when word start .
        /// </summary>
        /// <param name="indexOfWord">pass the word index from the list that you want to play</param>
        /// <param name="isFirstWord">pass true if it is the first word of the exercise</param>
        private void PlaySoundForWord(int indexOfWord, bool isFirstWord)
        {
            //Play all sounds.
            //1 . Lets make word.
            //2 . What sound do you here when I say the word .
            //3 . Sound of the word like : at , am , sat etc .
            //4 . Click on the sound that you here .
            //5 . When I say .
            //6 . Sound of the word like : at , am , sat etc .
            if (isFirstWord)
                SoundController.Reference.Playclip(SoundController.Reference.LetsMakeWordAudioClip,
                    SoundController.Reference.WhatSoundDoYouHere,
                    WordListForGroupOne[indexOfWord].GetComponent<WordStructure>().WordAudioClip,
                    SoundController.Reference.ClickOnTheSoundThatYouHere,
                    WordListForGroupOne[indexOfWord].GetComponent<WordStructure>().WordAudioClip);
            else
                SoundController.Reference.Playclip(SoundController.Reference.WhatSoundDoYouHere,
                    WordListForGroupOne[indexOfWord].GetComponent<WordStructure>().WordAudioClip,
                    SoundController.Reference.ClickOnTheSoundThatYouHere,
                    WordListForGroupOne[indexOfWord].GetComponent<WordStructure>().WordAudioClip);

            //When WordHolder sound is complete just make letter ready to accept click .
            AlphabetStatusChange();
        }

        private IEnumerator ChangeStatusOfAlphabet()
        {
            while (SoundController.Reference.IsMusicOnPlay) yield return null;

            //check current word that is going to be create .
            //name of the current word .
            var nameofword = WordListForGroupOne[CurrentWordListIndex].GetComponent<WordStructure>().WordName;
            _selectedGameobjectForWord = new List<GameObject>();

            //iterate over alphabets.
            foreach (var item in Alphabets)
            {
                var itemName = item.GetComponent<Alphabet>().NameOfTheAlphabet;
                //As we know the alphabet have only one letter .
                //we will check word contain that alphabet or not .
                //Add those gameobject in selected gameobject for this word.
                if (nameofword.Contains(itemName))
                {
                    item.GetComponent<Alphabet>().CurrentAlphabetStatus =
                        Alphabet.AlphabetStatus.SelectedForCurrentWord;
                    _selectedGameobjectForWord.Add(item);
                }
                else
                {
                    item.GetComponent<Alphabet>().CurrentAlphabetStatus =
                        Alphabet.AlphabetStatus.NotSelectedForCurrentWord;
                }
            }

            //When letters are allowed to receive click start idle status check coroutine 
            InvokeRepeating(nameof(Idlecheck), AllowedIdleTime, AllowedIdleTime);
            //wait for final animation.
            ShowFinalAnimation();
            Debug.Log("Alphabet status change ......");
        }

        private void AlphabetStatusChange()
        {
            //alphabet status enumerator.
            var alphabetstatusEnumerator = ChangeStatusOfAlphabet();
            //start co routine ;
            StartCoroutine(alphabetstatusEnumerator);
        }

        /// <summary>
        ///     Button click received handeler.
        /// </summary>
        public void ButtonClickReceivedForSoundAlert()
        {
            //stop the idle check coroutine.
            CancelInvoke(nameof(Idlecheck));
            //If any previous coroutine is running then stop it .
            CancelInvoke(nameof(RepeatTheSoundToAlertThePlayer));
            //Make a fresh  invoke
            InvokeRepeating(nameof(RepeatTheSoundToAlertThePlayer), AllowedIdleTime, AllowedIdleTime);
        }

        /// <summary>
        ///     Play the sound after some time when player is idle.
        /// </summary>
        /// <returns></returns>
        private void RepeatTheSoundToAlertThePlayer()
        {
            //when your wait is over just play the sound
            SoundController.Reference.Playclip(SoundController.Reference.ClickOnTheSoundThatYouHere,
                WordListForGroupOne[CurrentWordListIndex].GetComponent<WordStructure>().WordAudioClip);
        }

        /// <summary>
        ///     Check the player idle condition and play the sound to alert him .
        ///     This method will start at the very begining .
        /// </summary>
        /// <returns></returns>
        private void Idlecheck()
        {
            //If any song is playing ovet qeue .
            //just ignore it
            if (SoundController.Reference.IsMusicOnPlay)
                return;
            SoundController.Reference.Playclip(SoundController.Reference.ClickOnTheSoundThatYouHere,
                WordListForGroupOne[CurrentWordListIndex].GetComponent<WordStructure>().WordAudioClip);
        }

        /// <summary>
        ///     When we receive an click over alphabet.
        /// </summary>
        /// <param name="source">which alphabet is clicked</param>
        /// <param name="args">details</param>
        private void WhenAlphabetClicked(object source, AlphabetEventArgs args)
        {
            //Button click received.
            ButtonClickReceivedForSoundAlert();
            if (args.IsCorrectAlphabetClicked) return;
            Debug.Log("Wrong button clicked");
            //increment wrong click count .
            _wrongClickCount = _wrongClickCount + 1;
            //Higher value of  wrong click have no effect
            if (_wrongClickCount > 4) return;

            if (_wrongClickCount > 3)
            {
                //Move the alpbahet to correct position 
                //MakeWord();
                CreateWord();
                return;
            }

            //if any sound is playing then stop and delete it
            SoundController.Reference.StopClip();
            //play the sound of wrong button
            SoundController.Reference.Playclip(WordListForGroupOne[CurrentWordListIndex]
                .GetComponent<WordStructure>()
                .WordPronouncedAudioClip);
            //Name of word.
            var nameofword = WordListForGroupOne[CurrentWordListIndex].name;
            //now check the correct alphabet have reached there desire condition or not 
            //iterate over alphabets.
            foreach (var item in Alphabets)
            {
                //If not correct alphabet .
                if (!nameofword.Contains(item.GetComponent<Alphabet>().NameOfTheAlphabet)) continue;
                //if alphabet has been moved to desire position.
                if (item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                    Alphabet.AlphabetTransitionStatus.TransitionCompleteed)
                    item.GetComponent<Alphabet>().BackToPreviousPosition();
            }
        }

        /// <summary>
        ///     Create word by moving alphabet .
        /// </summary>
        [Obsolete("Depricated")]
        private void MakeWord()
        {
            //Name of word.
            var nameOfword = WordListForGroupOne[CurrentWordListIndex].name;
            foreach (var item in Alphabets)
            {
                Debug.Log("item is " + item.name);
                //If not correct alphabet .
                if (!nameOfword.Contains(item.GetComponent<Alphabet>().NameOfTheAlphabet)) continue;
                //If alphabet is not selected extra check
                if (item.GetComponent<Alphabet>().CurrentAlphabetStatus ==
                    Alphabet.AlphabetStatus.NotSelectedForCurrentWord)
                    continue;
                //if alphabet has been moved to desire position.
                if (item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                    Alphabet.AlphabetTransitionStatus.TransitionCompleteed)
                    continue;

                //move over time
                MoveWordOverTime(nameOfword, item);
            }
        }

        /// <summary>
        ///     create the word by moving Alphabet one by one over time .
        /// </summary>
        public void CreateWord()
        {
            var makealphabetIenumerator = MoveAlphabetOneByOne();
            StartCoroutine(makealphabetIenumerator);
        }

        /// <summary>
        ///     Move alphabet one by one to create the word.
        /// </summary>
        private IEnumerator MoveAlphabetOneByOne()
        {
            //Name of word.
            var nameOfword = WordListForGroupOne[CurrentWordListIndex].name;
            //Iterate over the  selected object.
            foreach (var item in _selectedGameobjectForWord)
                //if alphabet moving or reached then ignore .
                if (item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                    Alphabet.AlphabetTransitionStatus.TransitionCompleteed ||
                    item.GetComponent<Alphabet>().CurrentAlphabetTransitionStatus ==
                    Alphabet.AlphabetTransitionStatus.TransitionInProgress)
                {
                }
                else
                {
                    MoveWordOverTime(nameOfword, item);
                    yield return new WaitForSeconds(GetAlphabetSound(item).length);
                }
        }

        /// <summary>
        ///     Move the word over time.
        ///     word will move to desire location depnding on the sound length used to pronounced the word.
        /// </summary>
        /// <returns></returns>
        private void MoveWordOverTime(string wordName, GameObject alphabetThatwillbeMoved)
        {
            //length of the sound that is currently playing.
            var lengthofthesound = GetAlphabetSound(alphabetThatwillbeMoved).length;

            //move to desire position
            alphabetThatwillbeMoved.GetComponent<Alphabet>()
                .MoveOneAlphabetToDesiredPositionOverTime(wordName, lengthofthesound);

            //play the sound
            PlayAlphabetSound(GetAlphabetSound(alphabetThatwillbeMoved));
        }

        /// <summary>
        ///     Return the sound of the alphabet,
        /// </summary>
        /// <param name="alphabetThatwillbeMoved"></param>
        /// <returns></returns>
        private AudioClip GetAlphabetSound(GameObject alphabetThatwillbeMoved)
        {
            //Audioclip for alphabet sound.
            AudioClip alphabetsound = null;
            //get all alphabet module to get the sound of the alphabet.
            var alphabetModules = WordListForGroupOne[CurrentWordListIndex].GetComponents<AlphabetModule>();
            //iterate over alphabet module.
            foreach (var alphabetModule in alphabetModules)
            {
                Debug.Log("Alphabet modules Attached " + alphabetModule.NameOfAlphabet);
                if (alphabetModule.NameOfAlphabet != alphabetThatwillbeMoved.GetComponent<Alphabet>().NameOfTheAlphabet)
                    continue;
                alphabetsound = alphabetModule.AlphabetSound;
            }

            return alphabetsound;
        }

        /// <summary>
        ///     play the sound of the clip .
        /// </summary>
        /// <param name="clip"></param>
        private void PlayAlphabetSound(AudioClip clip)
        {
            //stop all sound.
            SoundController.Reference.StopClip();
            //play the correct sound.
            SoundController.Reference.Playclip(clip);
        }

        /// <summary>
        ///     Show final Animation.
        /// </summary>
        private void ShowFinalAnimation()
        {
            //hook with a ienumerator.
            var finalIenumerator = ShowAnimation();
            //start coroutine.
            StartCoroutine(finalIenumerator);
        }

        /// <summary>
        ///     When all alphabet reached to the desire location .
        ///     Then Final animation is shown.
        /// </summary>
        private IEnumerator ShowAnimation()
        {
            //start the co routine in update manner.
            while (!IsAllAlphabetReached) yield return null;

            //Start animation alphhabet .
            foreach (var item in _selectedGameobjectForWord) item.GetComponent<Alphabet>().AnimateAlphabet();

            //play the word sound.
            SoundController.Reference.StopClip();
            //play the sound.
            SoundController.Reference.Playclip(WordListForGroupOne[CurrentWordListIndex]
                .GetComponent<WordStructure>()
                .WordAudioClip);

            //Wait for the animation to complete
            while (!IsAllAlphabetAnimationCompleted) yield return null;

            //reposition
            RePositionAlphabets();
        }

        /// <summary>
        ///     Bring all the alphabets back to it's initial position.
        /// </summary>
        private void RePositionAlphabets()
        {
            //start an enumerator for rebase
            var rebaseEnumerator = RebaseAllTheAlphabets();
            StartCoroutine(rebaseEnumerator);
        }

        /// <summary>
        ///     Bring all alphabet back to initial position.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RebaseAllTheAlphabets()
        {
            

            //chole a superlative sound.
            var superLative =
                SoundController.Reference.Superlatives[Random.Range(0, SoundController.Reference.Superlatives.Length)];
            //play the superlative sound.
            SoundController.Reference.Playclip(superLative);
            //wait for superlative to end.
            yield return new WaitForSeconds(superLative.length);
            //bring the alphabets back.
            foreach (var itemGameObject in _selectedGameobjectForWord)
                itemGameObject.GetComponent<Alphabet>().BackToPreviousPosition();

            //wait untill the letter return to initial position
            while (IsAnyAlphabetIsMoving) yield return null;

            //call on word complete.
            OnWordComplete();
        }

        /// <summary>
        ///     After complete the word.
        /// </summary>
        private void OnWordComplete()
        {
            //Reset wrong 
            _wrongClickCount = 0;
            //Stop all coroutine.
            StopAllCoroutines();
            //stop all invoke.
            CancelInvoke();
            //current word index should be less than total number of word in a list
            if (WordListForGroupOne.Count - 1 > CurrentWordListIndex)
            {
                //increment current word index.
                CurrentWordListIndex += 1;
            }
            else
            {
                OnExerciseComplete();
                return;
            }

            //call init
            Init();
            //play sounds
            PlaySoundForWord(CurrentWordListIndex, false);
        }

        /// <summary>
        ///     Called when Exercise is complete .
        /// </summary>
        private void OnExerciseComplete()
        {
            //Reset wrong 
            _wrongClickCount = 0;
            //Stop all coroutine.
            StopAllCoroutines();
            //stop all invoke.
            CancelInvoke();
            //just return if there is no exercise left.
            if (NextExercise == null) return;
            //Deactivate this gameobjcet .
            gameObject.SetActive(false);
            //Deactivate it's parent.
            gameObject.transform.parent.gameObject.SetActive(false);
            //Next exercise acivate.
            NextExercise.SetActive(true);
            //deactivate script.
            this.gameObject.GetComponent<AlphabetController>().enabled = false;
        }

        #endregion
    }
}