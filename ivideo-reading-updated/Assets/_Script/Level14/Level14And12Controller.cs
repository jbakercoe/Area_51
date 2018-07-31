using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level14And12Controller : MonoBehaviour {
    #region PUBLIC VARIABLE
    //Make class WordData as array to contain list of words used in exercise 14
    public WordData[] wordData;

    //List of voiceover sounds used
    public AudioClip weAreGoingToMakeWordsAgain;
    public AudioClip chooseAPicture;
    public AudioClip chooseAPictureAndMakeTheWord;
    public AudioClip chooseTheSoundsThatYouHearTheWord;
    public AudioClip iWillDoTheFirstOne;
    public AudioClip nowItsYourTurnDoIt;
    public AudioClip sayItWithMe;
    public AudioClip whichLetterMakesThisSound;
    public AudioClip[] rightAnswer;
    public AudioClip wrongAnswer;

    //Voiceover player
    public AudioSource teacherAudio;
    public GameObject teacherExerciseComplete;

    public Animator triceratopAnimator;

    //List of random words location appearing on the screen
    public GameObject[] randomWordLocation;
    //Gameobject to contain list of alphabets on the bottom screen
    public GameObject alphabetContainer;
    //Gameobject to contain list of picked alphabets on the middle screen
    public GameObject middleAlphabetContainer;

    public int alphabetSpeed;
    public bool is3D;
    #endregion

    #region PRIVATE VARIABLE
    //Number of chances to pick alphabet, default number is 3
    private int pickAlphabetChance;

    //Index word picked by player for WordData array
    private int wordIndex;

    //Index for random words location
    private int randLocation;

    //Index for random picture chosen by teacher for demo purpose
    private int randDemo;

    //Number of alphabet picked for checking the right or wrong answer
    private int totalPickedAlphabets;

    //List of unique words based on number of words in WordData array class
    private List<int> uniqueWords;

    //List of unique random words to display on the screen
    private List<int> randomUniqueWords;

    //List of alphabet order picked by player
    private List<string> playerAlphabetsOrder;

    //Check if player has done picking word
    private bool donePickingWord;

    private bool donePlayingAlphabetsSound;

    //Check if player has done picking alphabets
    private bool donePickingAlphabet;

    private bool donePickingSingleAlphabet;

    private bool isAlphabetWrongFirstTime;

    //Check if alphabet is correct or false
    private bool isAlphabetCorrect;

    private bool isSingleAlphabetCorrect;

    //Check if player has done demo
    [SerializeField]
    private bool doneDemo;

    #endregion

    void Awake() {
        uniqueWords = new List<int>();
        randomUniqueWords = new List<int>();
        playerAlphabetsOrder = new List<string>();
    }
    void Start() {
        Init();
        StartCoroutine(GameFlow());
    }

    #region GENERAL
    /// <summary>
    /// Method to init default state of gameobjects and to add unique words
    /// </summary>
    private void Init() {
        for (int i = 0; i < randomWordLocation.Length; i++)
        {
            randomWordLocation[i].SetActive(false);
        }
        alphabetContainer.SetActive(false);
        for (int i = 0; i < wordData.Length; i++)
        {
            uniqueWords.Add(i);
        }
    }
    /// <summary>
    /// Reset all variables used to their default state
    /// </summary>
    private void Reset() {
        randomUniqueWords.Clear();
        playerAlphabetsOrder.Clear();

        donePickingWord = false;
        donePlayingAlphabetsSound = false;
        donePickingAlphabet = false;
        donePickingSingleAlphabet = false;
        isAlphabetCorrect = false;
        isSingleAlphabetCorrect = false;
        isAlphabetWrongFirstTime = false;

        wordIndex = 0;
        randLocation = 0;
        randDemo = 0;
        pickAlphabetChance = 3;
        totalPickedAlphabets = 0;
    }
    /// <summary>
    /// Generates unique random numbers
    /// </summary>
    private void GenerateUniqueNumber(List<int> finishedNumbers, int totalNumbers, int totalRandomNumbers)
    {
        List<int> baseNumbers = new List<int>();
        for (int i = 0; i < totalNumbers; i++)
        {
            baseNumbers.Add(i);
        }

        for (int i = 0; i < totalRandomNumbers; i++)
        {
            int randomNumber = baseNumbers[Random.Range(0, baseNumbers.Count)];
            finishedNumbers.Add(randomNumber);
            baseNumbers.Remove(randomNumber);
        }
    }
    private void HideTriceratop() {
        /*triceratopsStationary.SetActive(false);
        triceratopsTraceTheLetters.SetActive(false);
        triceratopsCanYouFindTheWord.SetActive(false);
        triceratopsOneWord.SetActive(false);*/
    }
    private void DisplayTriceratop(GameObject triceratop) {
        /*if (is3D)
        {
            HideTriceratop();
            triceratop.SetActive(true);
        }*/
    }
    private void PlayTriceratopAnimation(string triggerName, bool triggerType) {
        if (is3D)
        {
            triceratopAnimator.SetBool(triggerName, triggerType);
        }
    }
    /// <summary>
    /// Core gameplay
    /// </summary>
    private IEnumerator GameFlow() {
        //Opening Section
        teacherAudio.clip = weAreGoingToMakeWordsAgain;
        teacherAudio.Play();
        //LevelEleven.SoundController.Reference.Playclip(weAreGoingToMakeWordsAgain);
        PlayTriceratopAnimation("Start With", true);
        yield return new WaitForSeconds(weAreGoingToMakeWordsAgain.length);
        PlayTriceratopAnimation("Start With", false);

        while (uniqueWords.Count > 0)
        {
            //Picture Section
            DisplayRandomWords();
            DisableRandomWordsButton();
            if (doneDemo == false)
            {
                //Teacher will demo first by picking random picture
                teacherAudio.clip = chooseAPictureAndMakeTheWord;
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(chooseAPictureAndMakeTheWord);
                PlayTriceratopAnimation("Start With", true);
                yield return new WaitForSeconds(chooseAPictureAndMakeTheWord.length);
                PlayTriceratopAnimation("Start With", false);

                teacherAudio.clip = iWillDoTheFirstOne;
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(iWillDoTheFirstOne);
                PlayTriceratopAnimation("Do It First", true);
                yield return new WaitForSeconds(iWillDoTheFirstOne.length);
                PlayTriceratopAnimation("Do It First", false);
                randDemo = Random.Range(0, randomUniqueWords.Count);
                yield return new WaitForSeconds(1);
                if (is3D)
                {
                    randomWordLocation[randLocation].transform.GetChild(randDemo).GetChild(0).Find("Interact").GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    randomWordLocation[randLocation].transform.GetChild(randDemo).GetComponent<Image>().color = Color.white;
                    randomWordLocation[randLocation].transform.GetChild(randDemo).GetComponent<Button>().onClick.Invoke();
                }
            }
            else
            {
                //Player will choose which picture to play
                EnableRandomWordsButton();
                teacherAudio.clip = chooseAPictureAndMakeTheWord;
                //While player hasn't picked picture, the teacher will repeat above sound
                while (donePickingWord == false)
                {
                    teacherAudio.Play();
                    PlayTriceratopAnimation("Start With", true);
                    int timer = 10;
                    while (timer > 0)
                    {
                        timer -= 1;
                        if (timer == 6)
                        {
                            PlayTriceratopAnimation("Start With", false);
                        }
                        if (donePickingWord)
                        {
                            PlayTriceratopAnimation("Start With", false);
                            break;
                        }
                        else
                        {
                            yield return new WaitForSeconds(1);
                        }
                    }
                }
            }
            //Alphabet Section
            DisplayAlphabets();
            teacherAudio.Stop();
            PlayTriceratopAnimation("Start With", false);
            //LevelEleven.SoundController.Reference.StopClip();
            if (doneDemo == false)
            {
                DisableAlphabets();
                teacherAudio.clip = wordData[wordIndex].soundVer1;
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                PlayTriceratopAnimation("1 Syllable WordHolder", true);
                yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                PlayTriceratopAnimation("1 Syllable WordHolder", false);

                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                PlayTriceratopAnimation("1 Syllable WordHolder", true);
                yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                PlayTriceratopAnimation("1 Syllable WordHolder", false);

                teacherAudio.clip = chooseTheSoundsThatYouHearTheWord;
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(chooseTheSoundsThatYouHearTheWord);
                PlayTriceratopAnimation("Start With", true);
                yield return new WaitForSeconds(chooseTheSoundsThatYouHearTheWord.length);
                PlayTriceratopAnimation("Start With", false);

                teacherAudio.clip = wordData[wordIndex].soundVer1;
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                PlayTriceratopAnimation("1 Syllable WordHolder", true);
                yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                PlayTriceratopAnimation("1 Syllable WordHolder", false);

                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                PlayTriceratopAnimation("1 Syllable WordHolder", true);
                yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                PlayTriceratopAnimation("1 Syllable WordHolder", false);

                //Teacher will demo first by picking alphabets in correct order
                for (int i = 0; i < wordData[wordIndex].alphabets.Length; i++)
                {
                    PickAlphabets(wordData[wordIndex].alphabets[i]);
                    yield return new WaitForSeconds(1);
                }
                teacherAudio.clip = nowItsYourTurnDoIt;
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(nowItsYourTurnDoIt);
                PlayTriceratopAnimation("Start With", true);
                yield return new WaitForSeconds(nowItsYourTurnDoIt.length);
                PlayTriceratopAnimation("Start With", false);
                donePlayingAlphabetsSound = true;
                doneDemo = true;
                totalPickedAlphabets = 0;
                HideMiddleAlphabets();
                EnableAlphabets();
            }
            else
            {
                EnableAlphabets();
                while(donePlayingAlphabetsSound == false) 
                {
                    teacherAudio.clip = wordData[wordIndex].soundVer1;
                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                    PlayTriceratopAnimation("1 Syllable WordHolder", true);
                    if (donePickingAlphabet)
                    {
                        PlayTriceratopAnimation("1 Syllable WordHolder", false);
                        break;
                    }
                    yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                    PlayTriceratopAnimation("1 Syllable WordHolder", false);

                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                    PlayTriceratopAnimation("1 Syllable WordHolder", true);
                    if (donePickingAlphabet)
                    {
                        PlayTriceratopAnimation("1 Syllable WordHolder", false);
                        break;
                    }
                    yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                    PlayTriceratopAnimation("1 Syllable WordHolder", false);

                    teacherAudio.clip = chooseTheSoundsThatYouHearTheWord;
                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(chooseTheSoundsThatYouHearTheWord);
                    PlayTriceratopAnimation("Start With", true);
                    float audioLength = chooseTheSoundsThatYouHearTheWord.length;
                    while (audioLength > 0)
                    {
                        if (donePickingAlphabet)
                        {
                            //teacherAudio.Stop();
                            PlayTriceratopAnimation("Start With", false);
                            break;
                        }
                        else
                        {
                            audioLength -= 1;
                            yield return new WaitForSeconds(1);
                        }
                    }
                    PlayTriceratopAnimation("Start With", false);

                    teacherAudio.clip = wordData[wordIndex].soundVer1;
                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                    PlayTriceratopAnimation("1 Syllable WordHolder", true);
                    if (donePickingAlphabet)
                    {
                        PlayTriceratopAnimation("1 Syllable WordHolder", false);
                        break;
                    }
                    yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                    PlayTriceratopAnimation("1 Syllable WordHolder", false);

                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].soundVer1);
                    PlayTriceratopAnimation("1 Syllable WordHolder", true);
                    if (donePickingAlphabet)
                    {
                        PlayTriceratopAnimation("1 Syllable WordHolder", false);
                        break;
                    }
                    yield return new WaitForSeconds(wordData[wordIndex].soundVer1.length);
                    PlayTriceratopAnimation("1 Syllable WordHolder", false);
                    donePlayingAlphabetsSound = true;
                }
                teacherAudio.Stop();
                //LevelEleven.SoundController.Reference.StopClip();
                donePlayingAlphabetsSound = true;
                yield return new WaitUntil(() => donePickingAlphabet == true);
            }
            yield return new WaitUntil(() => totalPickedAlphabets == wordData[wordIndex].alphabets.Length);
            if (isAlphabetCorrect)
            {
                DisableAlphabets();
                int randSuperlative = Random.Range(0, rightAnswer.Length);
                teacherAudio.clip = rightAnswer[randSuperlative];
                teacherAudio.Play();
                //LevelEleven.SoundController.Reference.Playclip(rightAnswer[randSuperlative]);
                PlayTriceratopAnimation("Start With", true);
                yield return new WaitForSeconds(rightAnswer[randSuperlative].length);
                PlayTriceratopAnimation("Start With", false);
                for (int i = 0; i < wordData[wordIndex].alphabetsSound.Length; i++)
                {
                    middleAlphabetContainer.transform.GetChild(i).GetComponent<Animator>().SetTrigger("Spin");
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                DisableAlphabets();
                playerAlphabetsOrder.Clear();
                while (pickAlphabetChance > 0)
                {
                    donePickingSingleAlphabet = false;
                    if (isSingleAlphabetCorrect == false)
                    {
                        playerAlphabetsOrder.Clear();
                        totalPickedAlphabets = 0;
                        teacherAudio.clip = wrongAnswer;
                        teacherAudio.Play();
                        //LevelEleven.SoundController.Reference.Playclip(wrongAnswer);
                        PlayTriceratopAnimation("Start With", true);
                        yield return new WaitForSeconds(wrongAnswer.length);
                        PlayTriceratopAnimation("Start With", false);
                        HideMiddleAlphabets();
                        pickAlphabetChance -= 1;
                        if (pickAlphabetChance <= 0)
                        {
                            break;
                        }
                    }
                    DisableAlphabets();
                    teacherAudio.clip = whichLetterMakesThisSound;
                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(whichLetterMakesThisSound);
                    PlayTriceratopAnimation("Start With", true);
                    yield return new WaitForSeconds(whichLetterMakesThisSound.length);
                    PlayTriceratopAnimation("Start With", false);

                    teacherAudio.clip = wordData[wordIndex].alphabetsSound[totalPickedAlphabets];
                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].alphabetsSound[totalPickedAlphabets]);
                    PlayTriceratopAnimation("1 Syllabel WordHolder", true);
                    yield return new WaitForSeconds(wordData[wordIndex].alphabetsSound[totalPickedAlphabets].length);
                    PlayTriceratopAnimation("1 Syllabel WordHolder", false);
                    EnableAlphabets();
                    yield return new WaitUntil(() => donePickingSingleAlphabet == true);
                    if (totalPickedAlphabets >= wordData[wordIndex].alphabets.Length)
                    {
                        break;
                    }
                }
                DisableAlphabets();
                CheckAlphabets();
                if (isAlphabetCorrect)
                {
                    int randSuperlative = Random.Range(0, rightAnswer.Length);
                    teacherAudio.clip = rightAnswer[randSuperlative];
                    teacherAudio.Play();
                    //LevelEleven.SoundController.Reference.Playclip(rightAnswer[randSuperlative]);
                    PlayTriceratopAnimation("Start With", true);
                    yield return new WaitForSeconds(rightAnswer[randSuperlative].length);
                    PlayTriceratopAnimation("Start With", false);
                    for (int i = 0; i < wordData[wordIndex].alphabetsSound.Length; i++)
                    {
                        middleAlphabetContainer.transform.GetChild(i).GetComponent<Animator>().SetTrigger("Spin");
                        yield return new WaitForSeconds(1);
                    }
                }
                else
                {
                    HideMiddleAlphabets();
                    for (int i = 0; i < wordData[wordIndex].alphabets.Length; i++)
                    {
                        while (Vector2.Distance(alphabetContainer.transform.Find(wordData[wordIndex].alphabets[i]).position, middleAlphabetContainer.transform.GetChild(i).position) > 0.05f)
                        {
                            alphabetContainer.transform.Find(wordData[wordIndex].alphabets[i]).position = Vector2.MoveTowards(alphabetContainer.transform.Find(wordData[wordIndex].alphabets[i]).position, middleAlphabetContainer.transform.GetChild(i).position, Time.fixedDeltaTime * alphabetSpeed);
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                    for (int i = 0; i < wordData[wordIndex].alphabetsSound.Length; i++)
                    {
                        teacherAudio.clip = wordData[wordIndex].alphabetsSound[i];
                        teacherAudio.Play();
                        //LevelEleven.SoundController.Reference.Playclip(wordData[wordIndex].alphabetsSound[i]);
                        PlayTriceratopAnimation("1 Syllable WordHolder", true);
                        yield return new WaitForSeconds(wordData[wordIndex].alphabetsSound[i].length);
                        PlayTriceratopAnimation("1 Syllable WordHolder", false);
                    }
                }
            }
            //Reset the game and continue for the next word
            yield return new WaitForSeconds(1);
            HideAlphabets();
            HideMiddleAlphabets();
            Reset();
            yield return new WaitForSeconds(1);
        }
        //If player has completed all words then go to next exercise
        teacherExerciseComplete.SetActive(true);
    }
    #endregion

    #region WORDS
    /// <summary>
    /// Display random words at random location on the screen
    /// </summary>
    private void DisplayRandomWords() {
        if (uniqueWords.Count >= 3)
        {
            GenerateUniqueNumber(randomUniqueWords, uniqueWords.Count, 3);
        }
        else
        {
            GenerateUniqueNumber(randomUniqueWords, uniqueWords.Count, uniqueWords.Count);
        }
        randLocation = Random.Range(0, randomWordLocation.Length);
        randomWordLocation[randLocation].SetActive(true);
        if (is3D)
        {
            for (int i = 0; i < randomUniqueWords.Count; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).gameObject.SetActive(true);
                randomWordLocation[randLocation].transform.GetChild(i).gameObject.GetComponent<Image>().enabled = false;
                randomWordLocation[randLocation].transform.GetChild(i).gameObject.GetComponent<Button>().enabled = false;
                GameObject instantiateWord = Instantiate(wordData[uniqueWords[randomUniqueWords[i]]].model, randomWordLocation[randLocation].transform.GetChild(i)) as GameObject;
                instantiateWord.transform.SetParent(randomWordLocation[randLocation].transform.GetChild(i), false);
                int root = i;
                instantiateWord.transform.Find("Interact").GetComponent<Button>().onClick.RemoveAllListeners();
                instantiateWord.transform.Find("Interact").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PickWord(uniqueWords[randomUniqueWords[root]]);
                    });
            }
        }
        else
        {
            for (int i = 0; i < randomUniqueWords.Count; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).gameObject.SetActive(true);
                randomWordLocation[randLocation].transform.GetChild(i).name = wordData[uniqueWords[randomUniqueWords[i]]].name;
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Image>().sprite = wordData[uniqueWords[randomUniqueWords[i]]].icon;
                int root = i;
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PickWord(uniqueWords[randomUniqueWords[root]]);
                    });
            }
        }
    }
    /// <summary>
    /// Hide random words
    /// </summary>
    private void HideRandomWords() {
        randomWordLocation[randLocation].SetActive(false);
        if (is3D)
        {
            for (int i = 0; i < randomWordLocation[randLocation].transform.childCount; i++)
            {
                Destroy(randomWordLocation[randLocation].transform.GetChild(i).GetChild(0).gameObject);
            }
        }
        else
        {
            for (int i = 0; i < randomWordLocation[randLocation].transform.childCount; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Enable random words button
    /// </summary>
    private void EnableRandomWordsButton() {
        if (is3D)
        {
            for (int i = 0; i < randomWordLocation[randLocation].transform.childCount; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).GetChild(0).Find("Interact").GetComponent<Button>().enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < randomWordLocation[randLocation].transform.childCount; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Image>().color = Color.white;
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Button>().enabled = true;
            }
        }
    }
    /// <summary>
    /// Disable random words button
    /// </summary>
    private void DisableRandomWordsButton() {
        if (is3D)
        {
            for (int i = 0; i < randomWordLocation[randLocation].transform.childCount; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).GetChild(0).Find("Interact").GetComponent<Button>().enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < randomWordLocation[randLocation].transform.childCount; i++)
            {
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Image>().color = Color.gray;
                randomWordLocation[randLocation].transform.GetChild(i).GetComponent<Button>().enabled = false;
            }
        }
    }
    /// <summary>
    /// Pick word
    /// </summary>
    public void PickWord(int index) {
        uniqueWords.Remove(index);
        wordIndex = index;
        donePickingWord = true;
        HideRandomWords();
    }
    #endregion

    #region ALPHABETS
    /// <summary>
    /// Display alphabets
    /// </summary>
    private void DisplayAlphabets() {
        alphabetContainer.SetActive(true);
        for (int i = 0; i < alphabetContainer.transform.childCount; i++)
        {
            string root = alphabetContainer.transform.GetChild(i).name;
            alphabetContainer.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            alphabetContainer.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => PickAlphabets(root));
        }
    }
    /// <summary>
    /// Hide alphabets and reset bottom alphabets color
    /// </summary>
    private void HideAlphabets() {
        alphabetContainer.SetActive(false);
        for (int i = 0; i < alphabetContainer.transform.childCount; i++)
        {
            alphabetContainer.transform.GetChild(i).GetComponent<Text>().color = new Color(255/255f, 237/255f, 0);
        }
    }
    /// <summary>
    /// Hide middle alphabets
    /// </summary>
    private void HideMiddleAlphabets() {
        for (int i = 0; i < middleAlphabetContainer.transform.childCount; i++)
        {
            middleAlphabetContainer.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Enable alphabets button and reset bottom alphabets color
    /// </summary>
    private void EnableAlphabets() {
        for (int i = 0; i < alphabetContainer.transform.childCount; i++)
        {
            alphabetContainer.transform.GetChild(i).GetComponent<Text>().color = new Color(255/255f, 237/255f, 0);
            alphabetContainer.transform.GetChild(i).GetComponent<Button>().enabled = true;
        }
    }
    /// <summary>
    /// Disable alphabets button
    /// </summary>
    private void DisableAlphabets() {
        for (int i = 0; i < alphabetContainer.transform.childCount; i++)
        {
            alphabetContainer.transform.GetChild(i).GetComponent<Button>().enabled = false;
        }
    }
    /// <summary>
    /// Check if player picked alphabets are correct or not
    /// </summary>
    private void CheckAlphabets() {
        if (totalPickedAlphabets == wordData[wordIndex].alphabets.Length)
        {
            int totalCorrectAlphabets = 0;
            for (int i = 0; i < playerAlphabetsOrder.Count; i++)
            {
                if (playerAlphabetsOrder[i] == wordData[wordIndex].alphabets[i])
                {
                    totalCorrectAlphabets += 1;
                }
            }
            if (totalCorrectAlphabets == playerAlphabetsOrder.Count)
            {
                isAlphabetCorrect = true;
            }
            else
            {
                isAlphabetCorrect = false;
                isAlphabetWrongFirstTime = true;
            }
            //totalPickedAlphabets = 0;
        }
    }
    /// <summary>
    /// Pick alphabets and put them in the middle screen
    /// Also change picked alphabets color to pink
    /// </summary>
    public void PickAlphabets(string alphabet) {
        donePickingAlphabet = true;
        if (doneDemo == false)
        {
            alphabetContainer.transform.Find(alphabet).GetComponent<Text>().color = new Color(186/255f, 20/255f, 135/255f);
            middleAlphabetContainer.transform.GetChild(totalPickedAlphabets).gameObject.SetActive(true);
            middleAlphabetContainer.transform.GetChild(totalPickedAlphabets).GetComponent<Text>().text = alphabet.ToLower();
            totalPickedAlphabets += 1;
        }
        else
        {
            playerAlphabetsOrder.Add(alphabet);
            alphabetContainer.transform.Find(alphabet).GetComponent<Text>().color = new Color(186/255f, 20/255f, 135/255f);
            alphabetContainer.transform.Find(alphabet).GetComponent<Button>().enabled = false;
            middleAlphabetContainer.transform.GetChild(totalPickedAlphabets).gameObject.SetActive(true);
            middleAlphabetContainer.transform.GetChild(totalPickedAlphabets).GetComponent<Text>().text = alphabet.ToLower();
            if (isAlphabetWrongFirstTime)
            {
                //If player has chosen wrong alphabets once
                Debug.Log("Player has chosen wrong alphabets once");
                if (alphabet != wordData[wordIndex].alphabets[totalPickedAlphabets])
                {
                    isSingleAlphabetCorrect = false;
                }
                else
                {
                    isSingleAlphabetCorrect = true;
                }
                donePickingSingleAlphabet = true;
                totalPickedAlphabets += 1;
            }
            else
            {
                totalPickedAlphabets += 1;
                CheckAlphabets();
            }
        }
    }
    #endregion
}
[System.Serializable]
/*Class for list of words used in Level 14
 *If there are other exercises using system similar to level 14, i will create new singleton to accomodate the system, for now i will add the class at this script
*/
public class WordData {
    public string name;
    public Sprite icon;
    public GameObject model;
    public AudioClip soundVer1;
    public string[] alphabets;
    public AudioClip[] alphabetsSound;
}
