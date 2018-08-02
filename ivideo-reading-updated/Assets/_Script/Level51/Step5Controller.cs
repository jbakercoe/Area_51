using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step5Controller : StepController {

    /// <summary>
    /// TODO try to combine similar functions with SpellWordExercise
    /// </summary>

    [SerializeField]
    GameObject letterBankPrefab;
    [SerializeField]
    GameObject[] words;
    [SerializeField]
    DialoguePlayer dialoguePlayer;

    GameObject letterBankObject;
    GameObject currentWord;
    Vector3[] finalLetterLocations;
    int numLettersPlacedInWord;
    int currentWordIndex;
    bool isFinishedBuildingWord;

    // Use this for initialization
    void Start() {
        Level51State.NotifyStepChange += OnStepChange;
        if (Level51State.CurrentStep == STEP)
        {
            StartExercise();
        }
    }

    void OnStepChange(int step)
    {
        if (step == STEP)
        {
            StartExercise();
        }
    }

    void StartExercise()
    {
        Letter.NotifyLetterObservers += OnLetterClick;
        words = RandomArray.ShuffleArray(words);
        letterBankObject = Instantiate(letterBankPrefab, transform);
        currentWordIndex = 0;
        isFinishedBuildingWord = false;
        StartCoroutine(WaitForInstructions());
    }

    // pause before speaking the first word
    IEnumerator WaitForInstructions()
    {
        yield return new WaitForSeconds(2.2f);
        GetNewWord();
    }

    /// <summary>
    /// Creates partial word object and other things you need
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The gameObject of the partial word</returns>
    GameObject InstantiateWordObject(int index)
    {
        GameObject word = words[index];
        StartCoroutine(dialoguePlayer.PlayClipWithRepeat(word.GetComponent<PartialWord>().Clip, 3, 3.5f));
        StartCoroutine(SayWordOverAndOver());
        return Instantiate(word);
    }

    void OnLetterClick(GameObject letterObject)
    {
        GameObject[] correctLetters = currentWord.GetComponent<PartialWord>().CorrectLetters;
        finalLetterLocations = currentWord.GetComponent<PartialWord>().FinalLetterLocations;
        for(int i = 0; i < correctLetters.Length; i++)
        {
            if(letterObject.name == correctLetters[i].name)
            {
                letterBankObject.GetComponent<LetterBank>().MoveLetterToPoint(letterObject, finalLetterLocations[i]);
                numLettersPlacedInWord++;
                if(numLettersPlacedInWord == correctLetters.Length)
                {
                    // We finished spelling the word
                    OnFinishSpellingWord();
                }
            }
        }
    }

    void OnFinishSpellingWord()
    {
        isFinishedBuildingWord = true;
        StartCoroutine(WaitBeforeContinue());
    }

    IEnumerator WaitBeforeContinue()
    {
        yield return new WaitForSeconds(1f);
        dialoguePlayer.Play(currentWord.GetComponent<PartialWord>().Clip);
        currentWordIndex++;
        yield return new WaitForSeconds(2f);
        if(currentWordIndex < words.Length)
        {
            // we have more words to spell
            letterBankObject.GetComponent<LetterBank>().ResetLetters();
            GetNewWord();
        }
        else
        {
            // All finished, can move to next step
            EndGame();
        }
    }

    void GetNewWord()
    {
        currentWord = InstantiateWordObject(currentWordIndex);
        numLettersPlacedInWord = 0;
    }

    IEnumerator SayWordOverAndOver()
    {
        // Wait for the teacher to finish saying the word 3 times
        yield return new WaitForSeconds(7f);
        while (!isFinishedBuildingWord)
        {
            yield return new WaitForSeconds(5f);
            if(!isFinishedBuildingWord)
                dialoguePlayer.Play(currentWord.GetComponent<PartialWord>().Clip);
        }
    }

    void EndGame()
    {
        Destroy(letterBankObject);
        Level51State.NextStep();
    }

}
