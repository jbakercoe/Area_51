using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System ;
using Random = UnityEngine.Random ;
/// <summary>
/// States of the game depends on the sound 
/// </summary>
public enum states{BeActive, Listen}


/// <summary>
/// Game controlle for leve 2.
/// </summary>
[Obsolete("This class is obsolate and no longer used in game",true)]
public class GameController : MonoBehaviour 
{


	#region of public variables 
	//Game controller static reference .
	public static GameController gc;

	//Hold play state editable in editor .
	public states playState;

	/// <summary>
	/// The lit.
	/// lit colot = click state .
	/// Unlit color when not clicked state .
	/// </summary>
	public Color32 lit, unlit;

	//List of audio clip that will be played over game .
	public AudioClip choose, yourTurn, awesome, great, super;


	#endregion


	#region private field 
	//Reward audio array .
	private AudioClip[] reward;
	//ehich reward audio will be played from reward array .
	private int rewardIndex;
	//reference to exericse controller .
	private ExerciseController ec;
	//Audio source connected .
	private AudioSource aud;
	//coroutine for the games controller .
	private Coroutine impatience, readyToShow;
	#endregion


	#region unity message system 


	private void Awake()
	{
		//creating single tone reference 

		if (gc == null)
			gc = this;
		else
			Destroy (this);
	}


	// Use this for initialization
	private void Start () 
	{
		//reward index is set to zero .
		rewardIndex = 0;
		//reward audio clip array is created .
		reward = new AudioClip[]{awesome, super, great};
		//Reward audioclip is mixed .
		MixRewardOrder ();
		//set up exercise controller .
		ec = ExerciseController.ec;
		//activate play state .
		playState = states.BeActive;
		//Audio source attached .
		aud = GetComponent<AudioSource> ();
		//play audio of click a leeter .
		PlayClip ();
		//waiting for a click 
		impatience = StartCoroutine (WaitForClick ());
	}
	
	// Update is called once per frame
	private void Update () 
	{
		if (aud.isPlaying)
			playState = states.Listen;
		else
			playState = states.BeActive;
	}



	#endregion

	/// <summary>
	/// Checks for completion.
	/// This method return the complete set of letters are at least clicked one
	/// </summary>
	/// <returns><c>true</c>, if for completion was checked, <c>false</c> otherwise.</returns>
	public bool CheckForCompletion()
	{
		//Number of click should be greater than number of letter to complete it once
		if (GameObject.FindGameObjectsWithTag("LetterImage").GetLength(0) > CountPickedLetters())
			return false;
		
		Debug.Log ("IsComplete");
		return true;
	}

	public int CountPickedLetters()
	{
		int picked = 0;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("LetterImage"))
			if (go.GetComponent<LetterScript> ().HasFinished ())
				picked++;

		return picked;
	}

	/// <summary>
	/// Mixs the reward order.
	/// This method jumble the clip .
	/// </summary>
	 private void MixRewardOrder()
	{
		
		List<AudioClip> temp = new List<AudioClip>();

		for (int i = 0; i < reward.Length; i++) 
		{
			//bring a random clip .
			AudioClip randomClip = reward [Random.Range (0, reward.Length)];

			//if the clip allready exist .
			//make it suffle .
			while (temp.Contains (randomClip)) 
			{
				randomClip = reward [Random.Range (0, reward.Length)];
			}

			//else add it .
			temp.Add (randomClip);
		}

		//set the array 
		reward = temp.ToArray ();
	}


	/// <summary>
	/// Arbitaries the index of array.
	/// </summary>
	/// <returns>The index of array.</returns>
	/// <param name="lengthofArray">Lengthof array.</param>
	private int ArbitaryIndexOfArray(int lengthofArray){
		
		int lowerBound = 0;
		int upperbound = 10000;
		int getaRandomnumber = Random.Range (lowerBound, upperbound);
		int normalized = getaRandomnumber % lengthofArray;
		return normalized;

	}

	/// <summary>
	/// Moves the index of the reward.
	/// Reset to zero when all reward voice is told .
	/// </summary>
	 private void MoveRewardIndex()
	{
		if (rewardIndex == reward.Length-1)
			rewardIndex = 0;
		else
			rewardIndex++;
	}

	/// <summary>
	/// Plaies the success clip.
	/// This method plays reward clip .
	/// </summary>
	 public void PlaySuccessClip()
	{
		Debug.Log ("Playing success clip ");
		StartCoroutine(WaitToPlaySound(reward[rewardIndex]));
		MoveRewardIndex ();
	}


	/// <summary>
	/// Plaies the clip.
	/// Play the Audio clip "Choose a letter and click on it .
	/// </summary>
	public void PlayClip()
	{
		if (!aud.isPlaying) 
		{
			aud.clip = choose;
			aud.Play ();
		}
	}


	/// <summary>
	/// Play the clip.
	/// Play the clip of the perticular letter attached with letter group .
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="playYourTurn">If set to <c>true</c> play your turn.</param>
	[Obsolete("This class is obsolate and no longer used in game",true)]
	public void PlayClip(AudioClip clip, bool playYourTurn)
	{
		Debug.Log ("Audio will be played  " + clip.name + "  Corresponding bool is " + playYourTurn);
		if (!aud.isPlaying) 
		{
			aud.clip = clip;
			aud.Play ();

			//This coroutine will bring the letters to previous condition .
			//if all leters are cllicked you donot need to do anything
			if (!CheckForCompletion ()) {
				readyToShow = StartCoroutine (WaitToUnhideLetters ());
			}


			//if play your turn is true 
			if (playYourTurn)
				StartCoroutine(WaitToPlaySound());
		}
	}
	/// <summary>
	/// Focuses the on.
	/// Find out the particular letter .
	/// And light it up .
	/// </summary>
	/// <param name="specific">Specific.</param>
	public void FocusOn(GameObject specific)
	{
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("LetterImage"))
		{
			//ignore the letter which is clicked .
			//And the letter which has been allready focoused .
			//May be the letter is disappearing .
			//that letter should not be touched .
			if (go != specific && !go.GetComponent<LetterScript>().HasFinished() )
				go.GetComponent<LetterScript> ().Hide (true);
		}


	}
	/// <summary>
	/// Resets the impatience.
	/// This method Resets 
	/// 1.wait for click coroutine .
	/// 2.shaking of letters .
	/// If game is complete then move to next level .
	/// Make a fresh start for wait for click coroutine .
	/// </summary>
	[Obsolete("This class is obsolate and no longer used in game",true)]
	public void ResetImpatience()
	{
		//stop the coroutine that wait for a click .
		StopCoroutine (impatience);
		//stop all animation of letters .
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("LetterImage"))
			go.GetComponent<Animator> ().SetBool ("ShakeEm", false);


		//check all letters are done or not.
		if (CheckForCompletion ()) 
		{
			Debug.Log ("All letters are completed");

			PlaySuccessClip ();
			//next level or next time co routine
			StartCoroutine (WaitToTransitionExercise ());
		}

		//Wait for click  coroutine started 
		impatience = StartCoroutine (WaitForClick ());
	}

	/// <summary>
	/// Waits for click.
	/// </summary>
	/// <returns>The for click.</returns>
	[Obsolete("This class is obsolate and no longer used in game",true)]
	private IEnumerator WaitForClick()
	{
		// when play the audio do not do anything .
		while (aud.isPlaying) 
		{
			yield return null;	
		}

		//wait for 10 seconds .
		yield return new WaitForSecondsRealtime (10);

		//Shake the letters.
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("LetterImage"))
			go.GetComponent<LetterScript> ().AttemptShake ();

		//play the audio clip of click any letter 
		PlayClip ();
	}
	/// <summary>
	/// Waits to play sound.
	/// This method is used to play wait and play "you say it" .
	/// </summary>
	/// <returns>The to play sound.</returns>
	[Obsolete("This class is obsolate and no longer used in game",true)]
	 private IEnumerator WaitToPlaySound()
	{
		while (aud.isPlaying) 
		{
			yield return null;
		}
			
		//set reward clip 
		aud.clip = yourTurn;
		//play it 
		aud.Play ();
		 
	}
	/// <summary>
	/// Waits to play sound.
	/// Co routine used to play reward sound .
	/// </summary>
	/// <returns>The to play sound.</returns>
	/// <param name="clip">Clip.</param>
	IEnumerator WaitToPlaySound(AudioClip clip)
	{
		Debug.Log ("WaitToPlaySound method invoked");

		while (aud.isPlaying) 
		{
			yield return null;
		}
		//Modified
		//PlayClip (clip, false); 

		aud.clip = clip;
		aud.Play ();
		//This is wrong
		//readyToShow = StartCoroutine (WaitToUnhideLetters ());
	}
	/// <summary>
	/// Waits to transition exercise.
	/// Moves the level to next stage .
	/// </summary>
	/// <returns>The to transition exercise.</returns>
	public IEnumerator WaitToTransitionExercise()
	{
		while (aud.isPlaying) 
		{
			yield return null;
		}


		ec.AdvanceGroup ();
	}



	/// <summary>
	/// Waits to unhide letters.
	/// Bring the letters Back to normal .
	/// </summary>
	/// <returns>The to unhide letters.</returns>
	private IEnumerator WaitToUnhideLetters()
	{
		
		Debug.Log ("Restart all the letters ");


		// when audio playing of letter do not do any thing
		while (aud.isPlaying) 
		{
			yield return null;
		}

	
		//Bring to preveious condition

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("LetterImage")) {
			//bring all the letters to its previous condition .
			//except the one you have clicked .
			//We have planned to disssappear it .
			if ( !go.GetComponent<LetterScript> ().HasFinished()) {
				go.GetComponent<LetterScript> ().Hide (false);
			}
		}
	}


	                                                                                                                            
}
