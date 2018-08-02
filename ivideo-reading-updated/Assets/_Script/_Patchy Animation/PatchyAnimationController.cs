using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PatchyAnimationController : MonoBehaviour
{
    #region publicField

    public static PatchyAnimationController ReferenceAnimationController;

    public enum TypeOfAnimation :int
    {
        IdleAnimation = 0,
        WalkAnimation = 1,
        NowYouDoItAnimation = 2,
        IwillDoItAnimayion = 3,
        Watchit =4,
        TraceTheLetterAnimation = 5,
        SingleWordUtterAnimation =6, // This Animation Type say the word a , b ,c etc.
		Super=7,
		TheWordStartsWithOne = 8,
		CanYouFindOne = 9,
		TwoSyllableUtter = 10,
		ThreeSyllableUtter = 11,
		OneSyllableUtter = 12, //this or single word utter animation should be alternated when using two or more single syllable animations consecutively
    }

    #endregion

    #region PrivateVariable

    private Animator _animator;

    #endregion

    #region Unity Function

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        ReferenceAnimationController = this;
    }

    #endregion

    #region Functions

    /// <summary>
    ///     Start Animation of patchy
    /// </summary>
    /// <param name="animationType"></param>
    /// <param name="deactivateAfterTime">Pass the length of the sound clip length</param>
    /// <param name="offset"> if offset required to adjsut time</param>
	/// <param name="delay"> if delay required to delay start time</param>
	public void StartAnimation(TypeOfAnimation animationType, float deactivateAfterTime, float offset = 0f, float delay = 0f)
    {
       
        //Idle Animation is deafault Animation .
        //This Animation will start automatically .
        if (animationType != TypeOfAnimation.IdleAnimation)
        {
			// If there is a delay, do delay animation
			if (delay != 0)
				StartCoroutine (DelayAnimation (animationType, deactivateAfterTime, delay, offset));
			else
			{
				_animator.SetBool (animationType.ToString (), true);
				var tracEnumerator = BringToIdleAnimation (animationType,
				deactivateAfterTime + offset);
				StartCoroutine (tracEnumerator);
			}
        }
    }

    /// <summary>
    ///     Stop the corresponding Animation.
    ///     Make controller back to idle.
    /// </summary>
    private IEnumerator BringToIdleAnimation(TypeOfAnimation typeOfAnimation, float timetocomplete)
    {
        //Debug.Log("Ienumerator started with parameters "+typeOfAnimation.ToString()+" "+ timetocomplete);
        //wait to complete the sound.
        yield return new WaitForSeconds(timetocomplete);

        //check the corresponding value for this iteration .
        //Deactivate the bool.
        //Bring it to idle.
         _animator.SetBool (typeOfAnimation.ToString (), false);

    }

	private IEnumerator DelayAnimation(TypeOfAnimation animationType, float deactivateAfterTime, float delayTime, float offset = 0f)
	{
		yield return new WaitForSeconds (delayTime);

		StartAnimation(animationType, deactivateAfterTime, offset);
	}

    #endregion
}