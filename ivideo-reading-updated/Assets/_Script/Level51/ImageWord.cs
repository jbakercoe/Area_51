using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageWord : MonoBehaviour {

    [SerializeField]
    string word;
    [SerializeField]
    GameObject[] letters;

    public string Word { get { return word; } }
    public GameObject[] Letters { get { return letters; } }

}
