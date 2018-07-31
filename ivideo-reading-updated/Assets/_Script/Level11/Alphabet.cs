using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEleven
{
    /// <summary>
    /// Aplphabet class of Level 11.
    /// </summary>
    [AddComponentMenu("LevelEleven_Reezoo/Alphabet")]
    [RequireComponent(typeof(Text))]
    public class Alphabet : MonoBehaviour
    {

        #region public Variable
        /// <summary>
        /// Name of the Alphabet
        /// </summary>
        public string Name;

        /// <summary>
        /// Sound Of this Alphabet.
        /// This Audio must have this setting.
        /// Load Type : Decompress On Load
        /// Format : PCM
        /// </summary>
        public AudioClip SoundOfAlphabet;

        /// <summary>
        /// Action will be raised when .
        /// Appearence of the alphabet is complete .
        /// </summary>
        public Action<int> OnAppearComplete;


        /// <summary>
        /// Event raised when Alphabet Disappeared.
        /// </summary>
        public static event EventHandler AlphabetDisappeared;

        /// <summary>
        /// Corresponding Rect TransForm .That holds the letter Final position in the word.
        /// </summary>
        public RectTransform LetterFinalPositionInWord;

        /// <summary>
        /// Corresponding Rect TransForm.That holds the letter Initial position in the WordHolder.
        /// </summary>
        public RectTransform LetterInitialPositionInWord;

        /// <summary>
        /// Color Of the Alphabet When it is Single.
        /// </summary>
        public Color AlphabetOriginColor;
        /// <summary>
        /// Color of the Alphabet when it is Mingle.
        /// </summary>
        public Color AlphabetColorInWord;



        #endregion


        #region Normal Functions

        /// <summary>
        /// Init method.
        /// </summary>
        public void Init()
        {
            Debug.Log("Name" + gameObject.name);
            //Set color
            gameObject.GetComponent<Text>().color = AlphabetOriginColor;
            //show
            Show();
            //attach actions
            OnAppearComplete = PlayTheSoundOfAlphabet;
            //updated code .
            OnAppearComplete.Invoke(3);
        }
        /// <summary>
        /// Hide The letter Instantly making Alpha zero .
        /// </summary>
        public void Hide()
        {
           
            //Get the Image component and make alpha Zero.
            var textColor = this.gameObject.GetComponent<Text>().color;
            textColor.a = 0f;
            this.gameObject.GetComponent<Text>().color = textColor;
            //DeActivate Gameobject.
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show The letter Instantly Making Alpha 1 .
        /// </summary>
        public void Show()
        {
            //DeActivate Gameobject.
            this.gameObject.SetActive(true);
            Debug.Log("Name of Gameobject  "+gameObject.name);
            //Get the text component and make alpha one
            var textColor = this.gameObject.GetComponent<Text>().color;
            textColor.a = 1f;
            this.gameObject.GetComponent<Text>().color = textColor;
        }

        /// <summary>
        /// Make Letter Appear on the screen.
        /// </summary>
        public void Appear()
        {
            if (!this.gameObject.activeInHierarchy)
            {
                this.gameObject.SetActive(true);
            }
            var appearIenumerator = MakeTheLetterApperOrDisappear(0f, 0.2f, true);
            StartCoroutine(appearIenumerator);
        }

        /// <summary>
        /// Make letter Disappear on the screen.
        /// </summary>
        public void Disappear()
        {
            var disAppearIenumerator = MakeTheLetterApperOrDisappear(0f, 0.2f, false);
            StartCoroutine(disAppearIenumerator);
        }

        /// <summary>
        /// Make the letter Apper or Disappear .
        /// </summary>
        /// <param name="delay">control over frames</param>
        /// <param name="howFastItWillAppear">control over color</param>
        /// <param name="appear"> True : appear , Flase : Disappear  </param>
        /// <returns></returns>
        private IEnumerator MakeTheLetterApperOrDisappear(float delay, float howFastItWillAppear, bool appear = true)
        {
            //Get the color of gameobject
            Color colorOfImage = this.gameObject.GetComponent<Text>().color;
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
                    this.gameObject.GetComponent<Text>().color = updatedColor;

                    //Ieterate over a delay 
                    yield return new WaitForSeconds(delay);
                }
                //appearence of letter is comple play the sound 3 times .
                OnAppearComplete.Invoke(3);
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
                    this.gameObject.GetComponent<Text>().color = updatedColor;

                    //Ieterate over a delay 
                    yield return new WaitForSeconds(delay);
                }
                //letter disappear 
                OnDisAppearComplete(this,EventArgs.Empty);
            }
        }

        /// <summary>
        /// Playe the sound of the alphabet.
        /// </summary>
        /// <param name="numberOfTimes">number of times sound will be played</param>
        private void PlayTheSoundOfAlphabet(int numberOfTimes)
        {
            //play the sound
            
            //Add the sounds.
            for (var i = 0; i < numberOfTimes; i++)
            {
                //play the sound
                SoundController.Reference.Playclip(SoundOfAlphabet);
            }
            //wait for alphabet sound to complete.
            WaitForAllAlphabetSoundToComplete();


        }


        /// <summary>
        /// Event will be raised when The letter Disapper.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="eventArgs"></param>
        public void OnDisAppearComplete(object source,EventArgs eventArgs)
        {
            AlphabetDisappeared?.Invoke(source, eventArgs);
            
        }

        /// <summary>
        /// Wait for the time untill all sounds are played completely.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitFortheSoundTocmplete()
        {
            while (SoundController.Reference.IsMusicOnPlay)
            {
                yield return null;
            }
            //Make the word disappear .
            Disappear();
        }

        /// <summary>
        /// Wait for the time untill all sounds are played completely.
        /// </summary>
        private void WaitForAllAlphabetSoundToComplete()
        {
            var soundPlayIenumerator = WaitFortheSoundTocmplete();
            StartCoroutine(soundPlayIenumerator);

        }
        /// <summary>
        /// Move over the time.
        /// you can provide any time.
        /// But it is designed to provide the time equivalent to the length of the clip.
        /// </summary>
        public void MoveAlphabetOverTime(float lengthofTheClip,float percentage,Color ?desirecolColor = null)
        {
            var movealphabetIenumeration = MoveOverTime(lengthOfTime:lengthofTheClip,withdelay:0f,percentage:percentage,desireColor:desirecolColor);
            StartCoroutine(movealphabetIenumeration);
        }

        /// <summary>
        /// Move the alphabet over time.
        /// </summary>
        /// <param name="lengthOfTime">Iteration over this amount of time</param>
        /// <param name="withdelay">extra time gap added over each iteration.</param>
        /// <param name="percentage">percentage of total distance .If percentage is ignored then the corotine will complete the full distance</param>
        /// <param name="desireColor">This color will be achieved.</param>
        /// <returns></returns>
        private IEnumerator MoveOverTime(float lengthOfTime , float withdelay=0f,float percentage=100f,Color ?desireColor =null)
        {
            //if percentage is negative.
            if(percentage<0 && percentage>100)
                throw new OperationCanceledException();
            //percentage calculation
            percentage = percentage / 100;
            //Current time when we start.
            var currentTime = 0f;

            //Old position for max
            var oldPositionMax = gameObject.GetComponent<RectTransform>().offsetMax;
                //LetterInitialPositionInWord.offsetMax;

            //Old position for min
            var oldPositionMin = gameObject.GetComponent<RectTransform>().offsetMin;
                //LetterInitialPositionInWord.offsetMin;

            //final position for max
            var finalPositionMax = LetterFinalPositionInWord.offsetMax;

            //final position for min
            var finalPositionMin = LetterFinalPositionInWord.offsetMin;
            

            //desired position for offset max
            var desiredMax =  Vector2.Lerp(oldPositionMax, finalPositionMax, percentage);

            //desired position for offset min
            var desiredMin = Vector2.Lerp(oldPositionMin, finalPositionMin, percentage);

            //var desired color
            var desiredcolor = Color.Lerp(AlphabetOriginColor, AlphabetColorInWord, percentage);
            

            //Iterator block.
            while (currentTime <= lengthOfTime)
            {
                //Alphabet transition
                currentTime += Time.deltaTime;
                
                var normalizedValue = currentTime / lengthOfTime;
                gameObject.GetComponent<RectTransform>().offsetMax =
                    Vector2.Lerp(oldPositionMax, desiredMax, normalizedValue);
                

                gameObject.GetComponent<RectTransform>().offsetMin =
                    Vector2.Lerp(oldPositionMin, desiredMin, normalizedValue);
                
                
                //Color Transition
                if(desireColor==null)
                gameObject.GetComponent<Text>().color =
                    Color.Lerp(gameObject.GetComponent<Text>().color, desiredcolor, normalizedValue);
                        //Color.Lerp(AlphabetOriginColor, desiredcolor, normalizedValue);
                else
                {
                    gameObject.GetComponent<Text>().color =
                        Color.Lerp(gameObject.GetComponent<Text>().color, desireColor.GetValueOrDefault(), normalizedValue);
                }


                yield return new WaitForSeconds(withdelay);
            }
            
        }
        /// <summary>
        /// Make alphabet ready for word.
        /// </summary>
        public void MakeAlphabetReadyForWord()
        {
            //fist activate the alphabet
            Show();
            //Bring alphabet at initial position to create word
            this.gameObject.GetComponent<RectTransform>().offsetMax = LetterInitialPositionInWord.offsetMax;
            this.gameObject.GetComponent<RectTransform>().offsetMin = LetterInitialPositionInWord.offsetMin;
           
        }
        #endregion
    }
}