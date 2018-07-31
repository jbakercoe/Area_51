using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#region Summary
/*
 * An AudioChunk is simply a way to store arrays of AudioClips.
 * A chunk of AudioClips is a bunch of clips that the speaker needs to say one right after another.
 * 
 * Chunk: The array of AudioClips.
 * 
 * Meant for use with a VoiceController
 */
#endregion

[System.Serializable]
public class AudioChunk {

    public AudioClip[] Chunk;

}

// Experiment
// To have triggers for gameplay associated with AudioClips
[System.Serializable]
public class CustomClip
{
    public AudioClip clip;
    public bool hasActionTrigger;

    // default constructor
    public CustomClip()
    {
        clip = null;
        hasActionTrigger = false;
    }

    public CustomClip(AudioClip c)
    {
        clip = c;
        hasActionTrigger = false;
    }

    public CustomClip(AudioClip c, bool trigger)
    {
        clip = c;
        hasActionTrigger = trigger;
    }

}
