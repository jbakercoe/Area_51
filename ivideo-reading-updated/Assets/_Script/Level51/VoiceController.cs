using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(DialoguePlayer))]
public class VoiceController : MonoBehaviour {

    #region Summary

    /*
     * This script determines when the DialoguePlayer plays which audioClips 
     */

    #endregion

    [SerializeField]
    [Tooltip("These Audio Clips can be played in random order")]
    AudioClip[] PositiveFeedback;
    [SerializeField]
    [Tooltip("These Audio Clips can be played in random order")]
    AudioClip[] NegativeFeedback;

    DialoguePlayer dialoguePlayer;

    private int currentChunk = 0;

	// Use this for initialization
	void Start ()
    {
        dialoguePlayer = GetComponent<DialoguePlayer>();
        dialoguePlayer.notifySpeakerOfFinish += OnFinishSpeaking;
        Level51State.NotifyStepChange += OnStepChange;
        //StartCoroutine(PauseBeforeSpeaking(AudioChunks[currentChunk].Chunk, InitialDelay));
    }

    private void OnStepChange(int step)
    {
        //currentChunk++;
        //if(currentChunk < AudioChunks.Length)
        //    StartCoroutine(PauseBeforeSpeaking(AudioChunks[currentChunk].Chunk));
    }

    IEnumerator PauseBeforeSpeaking(CustomClip[] clips, float delayTime = 0.5f)
    {
        yield return new WaitForSeconds(delayTime);
        dialoguePlayer.Play(clips);
    }    

    private void OnFinishSpeaking(string id)
    {
        if(Level51State.CurrentStep == 0)
        {
            Level51State.NextStep();
        }
    }

    public void GivePositiveFeedback()
    {
        dialoguePlayer.Play(PositiveFeedback[Random.Range(0, PositiveFeedback.Length)]);
    }

    public void GiveNegativeFeedback()
    {
        dialoguePlayer.Play(NegativeFeedback[Random.Range(0, NegativeFeedback.Length)]);
    }

    public void Play(AudioClip clip)
    {
        dialoguePlayer.Play(clip);
    }

}
