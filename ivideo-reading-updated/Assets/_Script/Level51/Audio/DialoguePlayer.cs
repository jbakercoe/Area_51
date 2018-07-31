using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DialoguePlayer : MonoBehaviour {
    
    #region Summary
    /*
     * Plays supplied AudioClips
     * 
     */
    #endregion

    // To determine if an AudioClip is playing
    bool isPlayingAudio;
    AudioSource audioSource;
    
    public bool IsPlayingAudio { get { return isPlayingAudio; } }


    public delegate void OnFinishSpeaking(string id);
    public event OnFinishSpeaking notifySpeakerOfFinish;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
        audioSource.spatialBlend = .15f;
    }

    IEnumerator WaitForEndOfClip(AudioSource source)
    {
        if (source.isPlaying)
        {
            yield return null;
        }
        isPlayingAudio = false;
    }

    /// <summary>
    /// Plays a single AudioClip
    /// TODO Change both to overloaded Play() functions?
    /// </summary>
    /// <param name="source">AudioClip to be played</param>
    public void Play(AudioClip clip)
    {
        StartCoroutine(PlayAudioClip(clip));
    }

    /// <summary>
    /// Plays multiple AudioClips back to back
    /// </summary>
    /// <param name="sources"></param>
    /// <param name="callerId">Optional Id parameter so recievers of NotifyOnFinish can tell if they were the script to call Play</param>
    public void Play(AudioClip[] clips, string callerId = null)
    {
        StartCoroutine(PlayAudioClips(clips, callerId));
    }

    /// <summary>
    /// Plays multiple AudioClips back to back
    /// </summary>
    /// <param name="sources"></param>
    /// <param name="callerId">Optional Id parameter so recievers of NotifyOnFinish can tell if they were the script to call Play</param>
    public void Play(CustomClip[] clips, string callerId = null)
    {
        StartCoroutine(PlayAudioClips(clips, callerId));
    }

    IEnumerator PlayAudioClips(AudioClip[] clips, string callerId = null)
    {
        foreach (AudioClip clip in clips)
        {
            audioSource.clip = clip;
            isPlayingAudio = true;
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            isPlayingAudio = false;
        }
        notifySpeakerOfFinish(callerId);
    }

    IEnumerator PlayAudioClips(CustomClip[] clips, string callerId = null)
    {
        foreach (CustomClip customClip in clips)
        {
            audioSource.clip = customClip.clip;
            isPlayingAudio = true;
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            if (customClip.hasActionTrigger)
            {
                // Fire action trigger
                notifySpeakerOfFinish(callerId);
            }
            isPlayingAudio = false;
        }
        //notifySpeakerOfFinish(callerId);
    }

    IEnumerator PlayAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
        isPlayingAudio = true;
        audioSource.Play();
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        isPlayingAudio = false;
    }

    /// <summary>
    /// Plays the supplied AudioClip numRepeats amount of times with a delay 
    /// </summary>
    /// <param name="clip">The clip to be played</param>
    /// <param name="numRepeats">The number of times to repeat clip</param>
    /// <param name="pauseTime">The amount of time to pause between speaking</param>
    /// <returns></returns>
    public IEnumerator PlayClipWithRepeat(AudioClip clip, int numRepeats, float pauseTime = 2f)
    {
        for(int i = 0; i < numRepeats; i++)
        {
            Play(clip);
            if(i != numRepeats-1)
                yield return new WaitForSeconds(pauseTime);
        }
    }

}
