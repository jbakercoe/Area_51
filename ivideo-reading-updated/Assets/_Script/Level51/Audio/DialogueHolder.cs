using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DialogueHolder : MonoBehaviour {

    /// <summary>
    /// TODO Might make more sense to have a DialogueHolder on the object with its step logic, not just all on the teacher.
    /// Then script that controls step can check this script easily to see if the teacher is finished giving the instructions
    /// </summary>

    #region Summary

    /*
     * This script holds the dialogue that needs to be said
     * Also plays the dialogue at the correct step
     * A GameObject can have more than one DialogueHolder for different steps 
     */

    #endregion

    #region Serialized Variables

    // The step of the level where this dialogue needs to be played
    // To be set in inspecter
    [SerializeField]
    int step = -1;

    [SerializeField]
    [Tooltip("The Audio Clips will be played in the order they are listed here.")]
    AudioClip[] AudioClips;

    [SerializeField]
    [Range(1f, 5f)]
    float InitialDelay;

    #endregion

    DialoguePlayer dialoguePlayer;

    void Awake()
    {
        Assert.AreNotEqual(-1, step, "Need to specify step of level needed for dialogue.");
    }

    // Use this for initialization
    void Start () {
        Level51State.NotifyStepChange += OnStepChange;
        dialoguePlayer = GetComponent<DialoguePlayer>();
        if(Level51State.CurrentStep == step)
        {
            SpeakLines();
        }
	}
	
    private void SpeakLines()
    {
        dialoguePlayer.Play(AudioClips);
    }

    void OnStepChange(int currentStep)
    {
        if (currentStep == step)
        {
            SpeakLines();
        }
    }

}
