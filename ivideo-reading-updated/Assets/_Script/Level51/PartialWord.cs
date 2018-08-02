using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))] 
public class PartialWord : MonoBehaviour {

    [SerializeField]
    GameObject[] correctLetters;
    //[SerializeField]
    //float yPos = 3.61f;
    [SerializeField]
    Vector3[] finalLetterLocations;

    public Vector3[] FinalLetterLocations { get { return finalLetterLocations; } }
    public GameObject[] CorrectLetters { get { return correctLetters; } }
    public AudioClip Clip { get { return GetComponent<AudioSource>().clip; } }
    
    void Awake()
    {
        Assert.IsNotNull(correctLetters);
        Assert.IsNotNull(finalLetterLocations);

        Assert.AreEqual(correctLetters.Length, finalLetterLocations.Length);
    }
    
}
