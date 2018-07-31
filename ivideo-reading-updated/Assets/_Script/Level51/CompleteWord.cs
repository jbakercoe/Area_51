using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteWord : MonoBehaviour
{

    [SerializeField]
    bool isCorrectSound;

    public bool IsCorrectSound { get { return isCorrectSound; } }

    // delegate function to broadcast a word click
    public delegate void OnCompleteWordClick(bool isCorrect);
    public static event OnCompleteWordClick NotifyWordClickObservers;

    void OnMouseDown()
    {
        NotifyWordClickObservers(isCorrectSound);
        Destroy(gameObject);
    }

}
