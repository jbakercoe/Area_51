using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class JellyfishSpawner : StepController {

    #region Serialized Variables

    [SerializeField]
    GameObject[] JellyfishPrefabs;
    [SerializeField]
    Transform[] spawnLocations;
    [SerializeField]
    string correctSound;
    [SerializeField]
    string incorrectSound1;
    [SerializeField]
    string incorrectSound2;

    #endregion

    #region Private Variables

    private const int NUMBER_OF_CORRECT_SOUNDS = 5;
    private const int NUMBER_OF_INCORRECT_SOUNDS = 3;

    private int correctClicks = 0;
    private GameObject teacher;
    private VoiceController teacherVoice;

    #endregion

    #region Assertions

    void Awake()
    {
        Assert.IsNotNull(JellyfishPrefabs);
        Assert.IsNotNull(spawnLocations);
        Assert.IsNotNull(correctSound);
        Assert.IsNotNull(incorrectSound1);
        Assert.IsNotNull(incorrectSound2);
    }

    #endregion

    void Start()
    {
        Level51State.NotifyStepChange += OnStepChange;
        teacher = GameObject.FindGameObjectWithTag("Teacher");
        teacherVoice = teacher.GetComponent<VoiceController>();
        if (Level51State.CurrentStep == STEP)
        {
            // Start Jellyfish game
            StartJellyfishGame();
        }
    }

    void OnStepChange(int step)
    {
        if (step == STEP)
        {
            // Start Jellyfish game
            StartJellyfishGame();
        }
    }

    void StartJellyfishGame()
    {
        Jellyfish.NotifyJellyfishObservers += OnJellyfishClick;
        SpawnJellyfish();
    }

    /// <summary>
    /// Spawns RandomLocations.Length-1 number of jellyfish, using random prefabs
    /// one of the locations will not be used for added variety
    /// </summary>
    private void SpawnJellyfish()
    {
        Transform[] RandomLocations = RandomArray.ShuffleArray(spawnLocations);
        // cycles through random order of spawn locations.
        // skips the last one for variety
        for(int i = 0; i < RandomLocations.Length - 1; i++)
        {
            GameObject newJellyfish = Instantiate(ChooseRandomJellyfish(), transform);
            newJellyfish.transform.position = RandomLocations[i].position;
            SetJellyfishTextValue(i, newJellyfish);
        }
    }

    /// <summary>
    /// Assigns a sound to the jellyfish
    /// </summary>
    /// <param name="num">Assumed to increment each call to keep track of sounds to use</param>
    /// <param name="jellyfish">The jellyfish object to change</param>
    private void SetJellyfishTextValue(int num, GameObject jellyfish)
    {
        // Get child with text mesh
        for(int i = 0; i < jellyfish.transform.childCount; i++)
        {
            GameObject childObject = jellyfish.transform.GetChild(i).gameObject;
            if (childObject.CompareTag("Text"))
            {
                // Got the text child
                TextMesh textMesh = childObject.GetComponent<TextMesh>();
                if(num < NUMBER_OF_CORRECT_SOUNDS)
                {
                    // Give it the correct sound
                    textMesh.text = correctSound;
                    jellyfish.GetComponent<Jellyfish>().IsCorrectSound = true;
                } else
                {
                    // Alternate between incorrect sounds
                    if(num % 2 == 0)
                    {
                        textMesh.text = incorrectSound1;
                    } else
                    {
                        textMesh.text = incorrectSound2;
                    }
                    jellyfish.GetComponent<Jellyfish>().IsCorrectSound = false;
                }
            }
        }
    }

    /// <summary>
    /// Picks a Jellyfish Prefab at random to use
    /// </summary>
    /// <returns>The prefab GameObject</returns>
    private GameObject ChooseRandomJellyfish()
    {
        return JellyfishPrefabs[Random.Range(0, JellyfishPrefabs.Length)];
    }

    /// <summary>
    /// Listener for click event
    /// </summary>
    /// <param name="isCorrect">If the jellyfish had the correct sound</param>
    private void OnJellyfishClick(bool isCorrect)
    {
        if (isCorrect)
        {
            teacherVoice.GivePositiveFeedback();
            correctClicks++;
            if(correctClicks == NUMBER_OF_CORRECT_SOUNDS)
            {
                StartCoroutine(PauseBeforeEndGame());
            }
        }
        else
        {
            teacherVoice.GiveNegativeFeedback();
        }
    }

    IEnumerator PauseBeforeEndGame()
    {
        yield return new WaitForSeconds(2f);
        EndGame();
    }

    /// <summary>
    /// Housecleaning before moving to next step
    /// </summary>
    void EndGame()
    {
        // end Jellyfish game
        Jellyfish.NotifyJellyfishObservers -= OnJellyfishClick;
        // Destroys any remaining jellyfish
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.CompareTag("Jellyfish"))
            {
                Destroy(child);
            }
        }
        Level51State.NextStep();
    }
}
