using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete("This class is obsolate and no longer used in game",true)]
[RequireComponent(typeof(Button))]
public class LetterScript : MonoBehaviour 
{

	#region for Fields 
	//sound of the letter .
	public AudioClip mySound;
	//This multiplier select how fast letter will disappear .
	public float vanishingmultiplier = 5f;
	//Image of the letter .
	private Image myImage;
	//Game controller instance .
	private GameController gc;
	//Is this letter has been picked up .
	private bool picked;
	//Animation for shaking .
	private Animator anim;
	//Button in which it is attached
	private Button attachedButton ;

	#endregion



	#region unity message system 

	// Use this for initialization .
	void Start () 
	{
		gc = GameController.gc;
		myImage = GetComponent<Image> ();
		myImage.color = gc.lit;
		picked = false;
		anim = GetComponent<Animator> ();
		attachedButton = GetComponent<Button> ();
	
			
	}
	#endregion


	public bool HasFinished()
	{
		return picked;
	}

	public void Hide(bool hide)
	{
		if (hide)
			myImage.color = gc.unlit;
		else
			myImage.color = gc.lit;
	}
	public void AttemptShake()
	{
		if (picked)
			anim.SetBool ("ShakeEm", false);
		else
			anim.SetBool ("ShakeEm", true);
	}
	[System.Obsolete("This class is obsolate and no longer used in game",true)]
	public void Selected()
	{
		if (gc.playState == states.BeActive) 
		{
			//if letters are shaking then stop them.
			anim.SetBool ("ShakeEm", false);
			//Make picked true .
			picked = true;
			//Focous the particular gameobject .
			gc.FocusOn (gameObject);
			//Disabling the button receiver
			//that you are not able to click on it .
			attachedButton.enabled = false;

			/*<Depricated>
			if (gc.CountPickedLetters () == 1)
				gc.PlayClip (mySound, true);
			else
				gc.PlayClip (mySound, false);
				</Depricated>*/

			//play the sound of letter 
			gc.PlayClip (mySound, true);

			//Reset the values.
			gc.ResetImpatience ();

			//wait for certain time + time taken by the voice "now you say it" .
			//Then make the letter disappear .
			Invoke ("DisapperTheCurrentLetter", (gc.yourTurn.length + 5f));

		}

	}



	/// <summary>
	/// Disappers the current letter.
	/// </summary>
	private void DisapperTheCurrentLetter(){
		
		IEnumerator vanishThread = Vanish (0.5F);
		StartCoroutine (vanishThread);
		//make sure invoke get cancelled 
		CancelInvoke ("DisapperTheCurrentLetter");
	}



	/// <summary>
	/// Vanish the specified withaDelay.
	/// This method helps to dis appear the word by a coroutine .
	/// </summary>
	/// <param name="withaDelay">Witha delay.</param>
	private IEnumerator Vanish( float withaDelay){

		//current color .
		Color currentcolor = myImage.color;

		//alpha of the image
		float alpha = currentcolor.a;

		//Iterate over the loop 
		while (alpha > 0f) {


			//reduce alpha over time 
			alpha -= Time.deltaTime * vanishingmultiplier ;

			//current color 
			Color c = myImage.color;

			//Set the color 
			c.a = alpha ;

			//Set the color 
			myImage.color = c;


			//Ieterate over a delay 
			yield return new WaitForSeconds (withaDelay) ;
		}

		//Deactivate the gameobject when it is done .
			this.gameObject.SetActive (false);

		//Refun it's own alpha value 

		//set a new color
		Color newColor =  gc.lit ;

		//Set color 
		myImage.color = newColor ;


		//reset a bool 
		picked = picked ? false : true ;


		//Let the button accept responce
		attachedButton.enabled = true ;

		}
	}

