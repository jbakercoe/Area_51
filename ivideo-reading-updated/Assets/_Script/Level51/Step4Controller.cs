using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Step4Controller : StepController {

    #region Summary

    /// <summary>
    /// Controls the logic for step 4
    /// Where the player is presented with words missing a blended sound
    /// Player clicks the sound and it moves into position to spell the word
    /// </summary>

    #endregion

    #region Serialized Variables

    [SerializeField]
    GameObject[] partialWords;
    [SerializeField]
    GameObject soundPrefab;
    [SerializeField]
    DialoguePlayer dialoguePlayer;
    //[SerializeField]
    //AudioClip instructionClip;
    [SerializeField] [Range(0f, 2f)]
    const float PAUSE_TIME = 1f;

    #endregion

    #region Private Variables

    int wordsCompleted;
    GameObject currentWord;
    Vector3 positionOffset = new Vector3(-.69f, 0f, 0f);
    GameObject soundObject;
    AudioClip wordClip;

    Vector3 partialWordPosition = new Vector3(0f, 2.46f, 0f);
    Vector3 soundBlendPosition = new Vector3(0f, -2f, 0f);

    #endregion

    #region Assertions

    /// <summary>
    /// Makes sure certain requirements are met for basic level functionality
    /// </summary>
    void Awake()
    {
        Assert.IsNotNull(partialWords);
        Assert.IsNotNull(soundPrefab);
        Assert.IsNotNull(dialoguePlayer);
        //Assert.IsNotNull(instructionClip);
    }

    #endregion

    void Start()
    {
        Level51State.NotifyStepChange += OnStepChange;
        if(Level51State.CurrentStep == STEP)
        {
            // This is our step baby
            InitializeGame();
        }
    }

    void OnStepChange(int step)
    {
        if (step == STEP)
        {
            // This is our step baby
            InitializeGame();
        }
    }

    private void InitializeGame()
    {
        Letter.NotifyLetterObservers += OnLetterClick;
        wordsCompleted = 0;
        // shuffle partialWords to random order
        partialWords = RandomArray.ShuffleArray(partialWords);
        soundObject = Instantiate(soundPrefab, transform);
        soundObject.transform.position = soundBlendPosition;
        currentWord = InstantiatePartialWord(wordsCompleted);
    }
    
    /// <summary>
    /// Returns the GameObject of the partial word
    /// Also sets the appropriate member variables
    /// </summary>
    /// <param name="index">which partial word to use</param>
    /// <returns>The GameObject of the partial word</returns>
    GameObject InstantiatePartialWord(int index)
    {
        GameObject word = partialWords[index];
        wordClip = word.GetComponent<AudioSource>().clip;
        //AudioClip[] clips = new AudioClip[] { instructionClip, wordClip };
        dialoguePlayer.Play(wordClip);
        GameObject newWord = Instantiate(word, this.transform);
        newWord.transform.position = partialWordPosition;
        return newWord;
    }

    /// <summary>
    /// Called when recieve letter click event
    /// </summary>
    /// <param name="letter">The letter that was clicked</param>
    void OnLetterClick(GameObject letter)
    {
        letter.GetComponent<Letter>().ChooseLetter(currentWord.transform.position + positionOffset);
        StartCoroutine(WaitAndThenSpeak(wordClip));
    }

    /// <summary>
    /// Pauses for a bit before saying the word
    /// pauses before continuing
    /// </summary>
    IEnumerator WaitAndThenSpeak(AudioClip clip, float pause = PAUSE_TIME)
    {
        yield return new WaitForSeconds(pause);
        dialoguePlayer.Play(clip);
        yield return new WaitForSeconds(2f);
        AfterWordIsSpoke();
    }

    /// <summary>
    /// Determines if we have more words to do or if minigame is finished
    /// </summary>
    void AfterWordIsSpoke()
    {
        wordsCompleted++;
        if(wordsCompleted < partialWords.Length)
        {
            // got more words to do
            soundObject.GetComponent<Letter>().ResetLocation();
            Destroy(currentWord);
            currentWord = InstantiatePartialWord(wordsCompleted);
        }
        else
        {
            // finish minigame
            EndMiniGame();
        }
    }

    /// <summary>
    /// Housecleaning before changing to next step
    /// </summary>
    private void EndMiniGame()
    {
        Letter.NotifyLetterObservers -= OnLetterClick;
        Destroy(soundObject);
        Destroy(currentWord);
        Level51State.NextStep();
    }
}
