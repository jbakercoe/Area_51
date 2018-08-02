using UnityEngine;

namespace Level6
{
    public class AlphabetModule : MonoBehaviour
    {
        [Header("Name of the alphabet")] public string NameOfAlphabet;
        [Header("Corresponding object")] public GameObject AlphabetObject;
        [Header("Alphabet clip")] public AudioClip AlphabetSound;
    }
}