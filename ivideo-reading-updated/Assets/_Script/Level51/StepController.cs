using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class StepController : MonoBehaviour {

    [SerializeField]
    protected int STEP = -1;

    void Awake()
    {
        Assert.AreNotEqual(STEP, -1, "You must set a step number for this controller.");
    }

    //public abstract void OnStepChange(int step);

}
