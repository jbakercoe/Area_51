using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Level8 controller.
/// </summary>
namespace Level8
{
	/// <summary>
	/// class responsible for controlling level 4
	/// </summary>
	public class Level8Controller : MonoBehaviour 
	{
		#region public variables
		/// <summary>
		/// The path for object sprites.
		/// </summary>
		public readonly string pathForObjectSprites = "Level8/Pictures/Set";

		/// <summary>
		/// The path for object name sounds.
		/// </summary>
		public readonly string pathForObjectNameSounds = "Level8/Sounds/Set";

		/// <summary>
		/// The path for vowel Sounds.
		/// </summary>
		public readonly string pathForLetterSounds = "Level8/Sounds/VowelSounds/";

		/// <summary>
		/// accesses the setNumber.
		/// </summary>
		public int set
		{
			get{ return (setNumber); }
			private set{setNumber = value;}
		}

		/// <summary>
		/// accesses the index of focused word in ObjectsOfWords array.
		/// </summary>
		/// <value>The index of focused.</value>
		public int indexOfFocused
		{
			get{ return(Mathf.Clamp(currentIndex, 0, 3)); }
			private set{ currentIndex = value; }
		}

		/// <summary>
		/// which part are we on? sets should be split in part 1 and part 2
		/// </summary>
		/// <value>The part.</value>
		public int part
		{
			get{ return( Mathf.Clamp(half, 1, 2)); }
			private set{ half = value; }
		}

		/// <summary>
		/// gets a value indicating whether this instance is waiting for audio.
		/// controls flow of coroutines
		/// </summary>
		/// <value><c>true</c> if waiting for audio; otherwise, <c>false</c>.</value>
		public bool waitingForAudio{get{ return(UpdatedAudioSource.Reference.IsAudioPlaying); }}

		/// <summary>
		/// accesses a value indicating whether this instance is waiting for object transition.
		/// controls flow of coroutines
		/// </summary>
		/// <value><c>true</c> if waiting for object transition; otherwise, <c>false</c>.</value>
		public bool waitingForTransition
		{
			get{ return(waitOnTransition); }
			private set{ waitOnTransition = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance can accept clicks.
		/// </summary>
		/// <value><c>true</c> if can we click; otherwise, <c>false</c>.</value>
		public bool canWeClick
		{
			get{ return(canClick); }
			private set{canClick = value;}
		}
			
		/// <summary>
		/// The objects representing words. Sprite component of these objects will be modified accordingly
		/// </summary>
		public GameObject[] objectsOfWords;

		/// <summary>
		/// The letters of the word. Example: {b, a, t}
		/// </summary>
		public GameObject[] lettersOfWords;

		/// <summary>
		/// holds the sprites for current set.
		/// </summary>
		/// <value>The sprites for set.</value>
		public Sprite[] spritesForSet
		{
			get {return(spriteArray);}
			private set{spriteArray = value;}
		}

		/// <summary>
		/// stores the superlatives from inspector into this array.
		/// </summary>
		public AudioClip[] superlatives;

		/// <summary>
		/// stores the audio clip from inspector
		/// </summary>
		public AudioClip weAreGoingToPlayAGame;

		/// <summary>
		/// stores the audioclip from inspector
		/// </summary>
		public AudioClip letsMake;

		/// <summary>
		/// stores the audioclip from inspector
		/// </summary>
		public AudioClip sayTheWordWithMe;

		/// <summary>
		/// stores the audioclip from inspector
		/// </summary>
		public AudioClip canYouFind;

		/// <summary>
		/// stores the audioclip from inspector
		/// </summary>
		public AudioClip clickOn;

		/// <summary>
		/// stores the audioclip from inspector
		/// </summary>
		public AudioClip winSound;

		/// <summary>
		/// stores the audioclip from inspector
		/// </summary>
		public AudioClip whatsThis;

		#endregion

		#region Private Variables
		/// <summary>
		/// The set number.
		/// </summary>
		private int setNumber;

		/// <summary>
		/// The index of the current focused word.
		/// </summary>
		private int currentIndex;

		/// <summary>
		/// represents the half of the set we are on
		/// </summary>
		private int half;

		/// <summary>
		/// The number of times instructions were repeated.
		/// </summary>
		private int numberOfRepeatInstructions = 0;

		/// <summary>
		/// are we waiting on an object transition?
		/// </summary>
		private bool waitOnTransition = false;

		/// <summary>
		/// The waiting on step one. Controls flow of Init
		/// </summary>
		private bool waitingOnStepOne = false;

		/// <summary>
		/// The waiting on step two. Controls flow of Init
		/// </summary>
		private bool waitingOnStepTwo = false;

		/// <summary>
		/// The waiting on step three. Controls flow of Init
		/// </summary>
		private bool waitingOnStepThree = false;

		/// <summary>
		/// represents boolean that decides if we will take click input from user
		/// </summary>
		private bool canClick = false;

		/// <summary>
		/// The sprite array for this current set.
		/// </summary>
		private Sprite[] spriteArray = null;

		/// <summary>
		/// coroutine that check the length of inactivity
		/// </summary>
		private Coroutine checkingInactivity = null;

		#endregion

		// Use this for initialization
		void Start () 
		{
			
			//the first set is set1
			set = 1;

			//begin on first half of set
			part = 1;
		
			indexOfFocused = 0;

			//button should not be interactable at beginnning of game --- should be interactable while waitingForInactivity
			lettersOfWords [1].GetComponent<Button> ().interactable = false;

			//play intro audio
			SoundController.Reference.Playclip (weAreGoingToPlayAGame);
			TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.WeAreGoingToPlay, weAreGoingToPlayAGame.length);

			//set the spritesForSet array
			spritesForSet = LoadSpritesFromResources(pathForObjectSprites + set);

			if(part == 1)
				for (int i = 0; i < objectsOfWords.Length; i++)
				{
					//set the appropriate sprite, and change its name
					objectsOfWords[i].SetActive(true);
					objectsOfWords [i].GetComponent<Image> ().sprite = spritesForSet [i];
					objectsOfWords [i].name = spritesForSet [i].name;
					//move the object to screen
					StartCoroutine (MoveObjectToScreen (objectsOfWords [i]));
				}
			else if(part ==2) // part 2 takes the second half of the sprites of the set
				for (int i = 0; i < objectsOfWords.Length; i++)
				{
					//set the appropriate sprite, and change its name
					objectsOfWords[i].SetActive(true);
					objectsOfWords [i].GetComponent<Image> ().sprite = spritesForSet [i+objectsOfWords.Length]; 
					objectsOfWords [i].name = spritesForSet [i+objectsOfWords.Length].name;
					//move the object to screen
					StartCoroutine (MoveObjectToScreen (objectsOfWords [i]));
				}

			//initialize the game
			Init ();
		}
			

		/// <summary>
		/// Init this instance.
		/// </summary>
		void Init()
		{
			//***** NOTE: method makes use of multiple coroutines within loops. Order of Coroutines are tracked by "waitingFor" or "WaitingOn" booleans****

			//Waiting on all steps
			waitingOnStepOne = true;
			waitingOnStepTwo = true;
			waitingOnStepThree = true;


			//Empty each letter on inits
			SetLettersEmpty();

			//Prepare and start step 1
			StartCoroutine (Step1 ());

			//Prepare Step 2
			StartCoroutine(Step2());


			//Prepare Step 3
			StartCoroutine(Step3());


			//WaitUntilAllCoroutinesStop then increase index
			StartCoroutine (DecideNextPhase ());


		}

		/// <summary>
		/// Raises the vowel clicked event. (Step 3)
		/// </summary>
		public void OnVowelClicked()
		{
			if (canWeClick)
			{
				//stop coroutine we are no longer waiting
				StopCoroutine (checkingInactivity);

				//not waiting for object transition
				waitingForTransition = false;

				AudioClip randomSuperlative = superlatives [Random.Range (0, superlatives.Length)];
				//play a superlative
				SoundController.Reference.Playclip (randomSuperlative);
				TriceratopsAnimationController.ReferenceAnimationController.StartAnimation(TriceratopsAnimationController.TypeOfAnimation.LetsMake, randomSuperlative.length);


				//after clicking, we can not click anymore
				canWeClick = false;

				//button should not be interactable anymore
				lettersOfWords [1].GetComponent<Button> ().interactable = false;

				Behaviour halo = (Behaviour)lettersOfWords [1].transform.GetChild(0).GetComponent ("Halo");
				halo.enabled = false;

				//Step 3 is over
				waitingOnStepThree = false;
			}
				
		}

		/// <summary>
		/// Make the word appear empty
		/// </summary>
		private void SetLettersEmpty()
		{
			for (int i = 0; i < lettersOfWords.Length; i++)
			{
				lettersOfWords [i].GetComponent<Text> ().text = "";
			}
		}

		/// <summary>
		/// Plays list of animations.
		/// </summary>
		/// <param name="animations">Animation&length objects.</param>
		private void PlayAnimations(params TriceratopsAnimationClass[] animations)
		{

			float delaySum = 0;
			for (int i = 0; i < animations.Length; i++)
			{
				TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (animations [i].animation, animations [i].lengthOfAnimation, 0, delaySum);
				delaySum += animations [i].lengthOfAnimation;
			}

		}

		/// <summary>
		/// Step1 of this instance. Moves objects to screen, gives directions, sounds out word
		/// </summary>
		private IEnumerator Step1()
		{

			// after all objects are moved to the screen
			while (waitingForAudio || waitingForTransition)
			{
				yield return null;
			}
			//All objects but the current should dissapear
			StartCoroutine (PrepImageVanish ());

			//Lets make (word)-- sounds out (word) -- say the word with me -- (word)
			SoundController.Reference.Playclip (letsMake);
			TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.LetsMake, letsMake.length);

			yield return new WaitForSeconds (letsMake.length);
			//walk through (word)
			StartCoroutine (PrepWordWalkThrough(objectsOfWords [indexOfFocused].name));

			//will wait till after walkthrough
			//After all audio or transitions are done, step 1 is done
			while (waitingForTransition || waitingForAudio)
			{
				yield return null;
			}
				
			//one second between step 1 and 2
			yield return new WaitForSeconds (1f);
			waitingOnStepOne = false;
		}

		/// <summary>
		/// Step2 of this instance. Points to letters, "what is this?
		/// </summary>
		private IEnumerator Step2()
		{
			while (waitingOnStepOne)
			{
				yield return null;
			}

			//Points to each letter, What is this? after 5 seconds says sound
			StartCoroutine(PrepLetterJumping());

			//After all audio or transitions are done, step 2 is done
			while (waitingForAudio || waitingForTransition)
			{
				yield return null;
			}

			waitingOnStepTwo = false;
		}

		/// <summary>
		/// Step3 of this instance. Can you find Vowel? Click on it
		/// </summary>
		private IEnumerator Step3()
		{
			while (waitingOnStepTwo)
			{
				yield return null;
			}


			//index 1  of the current word will always be the location of the vowel
			AudioClip vowel = LoadAudioFromResources (objectsOfWords [indexOfFocused].name.Substring (1, 1), pathForLetterSounds);
			SoundController.Reference.Playclip (canYouFind, vowel, clickOn, vowel);
			TriceratopsAnimationClass canyoufind = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.CanYouFindTheFirst, canYouFind.length);
			TriceratopsAnimationClass vowelSoundAnim = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, vowel.length-.5f);
			TriceratopsAnimationClass click = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.Awesome, clickOn.length);

			PlayAnimations (canyoufind, vowelSoundAnim, click, vowelSoundAnim);

			//Start waiting for inactivity
			checkingInactivity = StartCoroutine (WaitForInactivity(10));

			//Will be stopped by OnVowelClicked
		}

		/// <summary>
		/// Moves the object to a suitable place on the screen. (Step 1)
		/// </summary>
		/// <returns>The object to screen.</returns>
		/// <param name="objectToMove">Object to move.</param>
		private IEnumerator MoveObjectToScreen(GameObject objectToMove)
		{
			//if we ware waiting for audio or object transition, do nothing
			while (waitingForAudio || waitingForTransition)
			{
				yield return null;
			}
			//this is an object transition
			//other coroutines will wait for this transition to finish
			waitingForTransition = true;
			//Stores the origin point of objectToMove
			Vector3 origin = objectToMove.GetComponent<RectTransform>().anchoredPosition;
			//Target position will be a certain distance on the x axis away from origin
			Vector3 targetPosition = new Vector3 (origin.x - 900, origin.y, origin.z);
			//make sure object is active
			objectToMove.SetActive (true);

			//while still further than the target position, move towards it
			while (objectToMove.GetComponent<RectTransform>().anchoredPosition.x > targetPosition.x)
			{
				objectToMove.GetComponent<RectTransform>().anchoredPosition -= new Vector2 (3, 0);
				yield return null;
			}

			AudioClip loadedAudio = LoadAudioFromResources (objectToMove.name, pathForObjectNameSounds + set + "/Straight/");
			//When destination reached, say name of object
			SoundController.Reference.Playclip (loadedAudio);

			TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, loadedAudio.length-.3f);
			//this transition is done
			waitingForTransition = false;
		}

		/// <summary>
		/// Preps the word walk through: "Lets make (word)" word appears on screen. (Step 2)
		/// </summary>
		/// <param name="word">WordHolder.</param>
		private IEnumerator PrepWordWalkThrough(string word)
		{
			while(waitingForAudio || waitingForTransition)
			{
				yield return null;
			}

			waitingForTransition = true;


			AudioClip loadedAudio = LoadAudioFromResources (word, pathForObjectNameSounds + set + "/Straight/");
			//Play name of word
			SoundController.Reference.Playclip(loadedAudio);
			TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, loadedAudio.length);
	
			yield return new WaitForSeconds (loadedAudio.length);

			//Spawning Letters in
			for (int i = 0; i < lettersOfWords.Length; i++)
			{
				AudioClip letterSound = LoadAudioFromResources(word.Substring(i,1), pathForLetterSounds);
				SoundController.Reference.Playclip(letterSound);
				TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, letterSound.length-.5f);
				lettersOfWords[i].GetComponent<Text>().text = word.Substring (i, 1);
				lettersOfWords [i].name = word.Substring (i, 1);

				//wait 1 second inbetween letters
				yield return new WaitForSecondsRealtime (1.5f);

			}

			//Disable the horizontal group so we can move x and y of object
			lettersOfWords [0].transform.parent.GetComponent<HorizontalLayoutGroup> ().enabled = false;

			for (int iteration = 0; iteration < 4; iteration++)
			{
				//After you spawn letters in, repeat the letters sounding them out, raising the letters you are focused on
				for (int i = 0; i < lettersOfWords.Length; i++)
				{
					AudioClip letterSound = LoadAudioFromResources(word.Substring(i,1), pathForLetterSounds);
					SoundController.Reference.StopClip ();
					//Play letter sound
					SoundController.Reference.Playclip (letterSound);
					TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, letterSound.length-.5f);
					RectTransform letterTransform = lettersOfWords [i].GetComponent<RectTransform> ();
					//raise letter
					letterTransform.anchoredPosition = new Vector2 (letterTransform.anchoredPosition.x, letterTransform.anchoredPosition.y + 50);
			

					//wait 1 second inbetween letters
					yield return new WaitForSeconds (1.5f - (.4f * iteration));

					letterTransform.anchoredPosition = new Vector2 (letterTransform.anchoredPosition.x, letterTransform.anchoredPosition.y - 50);

				}
			}



			SoundController.Reference.Playclip (loadedAudio, sayTheWordWithMe, loadedAudio);
			TriceratopsAnimationClass lSound = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, loadedAudio.length);
			TriceratopsAnimationClass sayTheWord = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.CanYouFindTheFirst, sayTheWordWithMe.length);

			PlayAnimations (lSound, sayTheWord, lSound);

			while (waitingForAudio)
			{
				yield return null;
			}

			//This transition is over
			waitingForTransition = false;
		
		}

		/// <summary>
		/// Preps the next phase.
		/// </summary>
		/// <returns>The new index.</returns>
		private IEnumerator DecideNextPhase()
		{
			//do nothing while waiting for steps to finish
			while(waitingOnStepOne|| waitingOnStepTwo|| waitingOnStepThree)
			{
				yield return null;
			}

			//if ready, 

			//increase focused index
			indexOfFocused++;
			//if index is at objectOfWords.length we are ready for the next half of this set or a new set
			if (indexOfFocused == objectsOfWords.Length)
			{
				//Reset objectsOfWords positions
				for(int i = 0; i < objectsOfWords.Length; i++)
				{
					objectsOfWords[i].GetComponent<RectTransform>().anchoredPosition 
					= new Vector2 (objectsOfWords[i].GetComponent<RectTransform>().anchoredPosition.x+900, 
						objectsOfWords[i].GetComponent<RectTransform>().anchoredPosition.y);
				}

				indexOfFocused = 0;

				//If we are on part 1
				if (part == 1)
				{
					//we should now be on second part
					part = 2;

				} 

				else if (part == 2)
				//if we are already on second half, start a new set
				{
					//start on first part of new set
					part = 1;
					//increase set
					set++;

					//Play triumphant sound before next phase
					//PrepNextClip (winSound, true);

					//set the spritesForSet array for new set
					spritesForSet = LoadSpritesFromResources(pathForObjectSprites + set);

				}

				if(part == 1)
					for (int i = 0; i < objectsOfWords.Length; i++)
					{
						//set the appropriate sprite, and change its name
	
						objectsOfWords [i].GetComponent<Image> ().sprite = spritesForSet [i];
						objectsOfWords [i].name = spritesForSet [i].name;
						//move the object to screen
						StartCoroutine (MoveObjectToScreen (objectsOfWords [i]));
					}
				else if(part ==2) // part 2 takes the second half of the sprites of the set
					for (int i = 0; i < objectsOfWords.Length; i++)
					{
						//set the appropriate sprite, and change its name
			
						objectsOfWords [i].GetComponent<Image> ().sprite = spritesForSet [i+objectsOfWords.Length]; 
						objectsOfWords [i].name = spritesForSet [i+objectsOfWords.Length].name;
						//move the object to screen
						StartCoroutine (MoveObjectToScreen (objectsOfWords [i]));
					}

				yield return new WaitForSeconds (4);
				Init ();
			} 
			else //If not on index 3
			{
				yield return new WaitForSecondsRealtime (2);
				//DeactivateAllObjects but the focused one
				for (int i = 0; i < objectsOfWords.Length; i++)
					if (i != indexOfFocused)
						objectsOfWords [i].SetActive (false);
					else
						objectsOfWords [i].SetActive (true);

				//Init new instance on different index
				Init ();
			}
		}

		/// <summary>
		/// Preps to make all objects but the focused object to dissapear. (Step 1)
		/// </summary>
		/// <returns>The image vanish.</returns>
		private IEnumerator PrepImageVanish()
		{
			while(waitingForAudio || waitingForTransition)
			{
				yield return null;
			}

			//DeactivateAllObjects but the focused one
			for (int i = 0; i < objectsOfWords.Length; i++)
				if (i != indexOfFocused)
					objectsOfWords [i].SetActive (false);
				else
					objectsOfWords [i].SetActive (true);
					
		}

		/// <summary>
		/// Preps the letter pointing action (step 2).
		/// </summary>
		/// <returns>The letter pointing.</returns>
		private IEnumerator PrepLetterJumping()
		{
			while(waitingForAudio || waitingForTransition)
			{
				yield return null;
			}

			//we are now waiting for object transition
			waitingForTransition = true;

			//Disable the horizontal group so we can move x and y of object
			lettersOfWords [0].transform.parent.GetComponent<HorizontalLayoutGroup> ().enabled = false;

			//for each letter
			for (int i = 0; i < lettersOfWords.Length; i++)
			{
				
				RectTransform letterTransform = lettersOfWords [i].GetComponent<RectTransform> ();
				letterTransform.anchoredPosition = new Vector2 (letterTransform.anchoredPosition.x, letterTransform.anchoredPosition.y + 50);
			
				//If audio is playing, wait till over
				while (waitingForAudio)
				{
					yield return null;
				}

				//whats this?
				SoundController.Reference.Playclip (whatsThis);
				TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.TwoSyllableWord, whatsThis.length-.4f);

				//Wait 5 seconds
				yield return new WaitForSecondsRealtime (5);
				//If audio is still playing, wait
				while (waitingForAudio)
				{
					yield return null;
				}

				AudioClip loadedAudio = LoadAudioFromResources (lettersOfWords [i].name, pathForLetterSounds);
				//play the letter sound next
				SoundController.Reference.Playclip(loadedAudio);
				TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, loadedAudio.length-.3f);


				//wait 2 seconds inbetween each
				yield return new WaitForSecondsRealtime (2);

				// parent being enabled does not fix the letters till after loop, so bring these back down after each iteration
				letterTransform.anchoredPosition = new Vector2 (letterTransform.anchoredPosition.x, letterTransform.anchoredPosition.y - 50);


			}

			//enable horizontal group again
			lettersOfWords [0].transform.parent.GetComponent<HorizontalLayoutGroup> ().enabled = true;

			AudioClip rewardAudio = superlatives [Random.Range (0, superlatives.Length)];
			//Play a superlative
			SoundController.Reference.Playclip (rewardAudio);
			TriceratopsAnimationController.ReferenceAnimationController.StartAnimation (TriceratopsAnimationController.TypeOfAnimation.LetsMake, rewardAudio.length);


			while (waitingForAudio)
			{
				yield return null;
			}

			//Done with this transition
			waitingForTransition = false;

		}

		/// <summary>
		/// Waits seconds for inactivity. (step 3)
		/// </summary>
		/// <returns>The for inactivity.</returns>
		/// <param name="seconds">Seconds.</param>
		private IEnumerator WaitForInactivity(float seconds)
		{
			while(waitingForAudio || waitingForTransition)
			{
				yield return null;
			}

			//Coroutine will count as object transition
			waitingForTransition = true;
			//Start accepting clicks
			canWeClick = true;

			lettersOfWords [1].GetComponent<Button> ().interactable = true;
			//wait for appropriate instructions
			yield return new WaitForSecondsRealtime (seconds);
			//if time is up, repeat instructions
			numberOfRepeatInstructions++;
			//Debug.Log (numberOfRepeatInstructions);

			AudioClip letterSound = LoadAudioFromResources (lettersOfWords [1].name, pathForLetterSounds);
			SoundController.Reference.Playclip (canYouFind, letterSound, clickOn, letterSound);

			TriceratopsAnimationClass lSound = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.OneSyllableWord, letterSound.length);
			TriceratopsAnimationClass canYou = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.CanYouFindTheFirst, canYouFind.length);
			TriceratopsAnimationClass click = new TriceratopsAnimationClass (TriceratopsAnimationController.TypeOfAnimation.Awesome, clickOn.length);

			PlayAnimations (canYou, lSound, click, lSound);

			if (numberOfRepeatInstructions == 3)
			{
				Behaviour halo = (Behaviour)lettersOfWords [1].transform.GetChild(0).GetComponent ("Halo");
				halo.enabled = true;
			}

			StopCoroutine (checkingInactivity);
			//wait again.
			checkingInactivity = StartCoroutine (WaitForInactivity (seconds));

					//Done waiting for now.
			waitingForTransition = false;


		}

		/// <summary>
		/// Loads the sprites from resources.
		/// </summary>
		/// <returns>The sprites from resources.</returns>
		/// <param name="path">Path.</param>
		private Sprite[] LoadSpritesFromResources(string path)
		{
			//Loads all sprites in path into an array
			Sprite[] sprites = Resources.LoadAll<Sprite> (path);
			//if sprites is not null, return
			if (sprites != null)
				return sprites;
			//if sprites is null, return null and send message
			else 
			{
				Debug.Log ("No sprites at " + path);
				return null;
			}
		}

		/// <summary>
		/// Loads the audio from resources.
		/// </summary>
		/// <returns>The audio from resources.</returns>
		/// <param name="clipName">Clip name.</param>
		/// <param name="path">Path.</param>
		private AudioClip LoadAudioFromResources(string clipName, string path)
		{
			//exact location of specific file
			string audioLocation = path + clipName;

			//the clip is loaded and stored
			AudioClip loadedAudio = Resources.Load<AudioClip> (audioLocation);

			//if loaded clip is not null, then return
			if (loadedAudio != null)
				return loadedAudio;
			//if loaded clip is null, send message and return empty clip
			else 
			{
				//Debug.Log ("No Audio was loaded at " + audioLocation);
				return null;
			}
		}
	}

	public class TriceratopsAnimationClass
	{
		public TriceratopsAnimationController.TypeOfAnimation animation;
		public float lengthOfAnimation;

		public TriceratopsAnimationClass(TriceratopsAnimationController.TypeOfAnimation anim, float length = 0)
		{
			animation = anim;
			lengthOfAnimation = length;
		}
	}

}
