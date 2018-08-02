using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Level 6 Controller
/// </summary>
namespace Level6
{
    /// <summary>
    /// Level Six Controller 
    /// This class is responsible for controlling level 6
    /// </summary>
    public class LevelSixController : MonoBehaviour
    {

        #region For Public Fields

        /// <summary>
		/// The exercise gameobject.
		/// This list holds all Exercise gameobject in which letters are child Gameobject.
		/// </summary>
        public List<GameObject> exercises;

        /// <summary>
        /// The mesh attached to the dinosaur teacher.
        /// </summary>
        public SkinnedMeshRenderer pachyMesh;

        /// <summary>
        /// The "Let's Make Words" audio clip.
        /// </summary>
        public AudioClip letsMakeWords;

        /// <summary>
        /// The "What Sounds Do you Hear When I say..." audio clip.
        /// </summary>
        public AudioClip whatSoundsDoYouHear;

        /// <summary>
        /// The "Click on the sound that you hear when I say..." audio clip.
        /// </summary>
        public AudioClip clickOnTheSound;

        /// <summary>
		/// The reward audio clips.
		/// </summary>
		public AudioClip[] rewardAudioClip;

        /// <summary>
        /// The color to change to when a letter is clicked.
        /// </summary>
        public Color clickedColor;

        /// <summary>
        /// The color that the letter is before being clicked.
        /// </summary>
        public Color idleColor;

        /// <summary>
        /// The holders that the letters translate to when clicked.
        /// </summary>
        public GameObject[] letterHolders = new GameObject[3];

        /// <summary>
        /// All the letters on the canvas
        /// </summary>
        //public GameObject[] letters;

        /// <summary>
        /// All of the exercises
        /// </summary>
        public MultiDimensionalArray[] exercisesAndWords;
        #endregion

        #region For Private Fields

        /// <summary>
        /// The time it takes before it repeats the word
        /// </summary>
        private const float REPEAT_TIMER = 5f;

        /// <summary>
        /// The amount we should lerp
        /// </summary>
        private const float LERP_AMOUNT = 0.2f;

        /// <summary>
        /// The time it takes to complete the jumping letters animation
        /// </summary>
        private const float ANIMATION_TIME = 1.4f;

        /// <summary>
        /// The attached audio source
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// The letters the player has selected correctly and their original position
        /// </summary>
        private Dictionary<GameObject, Vector2> lettersSelected = new Dictionary<GameObject, Vector2>();

        /// <summary>
        /// A list of all the words in every exercise
        /// </summary>
        private List<Word[]> allWords = new List<Word[]>();

        /// <summary>
        /// The coroutine to move the letter to the center of the screen
        /// </summary>
        private Coroutine moveLetter;

        /// <summary>
        /// The coroutine to make the current word
        /// </summary>
        private Coroutine makeWord;

        /// <summary>
        /// The coroutine that runs when the player runs out of tries
        /// </summary>
        private Coroutine usedUpTries;

        /// <summary>
        /// The coroutine that runs when the player clicks on the teacher
        /// </summary>
        private Coroutine teacherHelp;

        /// <summary>
        /// The current word the player is making
        /// </summary>
        private Word currentWord;

        /// <summary>
        /// Predicate to check if audio is playing
        /// </summary>
        private Func<bool> IsPlaying;

        /// <summary>
        /// The number of tries the player has left
        /// </summary>
        private int triesLeft;

        /// <summary>
        /// The number of words the player has gotten wrong during this exercise
        /// </summary>
        private int wordsMissed;

        /// <summary>
        /// The index of the current word
        /// </summary>
        private int currentWordIndex;

        /// <summary>
        /// The current exercise the player is on
        /// </summary>
        private int currentExerciseIndex;

        /// <summary>
        /// The number of times the player was told to click on the sound they hear.
        /// </summary>
        private int timesThePlayerReceivedInstruction;

        /// <summary>
        /// The time since the last action was taken
        /// </summary>
        private float timer;

        /// <summary>
        /// Check to see if the player can click on letters
        /// </summary>
        private bool canChoose;

        /// <summary>
        /// Checks if the player got the word wrong and has to wait for the teacher
        /// </summary>
        private bool gotWordWrong;

        #endregion

        #region For Unity Message System

        /// <summary>
		/// Start this instance.
		/// </summary>
        void Start()
        {
            Init();
        }

        /// <summary>
        /// Called once per frame
        /// </summary>
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && canChoose)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Letter"))
                        {
                            OnLetterClicked(hit.collider.gameObject);
                        }
                    }
                }
                else
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = pachyMesh.transform.position.z;
                    if (pachyMesh.bounds.Contains(mousePos))
                        teacherHelp = StartCoroutine(PlayHelpAudio());
                }
            }
            //if (canChoose)
            //    timer += Time.deltaTime;

            //if (timer >= 20)
            //    PlayHelpAudio();
        }

        #endregion

        #region For Functions

        /// <summary>
        /// Initialize this instance
        /// </summary>
        private void Init()
        {
            for (int i = 0; i < exercisesAndWords.Length; i++)
            {
                allWords.Add(exercisesAndWords[i].words);
            }
            currentWord = allWords[currentExerciseIndex][currentWordIndex];
            exercises[0].SetActive(true);
            triesLeft = 3;
            canChoose = false;
            gotWordWrong = false;
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = letsMakeWords;
            audioSource.Play();
            IsPlaying = () => audioSource.isPlaying;
            makeWord = StartCoroutine(MakeWord(currentWord.clips));
        }

        /// <summary>
        /// Starts a word for the player to try and make
        /// </summary>
        private IEnumerator MakeWord(AudioClip[] wordClips)
        {
            canChoose = false;

            if (audioSource.isPlaying)
                yield return new WaitWhile(IsPlaying);

            audioSource.clip = whatSoundsDoYouHear;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            audioSource.clip = wordClips[2];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            if (timesThePlayerReceivedInstruction < 2)
            {
                timesThePlayerReceivedInstruction++;
                audioSource.clip = clickOnTheSound;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
                audioSource.clip = wordClips[2];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
            canChoose = true;

            yield return new WaitWhile(IsPlaying);
            audioSource.clip = wordClips[2];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);

            while (canChoose)
            {
                yield return new WaitForSeconds(REPEAT_TIMER);
                if (audioSource.isPlaying)
                    yield return new WaitWhile(IsPlaying);

                audioSource.clip = wordClips[2];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
                yield return new WaitForSeconds(REPEAT_TIMER);

                if (audioSource.isPlaying)
                    yield return new WaitWhile(IsPlaying);

                audioSource.clip = clickOnTheSound;
                audioSource.Play();
                yield return new WaitWhile(IsPlaying);
                audioSource.clip = wordClips[4];
                audioSource.Play();
            }
        }

        /// <summary>
        /// Called when a letter on the canvas is clicked
        /// </summary>
        /// <param name="letter"></param>
        private void OnLetterClicked(GameObject letter)
        {
            if (canChoose && !lettersSelected.ContainsKey(letter) && !gotWordWrong)
            {
                timer = 0;
                Letter[] letters = currentWord.letters;
                for (int i = 0; i < letters.Length; i++)
                {
                    if (letter.name.ToLower().Equals(letters[i].letter.name.ToLower()))
                    {
                        moveLetter = StartCoroutine(MoveLetter(letter, letterHolders[i]));
                        return;
                    }
                }
                audioSource.clip = currentWord.clips[3];
                audioSource.Play();
                ResetLetters();
                triesLeft--;
                if (triesLeft <= 0)
                    usedUpTries = StartCoroutine(UsedUpTries());
            }
        }

        /// <summary>
        /// Moves the given letter to the center of the screen
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        private IEnumerator MoveLetter(GameObject letter, GameObject letterHolder)
        {
            canChoose = false;
            MeshRenderer meshRenderer = letter.GetComponentInChildren<MeshRenderer>();
            meshRenderer.material.color = clickedColor;
            meshRenderer.material.SetColor("_EmissionColor", clickedColor);
            RectTransform letterTransform = letter.GetComponent<RectTransform>();
            RectTransform holderTransform = letterHolder.GetComponent<RectTransform>();
            if (!lettersSelected.ContainsKey(letter))
                lettersSelected.Add(letter, letterTransform.anchoredPosition);

            bool isClose = false;
            while (!isClose)
            {
                letterTransform.anchoredPosition = Vector2.Lerp(letterTransform.anchoredPosition, holderTransform.anchoredPosition, LERP_AMOUNT);
                if (Vector2.Distance(letterTransform.anchoredPosition, holderTransform.anchoredPosition) <= 5)
                {
                    isClose = true;
                    letterTransform.anchoredPosition = holderTransform.anchoredPosition;
                }

                yield return new WaitForEndOfFrame();
            }

            StartCoroutine(CheckWordCompletion());
        }

        /// <summary>
        /// Checks if the player has completed the current word
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckWordCompletion()
        {
            Letter[] letters = currentWord.letters;

            canChoose = true;
            if (letters.Length != lettersSelected.Count || triesLeft == 0)
                yield break;

            int randomRewardClip = UnityEngine.Random.Range(0, rewardAudioClip.Length);
            audioSource.clip = rewardAudioClip[randomRewardClip];
            audioSource.Play();

            foreach (GameObject go in lettersSelected.Keys)
            {
                go.GetComponent<LetterAnimator>().Animate();
            }

            yield return new WaitForSeconds(ANIMATION_TIME);

            foreach (GameObject go in lettersSelected.Keys)
            {
                go.GetComponent<LetterAnimator>().StopAnimation();
            }

            canChoose = false;
            StopCoroutine(makeWord);
            if (teacherHelp != null)
                StopCoroutine(teacherHelp);

            triesLeft = 3;


            yield return new WaitWhile(IsPlaying);
            yield return new WaitForSeconds(ANIMATION_TIME);

            ResetLetters();
            MoveToNextWord();
        }

        /// <summary>
        /// Called when the player has used up their three tries
        /// </summary>
        private IEnumerator UsedUpTries()
        {
            canChoose = false;
            gotWordWrong = true;
            StopCoroutine(makeWord);
            wordsMissed++;
            if (wordsMissed >= 2)
            {
                StopAllCoroutines();
                SceneManager.LoadScene("Level3");
                yield break;
            }

            Letter[] letters = currentWord.letters;
            for (int i = 0; i < letters.Length; i++)
            {
                moveLetter = StartCoroutine(MoveLetter(letters[i].letter, letterHolders[i]));
                audioSource.clip = letters[i].clip;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
            audioSource.clip = currentWord.clips[3];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            ResetLetters();
            MoveToNextWord();
            triesLeft = 3;
            gotWordWrong = false;
        }

        /// <summary>
        /// Runs when player clicks on teacher
        /// </summary>
        private IEnumerator PlayHelpAudio()
        {
            foreach (Letter l in currentWord.letters)
            {
                audioSource.clip = l.clip;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
            audioSource.clip = currentWord.clips[3];
            audioSource.Play();
        }

        /// <summary>
        /// Used to move on to the next word
        /// </summary>
        private void MoveToNextWord()
        {
            currentWordIndex++;
            if (allWords[currentExerciseIndex].Length == currentWordIndex)
            {
                wordsMissed = 0;
                triesLeft = 3;
                exercises[currentExerciseIndex].SetActive(false);
                currentWordIndex = 0;
                currentExerciseIndex++;
                if (exercises.Count == currentExerciseIndex)
                {
                    StopAllCoroutines();
                    Scene currentScene = SceneManager.GetActiveScene();
                    int nextLevel = int.Parse(currentScene.name.Substring(6)) + 1;
                    SceneManager.LoadScene("Level" + nextLevel);
                    return;
                }
                exercises[currentExerciseIndex].SetActive(true);
            }
            currentWord = allWords[currentExerciseIndex][currentWordIndex];
            makeWord = StartCoroutine(MakeWord(currentWord.clips));
        }

        /// <summary>
        /// Resets the letters to their original position and clears the dictionary.
        /// </summary>
        private void ResetLetters()
        {
            if (lettersSelected.Count > 0)
            {
                foreach (GameObject let in lettersSelected.Keys)
                {
                    RectTransform rectTransform = let.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = lettersSelected[let];
                    MeshRenderer meshRenderer = let.GetComponentInChildren<MeshRenderer>();
                    meshRenderer.material.color = idleColor;
                    meshRenderer.material.SetColor("_EmissionColor", idleColor);
                }
            }
            lettersSelected.Clear();
        }

        #endregion
    }
}
