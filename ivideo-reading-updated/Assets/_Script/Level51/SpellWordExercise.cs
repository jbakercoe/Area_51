using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class SpellWordExercise : MonoBehaviour {

    #region Summary
    /*
     * This Class controlls step2 of Level51 where the teacher demos how to spell a word 
     * Also step3, the game where the player builds a word from the image
     * 
     * If this class gets too cluttered, this script can probably be broken up into:
     *      - script that runs teacher demo
     *      - script that runs player game
     * 
     * The 2 steps are combined because they have very similar functionality
     * 
     */
    #endregion

    #region Serialized Variables
    // reference to the teacher (To grab reference to dialogue player)
    [SerializeField]
    GameObject teacher;
    // The image to be selected and spelled by the teacher (step 2)
    [SerializeField]
    GameObject demoImage;
    // The locations the letters will be at when they spell the word (step 2 and 3)
    // Valid only for 3 letter words (counting blend as one letter)
    // Maybe store in the letter/partial word?
    [SerializeField]
    Transform[] finalLetterLocations;
    // The letters that will be used to spell the demo word (step 2)
    [SerializeField]
    GameObject[] demoWordLetters;
    // The audio clip of the demo word (step 2)
    [SerializeField]
    AudioClip demoWordClip;
    // The prefab to be instantiated into the letterBankObject (step 2 and 3)
    [SerializeField]
    GameObject letterBankPrefab;
    // Array of images that can be selected by the player to spell out (step 3)
    [SerializeField]
    Image[] wordImages;

    // A time delay before demo begins (step 2)
    [SerializeField] [Range(1f, 5f)]
    float DelayBeforeDemo = 3f;

    #endregion

    #region Private Variables
    
    // To play dialogue (step 2 and 3)
    DialoguePlayer dialoguePlayer;
    // The letters needed to spell the target word
    // For use when player needs to click letters
    GameObject[] correctLetters = null;
    // how many words we have made
    int wordsSpelled;
    // Numbers placed in the word so far (step 3)
    int numLettersPlacedInWord;
    // The current Image we're working with (step 3)
    GameObject currentImageObject;
    GameObject letterBankObject = null;

    #endregion

    #region Assertions

    /// <summary>
    /// Makes sure certain requirements are met for basic level functionality
    /// </summary>
    void Awake()
    {
        Assert.IsNotNull(teacher);
        Assert.IsNotNull(demoImage);
        //Assert.IsNotNull(finalLetterLocations);
        Assert.IsNotNull(demoWordLetters);
        Assert.IsNotNull(demoWordClip);
        Assert.IsNotNull(letterBankPrefab);
        Assert.IsNotNull(wordImages);

        // Make sure there are equal letters in word with target spaces
        //Assert.AreEqual(finalLetterLocations.Length, demoWordLetters.Length);
    }

    #endregion

    void Start()
    {
        // To listen for step change
        Level51State.NotifyStepChange += OnStepChange;
        dialoguePlayer = teacher.GetComponent<DialoguePlayer>();
        if (Level51State.CurrentStep == 2)
        {
            PlayTeacherDemo();
        } else if (Level51State.CurrentStep == 3)
        {
            StartPlayerExercise();
        }
    }

    /// <summary>
    /// Listener for when the level step changes
    /// If level changes to this step level, the minigame begins
    /// </summary>
    /// <param name="step">The level's current step</param>
    private void OnStepChange(int step)
    {
        if(step == 2)
        {
            PlayTeacherDemo();
        } else if(step == 3)
        {
            StartPlayerExercise();
        }
    }

    #region Teacher Demo

    // These functions are used by the teacher demo (step 2)

    /// <summary>
    /// Starts the teacher's demo (Spell letter based on word image)
    /// </summary>
    private void PlayTeacherDemo()
    {
        // Makes pictures visable
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        DisableButtonComponents();
        // create letterbank
        letterBankObject = Instantiate(letterBankPrefab);
        StartCoroutine(SpellDemoWord());
    }

    /// <summary>
    /// Animates the example image to large state
    /// </summary>
    private void TeacherSelectImage()
    {
        Animator anim = demoImage.GetComponent<Animator>();
        anim.SetBool("isSelected", true);
    }
    
    /// <summary>
    /// Chooses the proper letters to spell the demo word
    /// </summary>
    /// <returns></returns>
    IEnumerator SpellDemoWord()
    {
        yield return new WaitForSeconds(DelayBeforeDemo);
        TeacherSelectImage();
        yield return new WaitForSeconds(3f);
        // find correct letters in letterBank
        for (int i = 0; i < demoWordLetters.Length; i++)            
        {
            foreach (Transform childTransform in letterBankObject.transform)
            {
                if(childTransform.gameObject.name == demoWordLetters[i].name)
                {
                    letterBankObject.GetComponent<LetterBank>().MoveLetterToPoint(childTransform.gameObject, finalLetterLocations[i]);
                    yield return new WaitForSeconds(2f);
                }
            }
        }
        dialoguePlayer.Play(demoWordClip);
        yield return new WaitForSeconds(1.8f);
        EndTeacherDemo();
    }

    /// <summary>
    /// Housecleaning to wrap up the teacher's demo
    /// </summary>
    private void EndTeacherDemo()
    {
        Animator anim = demoImage.GetComponent<Animator>();
        anim.SetBool("isSelected", false);
        // Clear Letter Bank
        ResetLetters();
        // may need to have this outside if this demo is played at other times
        Level51State.NextStep();
    }

    #endregion

    #region Player Game

    // These functions are used by the player game (step 3)

    /// <summary>
    /// Starts the minigame where player gets to choose picture (as opposed to teacher demo)
    /// </summary>
    void StartPlayerExercise()
    {
        Letter.NotifyLetterObservers += OnLetterClick;
        if (letterBankObject == null)
            letterBankObject = Instantiate(letterBankPrefab);
        foreach (Image i in wordImages)
        {
            i.gameObject.SetActive(true);
        }
        EnableButtonComponents();
        wordsSpelled = 0;
    }

    /// <summary>
    /// What happens when the player clicks on an image
    /// </summary>
    /// <param name="imageObject"></param>
    public void OnImageClick(GameObject imageObject)
    {
        currentImageObject = imageObject;
        currentImageObject.GetComponent<Animator>().SetBool("isSelected", true);
        AudioClip wordClip = imageObject.GetComponent<AudioSource>().clip;
        StartCoroutine(dialoguePlayer.PlayClipWithRepeat(wordClip, 2, 1.5f));
        correctLetters = imageObject.GetComponent<ImageWord>().Letters;
        DisableButtonComponents();
        numLettersPlacedInWord = 0;
    }
    
    /// <summary>
    /// Checks to see if letter clicked is part of current word
    /// If so, moves to proper location
    /// </summary>
    /// <param name="letter">The letter that was clicked</param>
    public void OnLetterClick(GameObject letter)
    {
        if (correctLetters != null)
        {
            for (int i = 0; i < correctLetters.Length; i++)
            {
                if (letter.name == correctLetters[i].name)
                {
                    letterBankObject.GetComponent<LetterBank>().MoveLetterToPoint(letter, finalLetterLocations[i]);
                    numLettersPlacedInWord++;
                    if(numLettersPlacedInWord == correctLetters.Length)
                    {
                        // we're done spelling the word
                        OnFinishSpellingWord();
                    }
                    return;
                }
            }
        }
    }

    // (Almost) Same in step 5
    private void OnFinishSpellingWord()
    {
        wordsSpelled++;
        currentImageObject.GetComponent<Animator>().SetBool("isSelected", false);
        dialoguePlayer.Play(currentImageObject.GetComponent<AudioSource>().clip);
        StartCoroutine(WaitForWordToBeDoneBeforeProceeding());
    }

    /// <summary>
    /// Pauses to let the student see the finished word, and hear it again before game proceeds
    /// TODO rename
    /// Mostly same as step 5
    /// </summary>
    IEnumerator WaitForWordToBeDoneBeforeProceeding()
    {
        // Pause to let student see the finished word
        yield return new WaitForSeconds(3f);
        // Check if the are more words to spell
        if (wordsSpelled < wordImages.Length)
        {
            // we have more words to spell
            EnableButtonComponents();
            ResetLetters();
            currentImageObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            // done spelling words, can move to next step
            MoveToNextStep();
        }
    }

    /// <summary>
    /// Housecleaning before moving to next step
    /// </summary>
    private void MoveToNextStep()
    {
        Letter.NotifyLetterObservers -= OnLetterClick; 
        ResetLetters();
        Destroy(letterBankObject);
        foreach (Image image in wordImages)
        {
            image.gameObject.SetActive(false);
        }
        Level51State.NextStep();
    }

    #endregion

    #region Common Functions
    
     // These functions are shared by both the teacher demo and the player game (steps 2 and 3)
    

    /// <summary>
    /// Reset all letters' positions
    /// </summary>
    private void ResetLetters()
    {
        letterBankObject.GetComponent<LetterBank>().ResetLetters();
    }
    
    /// <summary>
    /// Enables the button components of the images
    /// </summary>
    void EnableButtonComponents()
    {
        foreach (Image i in wordImages)
        {
            Button button = i.GetComponent<Button>();
            button.enabled = true;
        }
    }

    /// <summary>
    /// Disables the button components of the images 
    /// Skips if button is in disabled mode
    /// </summary>
    void DisableButtonComponents()
    {
        foreach (Image i in wordImages)
        {
            Button button = i.GetComponent<Button>();
            if(button.interactable)
                button.enabled = false;
        }
    }

    #endregion

}
