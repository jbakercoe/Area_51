using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Level6
{
    [Serializable]
    [RequireComponent(typeof(Button))]
    public class Alphabet : MonoBehaviour
    {
        [Header("Alphabet Details :")] [Space(10)] [Tooltip("Name Of the Alphabet")]
        public string NameOfTheAlphabet;

        [Space(10)] [Tooltip("Alphabet Gameobject")]
        public GameObject AlphabetObject;

        [Space(10)] [Tooltip("Original Rect trnsform of alphabet")]
        public RectTransform AlphabetRectTransform;

        [Space(10)] [Tooltip("Default Rect trnsform of alphabet")]
        public RectTransform DefaultAlphabetRectTransform;

        [Space(10)] [Tooltip("Transition time to move alphabet in word")]
        public float TransitionTime;

        [Space(10)] [Tooltip("Animation Performed alphabet")]
        public AnimationType CurrentAnimation = AnimationType.FlipAnimation;

        [Header("Alphabet Colour Setting : ")] [Space(10)] [Tooltip("Color of Alphabet at begining ")]
        public Color AlphabetOriginColor;

        [Space(5)] [Tooltip("Color of Alphabet after placing in the word ")]
        public Color AlphabetColorInWord;

        [Header("Alphabet Flip / Jump / Roll Animation Settings :")]
        [Space(10)]
        [Tooltip("Speed of Animation value increase and speed decrease")]
        [Range(0f, 0.5f)]
        public float AnimationSpeed;

        [Space(5)]
        [Tooltip(
            "Amount of time the letter will take to jump . the speed increase if jump time decrease . less time more distance")]
        [Range(0f, 2f)]
        public float JumpTime = 1f;

        [Space(5)] [Tooltip("Amount of Jump")] [Range(100f, 200f)]
        public float JumpHeight = 150f;

        [Space(5)]
        [Tooltip(
            "Amount of time the letter will take to Fall . the speed increase if Fall time decrease . less time more distance")]
        [Range(0f, 2f)]
        public float FallTime = 1f;

        [Space(5)] [Tooltip("Rotation amount per unit")] [Range(100f, 300f)]
        public float RotationAmount = 200f;

        [Space(5)] [Tooltip("Amount of rotation in degree")] [Range(360f, 720f)]
        public float RotationDegree = 360f;

        [Header("Alphabet Status : ")] [Space(20)] [Tooltip("Selected/NotSelected for current word")]
        public AlphabetStatus CurrentAlphabetStatus;

        [Space(10)] [Tooltip("Where it is ")] public AlphabetTransitionStatus CurrentAlphabetTransitionStatus;

        [Space(20)] [Header("Words Settings : ")] [Tooltip("Add the name of those letter will use this alphabet")]
        public List<string> Words;

        [Space(10)]
        [Header("Rect Transform  Settings : ")]
        [Tooltip("Add the RectTransform of those letter will use this alphabet")]
        public List<RectTransform> AlphabetPositionInWord;

        //Delegate for button click event handeler .
        public delegate void ButtonClickEventHandeler(object source, AlphabetEventArgs eventArgs);

        //event raised when a button clicked .
        public static event ButtonClickEventHandeler Buttonclicked;
        private Dictionary<string, RectTransform> _wordPositionDictionary;

        public enum AlphabetStatus
        {
            SelectedForCurrentWord = 0,
            NotSelectedForCurrentWord = 1,
            WaitingForInitializarion = 2
        }

        public enum AlphabetTransitionStatus
        {
            ReadyForTransition = 0,
            TransitionInProgress = 1,
            TransitionCompleteed = 2,
            AnimationInprogress = 3,
            AnimationCompleted = 4
        }

        public enum AnimationType
        {
            FlipAnimation = 0,
            JumpAnimation = 1,
            SideJumpAnimation = 2,
            ReverseSideJumpAniation = 3,
            RecerseJumpAnimation = 4,
            RollAnimation = 5,
            ReverseRollAnimation = 6
        }

        public virtual void Awake()
        {
            InitAlphabet();
        }

        //An alternative Of Constructor
        private void InitAlphabet()
        {
            //Name the Alphabet if string is not null or empty .
            if (!string.IsNullOrEmpty(gameObject.name)) NameOfTheAlphabet = gameObject.name;

            //Initialize list and dictionary
            _wordPositionDictionary = new Dictionary<string, RectTransform>();

            //Make dictionary empty
            foreach (var item in _wordPositionDictionary)
                //Remove all keys 
                _wordPositionDictionary.Remove(item.Key);

            //construct The Dictionary 
            //iterate over keys
            for (var k = 0; k < Words.Count; k++)
                //iterate over Values
                //If extra value is added without key they will be just ignored.
                _wordPositionDictionary.Add(Words[k], AlphabetPositionInWord[k]);

            //Remove all listener if any redundent is added by mistake 
            gameObject.GetComponent<Button>().onClick.RemoveAllListeners();

            //Add Listener According to logic
            gameObject.GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    //Once we receive a click .
                    //we will make alphabet move according to his status .
                    //First We will check the aplphabet is the correct lphabet for the word or not .
                    //Correct alphabet will be marked as SelectedForCurrentWord by AlphabetController .
                    //Next we check The transition status of Alphabet .
                    //An Alphabet can staty in in three region .
                    //If Alphabet is at docker ie. at side then it will be 
                    //marked as Ready for transition .
                    //When it is moving it is moving or moved to its's destination we will not take any click action .

                    //if all alphabet has been reached to the destination then there is no neeed to receive any clivk.
                    if (AlphabetController.Referenece.IsAllAlphabetReached) return;

                    //if any alphabet is moving then also cancell all the click received at that time .
                    //When an alphabet is moving if that time user click a wrong alphabet .
                    //in order to avoid reverse call that bring alphabet back to it's initial position ,
                    //we will ignore the click.
                    if (AlphabetController.Referenece.IsAnyAlphabetIsMoving) return;


                    if (CurrentAlphabetTransitionStatus == AlphabetTransitionStatus.ReadyForTransition)
                    {
                        if (CurrentAlphabetStatus == AlphabetStatus.SelectedForCurrentWord)
                        {
                            Debug.Log("Right Click received");
                            MoveToDesiredPosition(AlphabetController.Referenece
                                .WordListForGroupOne[AlphabetController.Referenece.CurrentWordListIndex]
                                .name);
                            //Raise event when button is clicked.
                            //Create Alphabet eventargs with IsCorrectAlphabetClicked property true for correct button click .
                            var eventargs = new AlphabetEventArgs {IsCorrectAlphabetClicked = true};
                            //Buttonclicked?.Invoke(this, eventargs);
                        }

                        //if we click on alphaber that is not going to be used in word.
                        if (CurrentAlphabetStatus == AlphabetStatus.NotSelectedForCurrentWord)
                        {
                            
                            Debug.Log("Wrong Click received");
                            //Raise event when button is clicked.
                            //Create Alphabet eventargs with IsCorrectAlphabetClicked property true for correct button click .
                            var eventargs = new AlphabetEventArgs {IsCorrectAlphabetClicked = false};
                            //Buttonclicked?.Invoke(this, eventargs);

                            //TODO: A wrong WordHolder selected .

                            //Play The Actual Sound one more time .
                            //Make If correct alphabets are allready placed then make them back to original position.
                        }
                    }
                });
        }

        /// <summary>
        ///     This Method Move The Alphabet to its desired position for proper word .
        /// </summary>
        /// <param name="word"></param>
        public void MoveToDesiredPosition(string word)
        {
            //Check if the word present
            if (_wordPositionDictionary.ContainsKey(word))
            {
                //Then fetch the Desired Rect Transform .
                var desiredPosition = _wordPositionDictionary[word];
                //Attch with the coroutine .
                var moveIenumerator = MoveAlphabet(0f, desiredPosition, AlphabetTransitionStatus.TransitionCompleteed);
                //Start the coroutine .
                StartCoroutine(moveIenumerator);
            }
            else
            {
                Debug.Log("No record found for this word " + word);
            }
        }

        /// <summary>
        ///     This Method Move The Alphabet to its desired position over time for proper word one at a time .
        /// </summary>
        /// <param name="word">name of the word.</param>
        /// <param name="lengthofSound">length of the audio clip.</param>
        public void MoveOneAlphabetToDesiredPositionOverTime(string word, float lengthofSound)
        {
            //Check if the word present
            if (_wordPositionDictionary.ContainsKey(word))
            {
                //Then fetch the Desired Rect Transform .
                var desiredPosition = _wordPositionDictionary[word];
                //Attch with the coroutine .
                var moveIenumerator = MoveOneAlphabetOverTime(lengthofSound, 0f, desiredPosition,
                    AlphabetTransitionStatus.TransitionCompleteed);
                //Start the coroutine .
                StartCoroutine(moveIenumerator);
            }
            else
            {
                Debug.Log("No record found for this word " + word);
            }
        }

        /// <summary>
        ///     This Method Move The Alphabet to its previous position  .
        /// </summary>
        public void BackToPreviousPosition()
        {
            //Attch with the coroutine .
            var moveIenumerator = MoveAlphabet(0f, DefaultAlphabetRectTransform,
                AlphabetTransitionStatus.ReadyForTransition);
            //Start the coroutine .
            StartCoroutine(moveIenumerator);
        }

        /// <summary>
        ///     Coroutine That will be fired to move the letter in a word  or out from a word .
        /// </summary>
        /// <param name="withdelay">speed</param>
        /// <param name="finalRectTransform">where you want to move</param>
        /// <param name="status">When transition is complete what will be the status</param>
        /// <returns></returns>
        private IEnumerator MoveAlphabet(float withdelay, RectTransform finalRectTransform,
            AlphabetTransitionStatus status)
        {
            //Current time when we start.
            var currentTime = 0f;

            //Old position for max
            var oldPositionMax = AlphabetRectTransform.GetComponent<RectTransform>().offsetMax;

            //Old position for min
            var oldPositionMin = AlphabetRectTransform.GetComponent<RectTransform>().offsetMin;

            //Marked it as transition is in progress
            CurrentAlphabetTransitionStatus = AlphabetTransitionStatus.TransitionInProgress;
            //Iterator block.
            while (currentTime <= TransitionTime)
            {
                //Alphabet transition
                currentTime += Time.deltaTime;
                var normalizedValue = currentTime / TransitionTime;
                AlphabetRectTransform.GetComponent<RectTransform>().offsetMax =
                    Vector2.Lerp(oldPositionMax, finalRectTransform.offsetMax, normalizedValue);
                AlphabetRectTransform.GetComponent<RectTransform>().offsetMin =
                    Vector2.Lerp(oldPositionMin, finalRectTransform.offsetMin, normalizedValue);

                //Color Transition
                if (status == AlphabetTransitionStatus.TransitionCompleteed)
                    AlphabetObject.GetComponent<Text>().color =
                        Color.Lerp(AlphabetOriginColor, AlphabetColorInWord, normalizedValue);

                //Color Transition
                if (status == AlphabetTransitionStatus.ReadyForTransition)
                    AlphabetObject.GetComponent<Text>().color =
                        Color.Lerp(AlphabetColorInWord, AlphabetOriginColor, normalizedValue);

                yield return new WaitForSeconds(withdelay);
            }

            //Marked it as transition is complte .
            CurrentAlphabetTransitionStatus = status;
        }

        /// <summary>
        ///     Move One Alphabet one at a time .
        /// </summary>
        /// <param name="lengthOfTime">time taken by the alphabet to move to the destination.</param>
        /// <param name="withdelay">controll the co routine speed.</param>
        /// <param name="finalRectTransform">final position.</param>
        /// <param name="status">status of the alphabet when reached to destination.</param>
        /// <returns></returns>
        private IEnumerator MoveOneAlphabetOverTime(float lengthOfTime, float withdelay,
            RectTransform finalRectTransform, AlphabetTransitionStatus status)
        {
            //if any alphabet is moving then wait.
            if (AlphabetController.Referenece.IsAnyAlphabetIsMoving) yield return null;

            //Current time when we start.
            var currentTime = 0f;

            //Old position for max
            var oldPositionMax = AlphabetRectTransform.GetComponent<RectTransform>().offsetMax;

            //Old position for min
            var oldPositionMin = AlphabetRectTransform.GetComponent<RectTransform>().offsetMin;

            //Marked it as transition is in progress
            CurrentAlphabetTransitionStatus = AlphabetTransitionStatus.TransitionInProgress;
            //Iterator block.
            while (currentTime <= lengthOfTime)
            {
                //Alphabet transition
                currentTime += Time.deltaTime;
                var normalizedValue = currentTime / lengthOfTime;
                AlphabetRectTransform.GetComponent<RectTransform>().offsetMax =
                    Vector2.Lerp(oldPositionMax, finalRectTransform.offsetMax, normalizedValue);
                AlphabetRectTransform.GetComponent<RectTransform>().offsetMin =
                    Vector2.Lerp(oldPositionMin, finalRectTransform.offsetMin, normalizedValue);

                //Color Transition
                if (status == AlphabetTransitionStatus.TransitionCompleteed)
                    AlphabetObject.GetComponent<Text>().color =
                        Color.Lerp(AlphabetOriginColor, AlphabetColorInWord, normalizedValue);

                //Color Transition
                if (status == AlphabetTransitionStatus.ReadyForTransition)
                    AlphabetObject.GetComponent<Text>().color =
                        Color.Lerp(AlphabetColorInWord, AlphabetOriginColor, normalizedValue);

                yield return new WaitForSeconds(withdelay);
            }

            //Marked it as transition is complte .
            CurrentAlphabetTransitionStatus = status;
        }

        /// <summary>
        ///     play the anumation of Alphabet
        /// </summary>
        public void AnimateAlphabet()
        {
            //change alphbet transition status .
            CurrentAlphabetTransitionStatus = AlphabetTransitionStatus.AnimationInprogress;

            var animationEnumerator = AnimateAlphabet(AnimationSpeed);
            StartCoroutine(animationEnumerator);
            
        }

        /// <summary>
        ///     Animate alphabet with delay.
        /// </summary>
        /// <param name="withDelay"></param>
        /// <returns></returns>
        private IEnumerator AnimateAlphabet(float withDelay)
        {
            
            //Flip Animaation.
            //Fip aniation conation a jump and number of ritation simultaneously
            //and come back to its previous position.

            //Jump

            //current position for max
            var currentpositionMax = AlphabetRectTransform.GetComponent<RectTransform>().offsetMax;

            //current position for min
            var currentPositionMin = AlphabetRectTransform.GetComponent<RectTransform>().offsetMin;

            //max contain two value
            //left and top
            //left is the x co ordinate.
            //top is the y co ordinate.
            //min coitain two value.
            //Right and bottom.
            //x co ordinate is Right.
            //y co ordinate is bottom.
            //in order to provide a lift we need to increase top and bottom simultaneously.
            //so top = Y of MAX.
            //bottom = Y of min.

            //0.0f cast it to float
            var lift = 0.0f;
            if (CurrentAnimation == AnimationType.RecerseJumpAnimation ||
                CurrentAnimation == AnimationType.ReverseSideJumpAniation)
                lift = -JumpHeight;

            if (CurrentAnimation == AnimationType.FlipAnimation || CurrentAnimation == AnimationType.JumpAnimation ||
                CurrentAnimation == AnimationType.SideJumpAnimation)
                lift = JumpHeight;

            //Outer scope
            var desirepositionMax = new Vector2();
            var desirePositionMin = new Vector2();
            if (CurrentAnimation != AnimationType.SideJumpAnimation)
            {
                //current position for max
                desirepositionMax = new Vector2(AlphabetRectTransform.GetComponent<RectTransform>().offsetMax.x,
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMax.y + lift);

                //current position for min
                desirePositionMin = new Vector2(AlphabetRectTransform.GetComponent<RectTransform>().offsetMin.x,
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMin.y + lift);
            }

            if (CurrentAnimation == AnimationType.SideJumpAnimation)
            {
                //current position for max
                desirepositionMax = new Vector2(AlphabetRectTransform.GetComponent<RectTransform>().offsetMax.x + lift,
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMax.y);

                //current position for min
                desirePositionMin = new Vector2(AlphabetRectTransform.GetComponent<RectTransform>().offsetMin.x + lift,
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMin.y);
            }

            if (CurrentAnimation == AnimationType.ReverseSideJumpAniation)
            {
                //current position for max
                desirepositionMax = new Vector2(AlphabetRectTransform.GetComponent<RectTransform>().offsetMax.x + lift,
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMax.y);

                //current position for min
                desirePositionMin = new Vector2(AlphabetRectTransform.GetComponent<RectTransform>().offsetMin.x + lift,
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMin.y);
            }

            //Current time when we start Animation.
            var currentJumpTime = 0f;

            //Animation time
            var animationTime = JumpTime;

            //Animation will occur for jump and flip
            if (CurrentAnimation == AnimationType.FlipAnimation || CurrentAnimation == AnimationType.JumpAnimation ||
                CurrentAnimation == AnimationType.RecerseJumpAnimation ||
                CurrentAnimation == AnimationType.SideJumpAnimation ||
                CurrentAnimation == AnimationType.ReverseSideJumpAniation)
                while (currentJumpTime <= animationTime)
                {
                    currentJumpTime += Time.deltaTime;
                    var normalizedValue = currentJumpTime / animationTime;
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMax =
                        Vector2.Lerp(currentpositionMax, desirepositionMax, normalizedValue);
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMin =
                        Vector2.Lerp(currentPositionMin, desirePositionMin, normalizedValue);
                    yield return new WaitForSeconds(withDelay);
                }

            //Animation will occur only for flip
            if (CurrentAnimation == AnimationType.FlipAnimation)
            {
                //Rotate
                float totalRotation = 0;
                var rotationAmt = RotationAmount * Time.deltaTime;
                //Iterator block for rotate
                while (totalRotation < RotationDegree)
                {
                    AlphabetRectTransform.GetComponent<RectTransform>().Rotate(rotationAmt, 0, 0);
                    totalRotation += rotationAmt;
                    yield return new WaitForSeconds(withDelay);
                }
            }

            //Animation will occur for jump and flip
            if (CurrentAnimation == AnimationType.FlipAnimation || CurrentAnimation == AnimationType.JumpAnimation ||
                CurrentAnimation == AnimationType.RecerseJumpAnimation ||
                CurrentAnimation == AnimationType.SideJumpAnimation ||
                CurrentAnimation == AnimationType.ReverseSideJumpAniation 
                )
            {
                //Fall

                //Current time when we start Animation.
                var currentfallTime = 0f;

                //Animation time
                var fallanimationTime = FallTime;

                //Iterator block for fall
                while (currentfallTime <= fallanimationTime)
                {
                    currentfallTime += Time.deltaTime;
                    var normalizedValue = currentfallTime / fallanimationTime;
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMax = Vector2.Lerp(
                        AlphabetRectTransform.GetComponent<RectTransform>().offsetMax, currentpositionMax,
                        normalizedValue);
                    AlphabetRectTransform.GetComponent<RectTransform>().offsetMin = Vector2.Lerp(
                        AlphabetRectTransform.GetComponent<RectTransform>().offsetMin, currentPositionMin,
                        normalizedValue);
                    yield return new WaitForSeconds(withDelay);
                }
            }

            //Animation will occur only for Rotation
            if (CurrentAnimation == AnimationType.RollAnimation ||
                CurrentAnimation == AnimationType.ReverseRollAnimation)
            {
                //Rotate
                float totalRotation = 0;
                var rotationAmt = 0.0f;
                if (CurrentAnimation == AnimationType.RollAnimation)
                    rotationAmt = RotationAmount * Time.deltaTime;
                else
                    rotationAmt = -RotationAmount * Time.deltaTime;

                if (CurrentAnimation == AnimationType.RollAnimation)
                    while (totalRotation < RotationDegree)
                    {
                        AlphabetRectTransform.GetComponent<RectTransform>().Rotate(0, rotationAmt, 0);
                        totalRotation += rotationAmt;
                        yield return new WaitForSeconds(withDelay);
                    }
                else
                    while (totalRotation > -RotationDegree)
                    {
                        AlphabetRectTransform.GetComponent<RectTransform>().Rotate(0, rotationAmt, 0);
                        totalRotation += rotationAmt;
                        yield return new WaitForSeconds(withDelay);
                    }
            }

            //change alphbet transition status .
            CurrentAlphabetTransitionStatus = AlphabetTransitionStatus.AnimationCompleted;
        }
    }

    /// <summary>
    ///     Alphabet Event Args .
    /// </summary>
    public class AlphabetEventArgs : EventArgs
    {
        public bool IsCorrectAlphabetClicked { get; set; }
    }
}