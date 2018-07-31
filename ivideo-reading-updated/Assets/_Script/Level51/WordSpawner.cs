using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSpawner : MonoBehaviour {

    [SerializeField]
    Transform[] spawnLocations;
    [SerializeField]
    GameObject[] words;

    int numCorrect;
    VoiceController voiceController;
    List<GameObject> createdWords = new List<GameObject>();

    const int NUM_CORRECT_WORDS = 5;

    void Start()
    {
        Level51State.NotifyStepChange += OnStepChange;
        CompleteWord.NotifyWordClickObservers += OnWordClick;
        voiceController = GameObject.FindGameObjectWithTag("Teacher").GetComponent<VoiceController>();
        if(Level51State.CurrentStep == 6)
        {
            // Start activity
            StartActivity();
        }
    }

    void OnStepChange(int step)
    {
        if(step == 6)
        {
            // Start activity
            StartActivity();
        }
    }

    void OnWordClick(bool isCorrect)
    {
        print("Click recieved: " + isCorrect);
        if (isCorrect)
        {
            numCorrect++;
            voiceController.GivePositiveFeedback();
        } else
        {
            voiceController.GiveNegativeFeedback();
        }
        if(numCorrect == NUM_CORRECT_WORDS)
        {
            // Got 'em all, time to end the game
            foreach(GameObject word in createdWords)
            {
                if(word != null)
                    Destroy(word);
            }
            Level51State.NextStep();
        }
    }

    void StartActivity()
    {
        numCorrect = 0;
        words = RandomArray.ShuffleArray(words);
        for(int i = 0; i < spawnLocations.Length; i++)
        {
            GameObject newWord = Instantiate(words[i], transform);
            newWord.transform.position = spawnLocations[i].position;
            createdWords.Add(newWord);
        }
    }

}
