using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBank : MonoBehaviour {

    /// <summary>
    /// Moves a letter in the selected word to position to spell the word
    /// </summary>
    /// <param name="letter">The letter to be moved</param>
    /// <param name="location">The location to be moved to</param>
    public void MoveLetterToPoint(GameObject letter, Transform location)
    {
        if (letter != null)
        {
            letter.GetComponent<Letter>().ChooseLetter(location.position);
        }
    }

    /// <summary>
    /// Overloaded function to take Vector3 for position instead of transform
    /// </summary>
    /// <param name="letter"></param>
    /// <param name="location"></param>
    public void MoveLetterToPoint(GameObject letter, Vector3 location)
    {
        if (letter != null)
        {
            letter.GetComponent<Letter>().ChooseLetter(location);
        }
    }

    /// <summary>
    /// Places each letter back in original spot
    /// </summary>
    public void ResetLetters()
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<Letter>().ResetLocation();
        }
    }

}
