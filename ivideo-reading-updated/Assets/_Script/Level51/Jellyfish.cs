using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jellyfish : MonoBehaviour, IPointerClickHandler {

    // delegate function to broadcast a jellyfish click
    public delegate void OnJellyfishClick(bool isCorrect);
    public static event OnJellyfishClick NotifyJellyfishObservers;

    [HideInInspector]
    public bool IsCorrectSound;
        
    public void OnPointerClick(PointerEventData eventData)
    {
        // broadcast a click to any observers
        NotifyJellyfishObservers(IsCorrectSound);
        Destroy(gameObject);
    }

}
