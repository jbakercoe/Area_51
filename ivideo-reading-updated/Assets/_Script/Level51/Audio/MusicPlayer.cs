using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour {

    // This script controlls the settings for the music that plays in a level

    AudioSource source;
    
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = true;
        source.loop = true;
        source.volume = .1f;
        source.pitch = 1f;
    }

}
