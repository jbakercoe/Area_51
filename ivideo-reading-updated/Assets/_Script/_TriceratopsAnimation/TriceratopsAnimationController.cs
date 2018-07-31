using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triceratops Animation controller. Similar to Reezo's patchy animation controller
/// </summary>
[RequireComponent(typeof(Animator))]
public class TriceratopsAnimationController : MonoBehaviour 
{
	#region publicField

	public static TriceratopsAnimationController ReferenceAnimationController;

	// These enum names should match the intended animation parameter name
	public enum TypeOfAnimation :int
	{
		Idle = 0,
		TraceTheLetters = 1,
		NowYouDoIt = 2,
		CanYouFindTheFirst =3,
		OneSyllableWord =4,
		TwoSyllableWord = 5,
		TheWordOneSyllableStartsWith = 6,
		Awesome = 7,
		WeAreGoingToPlay = 8,
		LetsMake = 9

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
		if (animationType != TypeOfAnimation.Idle)
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