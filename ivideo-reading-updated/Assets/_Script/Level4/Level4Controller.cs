using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Level4
{


    public enum Enviornment
    {
        EnviornmentOne = 0,
        EnviornmentTwo = 1,
        EnviornmentThree = 3,
        EnviornmentFour = 4,
        EnviornmentFive = 5
    }


    /// <summary>
    /// Class responsible for controlling level 4
    /// </summary>
    public class Level4Controller : MonoBehaviour 
	{
		
		#region public variables
		/// <summary>
		/// List of Excercise Gameobjectsm which have letters as children
		/// </summary>
		public List<GameObject> excerciseObjects;

		/// <summary>
		/// List of spawn points for wordForLetterPrefabs
		/// </summary>
		public List<Transform> spawnPoints;

		/// <summary>
		/// represents the buttonPrefab array whose image will eventually be set to some
		/// sprite that corresponds to a specific letter/word
		/// </summary>
		public GameObject[] picturePrefabs;

		/// <summary>
		/// represents the buttonPrefab outline array. objects here will copy the image of picturePrefab
		/// and act to outline its corresponding buttonPrefab
		/// </summary>
		public GameObject[] pictureShadow;

		/// <summary>
		/// audio clip containinf "oops, try again"
		/// </summary>
		public AudioClip oops;

		/// <summary>
		/// audio clip containing "Can you find?"
		/// </summary>
		public AudioClip canYouFind;

		/// <summary>
		/// audio clip containing "The word..."
		/// </summary>
		public AudioClip theWord;

		/// <summary>
		/// audio clip containing "starts with..."
		/// </summary>
		public AudioClip startsWith;

		/// <summary>
		/// the time interval between actions that will start a reaction if max is reached
		/// </summary>
		[Range(0, 15)]
		public float clickTimeGap;

		/// <summary>
		/// the reward audio clips
		/// </summary>
		/// <value> audio clips played to reward player for completing task </value>
		public AudioClip[] rewardClips = new AudioClip[3]; 

		/// <summary>
		/// path to find sound for each word
		/// </summary>
		public readonly string pathForWordSound = "Level4/WordSounds/";

		/// <summary>
		/// path to find voice over that says sound of letter
		/// </summary>
		public readonly string pathForLetterWordSound = "Letter/";

		/// <summary>
		/// path to find a word picture sprites
		/// </summary>
		public readonly string pathForLetterWordPic = "Level4/WordLetters/";

		/// <summary>
		/// path to find sound effects for pictures
		/// </summary>
		public readonly string pathForSFX = "Level4/WordSFX/";

		/// <summary>
		/// gets a value that returns if audio is playing
		/// </summary>
		/// <value> true if audio playing, otherwise false </value>
		public bool isAudioPlaying { get {return (UpdatedAudioSource.Reference.IsAudioPlaying);}}

		/// <summary>
		/// accesses the currentExcerciseGroupIndex
		/// </summary>
		public int currentExcerciseGroupIndex
		{ 
			get { return(excerciseGroupIndex); } 
			private set { excerciseGroupIndex = value; }
		}

		/// <summary>
		/// accesses the index of the children of current excercisegroup
		/// </summary>
		/// <value> the letter out of the group that is currently focused on </value>
		public int currentLetterIndex
		{
			get { return (letterIndex); }
			private set { letterIndex = value; }
		}

		/// <summary>
		/// acesses the array of letters (which are children of excercise groups)
		/// </summary>
		public GameObject[] letterGroup
		{
			get { return (letters); }
			private set { letters = value; }
		}

		/// <summary>
		/// accesses the array of wordPics ( which are eventually loaded from resources )
		/// </summary>
		public Sprite[] wordPics
		{
			get { return (pics); }
			private set { pics = value; }
		}

		#endregion 

		#region private variables
		/// <summary>
		/// represents the excercise group index
		/// </summary>
		private int excerciseGroupIndex = 0;

		/// <summary>
		/// represents the letter index
		/// </summary>
		private int letterIndex = 0;

		/// <summary>
		/// array that represents the current letter group
		/// </summary>
		private GameObject[] letters = null;

		/// <summary>
		/// array that represents pics representing the letters
		/// </summary>
		private Sprite[] pics = null;

		/// <summary>
		/// Coroutine that will be used only to store (lightPictureWhenClickNotRecieved)
		/// </summary>
		private Coroutine willSoonLight = null;

		/// <summary>
		/// boolean that is true when the correct picture has been clicked on. False if not.
		/// </summary>
		private bool hasCorrectBeenPicked = false;

		/// <summary>
		/// list that will hold letter picture for letters in the current word besides the focused letter
		/// </summary>
		private List<GameObject> otherLetters = new List<GameObject>();
		#endregion

		// Use this for initialization
		void Start ()
		{

			wordPics = LoadSpritesFromResources (pathForLetterWordPic);
			Init ();
		}

		/// <summary>
		/// OBSOLETE COD?E
		/// </summary>
		/*void Update()
		{
			if (Input.GetKeyDown (KeyCode.Space))
				excerciseGroupIndex++;
		}*/

		#region all functions

		/// <summary>
		/// initialize this instance
		/// </summary>
		private void Init()
		{
			
			//Destroy extra letters before the new init. to save space
			foreach (GameObject go in otherLetters)
				Destroy (go);
			//hascorrectbeen picked reset to false
			hasCorrectBeenPicked = false;
			//Stops coroutine if running
			if(willSoonLight != null)
				StopCoroutine (willSoonLight);
			
			//sets all wordForLetter pictures inactive first
			foreach (GameObject go in picturePrefabs)
				go.SetActive (false);
			//sets all wordForLetter Shadows inactive
			foreach (GameObject go in pictureShadow)
				go.SetActive (false);
			//Clear spawnPoint List so no overlap when new objects are added in
			spawnPoints.Clear ();
			//adds all spawn points to the list of spawnPoints we will use in code
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("SpawnPoint"))
				spawnPoints.Add (go.GetComponent<RectTransform>());
			
			//Activates an excercise group
			ActivateGroup (currentExcerciseGroupIndex);
			//sets letter group as an array of all the children in current excercise group
			letterGroup = findChildrenArray(excerciseObjects[currentExcerciseGroupIndex]);
			//activates the current letter to be focused on
			ActivateCurrentLetter (currentLetterIndex);

			//Begin Find Object Process
			StartFindObjectProcess ();
		}

		/// <summary>
		/// this is part of the findObjectsProcess
		/// if self is correct, play reward clip
		/// else play "oops, try again"
		public void OnLetterWordClicked(GameObject self)
		{
			//stop coroutine that waits for click
			StopCoroutine (willSoonLight);

			//If hasCorrectBeenPicked is not yet true, do something, if it has, skip this section
			if (!hasCorrectBeenPicked) 
			{
				if (!isAudioPlaying)
				{
					//If self is of the correct first letter
					if (self.name.Substring (0, 1).ToLower ().Equals (letterGroup [currentLetterIndex].name.ToLower ().Substring (0, 1)))
					{
						//Has correct has been picked, set value to true
						hasCorrectBeenPicked = true;
						string nameOfAudio = self.name.Substring (0, 1).ToUpper () + self.name.Substring (1);


						AudioClip sfx = LoadAudioFromResources (nameOfAudio + "SFX", pathForSFX);
						AudioClip loadedAudio = LoadAudioFromResources (nameOfAudio, pathForWordSound);

						//play sfx (name of object)
						//play audio (name of object) next
						SoundController.Reference.StopClip ();
						SoundController.Reference.Playclip (sfx, loadedAudio);
						//Idle animation for duration of sfx
						PatchyAnimAndLength nothing = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.IdleAnimation, sfx.length);
						//animation for worsound
						PatchyAnimAndLength nameAud = new PatchyAnimAndLength (GetCorrectSyllableAnimation (nameOfAudio), loadedAudio.length);
						//playAnimations
						PlayAnimations (nothing, nameAud);



						//play reward audio after
						StartCoroutine (PrepRewardClip (rewardClips [Random.Range (0, rewardClips.Length)]));


						//move to the next letter index
						currentLetterIndex += 1;
						//Start wait Coroutine
						willSoonLight = StartCoroutine (LightPictureWhenClickNotRecieved ());
						//Decide what next step is necessary
						DecideNextStep ();
					} 
					else
					{
						//Start wait coroutine
						willSoonLight = StartCoroutine (LightPictureWhenClickNotRecieved ());

						//play clip saying oops, try again
						SoundController.Reference.Playclip ((oops));
						PatchyAnimAndLength oop = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.Super, .9f);
						PatchyAnimAndLength tryAgain = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.TwoSyllableUtter, oops.length - .9f);
						PlayAnimations (oop, tryAgain);
					}
				}
			}
		}
			
		/// <summary>
		/// activates the excercise group at (index) in our list of excercise groups
		/// also deactivates other groups in the scene
		/// </summary>
		private void ActivateGroup(int index)
		{
			
			//Debug.Log ("activating excercise" + (index+1));
			//activates group of specified index
			excerciseObjects [index].SetActive (true);

			//deactivates other groups
			for (int i = 0; i < excerciseObjects.Count; i++) 
			{
				if (i == index)
					continue;
				if (excerciseObjects [i].activeInHierarchy)
					excerciseObjects [i].SetActive (false);
			}
		}

		/// <summary>
		/// activates the letter at (index)
		/// once the letter is active it will play its audio and begin Find Object Process
		/// also deactivates other letters
		///  </summary>
		private void ActivateCurrentLetter(int index)
		{
			//activates letter at (index)
			letterGroup [index].SetActive (true);

			//deactivates other letters
			for (int i = 0; i < letterGroup.Length; i++) 
			{
				if (i == index)
					continue;
				if (letterGroup [i].activeInHierarchy)
					letterGroup [i].SetActive (false);
			}
		}

		/// <summary>
		/// checks variables in game to see what the next state of the game should be
		/// </summary>
		private void DecideNextStep()
		{
			//if on the last excercise and current letter index is over bounds. You've played through all excercises
			if (currentExcerciseGroupIndex >= excerciseObjects.Count - 1 && currentLetterIndex >= letterGroup.Length)
				Debug.Log ("all excercises Played Through");
			else
				//if not played through all levels, but current letter index is over bounds, prep the next excercise
				if (currentLetterIndex >= letterGroup.Length)
					StartCoroutine (PrepNewGroup ());
				// if currentletterindex is not over bounds, prep the next letter
				else
					StartCoroutine (PrepNewInit ());	
		}

		/// <summary>
		/// returns an array of all children in (parent)
		/// </summary>
		private GameObject[] findChildrenArray(GameObject parent)
		{
			//initializes new array of size parent child count
			GameObject[] childrenArray = new GameObject[parent.transform.childCount];
			//input each child in the new array
			for (int i = 0; i < childrenArray.Length; i++)
				childrenArray [i] = parent.transform.GetChild (i).gameObject;
			//returns the array after it is filled
			return childrenArray;
		}

		/// <summary>
		/// will load audioClip of (clipname) from resources and return it
		/// </summary>
		/// <value> clipname is name of specific clip </value>
		/// <value> path is the path where clip can be found </value>
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

		/// <summary>
		/// loads all sprites from resources and returns an aray
		/// </summary>
		/// <value> path is the path where sprites can be found </value>
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
				//Debug.Log ("No sprites at " + path);
				return null;
			}
		}

		/// <summary>
		/// begins the steps that are necessary for the player to look for a specfied item in the world space
		/// </summary>
		private void StartFindObjectProcess()
		{
			Instruct ();
			//Begin coroutine to wait to scatter the pictures after audio plays
			StartCoroutine (PrepScatterPictures ());

			//after coroutine, game will wait for item to be clicked on which triggers OnLetterWordClicked Method

			//current letter will dissapear after player is instructed to look for the pictures
			foreach(GameObject go in  findChildrenArray( letterGroup [currentLetterIndex]))
				StartCoroutine (DisappaerCoroutine (go, 0, 5));

		}

		/// <summary>
		/// Plays subsequent audio and animations to instruct the player
		/// </summary>
		private void Instruct()
		{
			string nameOfCurrentWord = letterGroup [currentLetterIndex].name;
			AudioClip letterSound = LoadAudioFromResources (letterGroup [currentLetterIndex].name.Substring (0, 1).ToUpper()+"Sound", pathForLetterWordSound);
			AudioClip nameOfWord = LoadAudioFromResources (nameOfCurrentWord, pathForWordSound);

			//Play the necessary clips in order
			SoundController.Reference.Playclip (letterSound, nameOfWord, theWord, nameOfWord, startsWith, letterSound, canYouFind, nameOfWord);

			//Create objects representing animations and their lengths to play simutaneously with audio

			//letterSound audio repeats the sound of the letter twice, create two animations for this
			PatchyAnimAndLength lSound = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.SingleWordUtterAnimation, letterSound.length/2);
			PatchyAnimAndLength lSound2 = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.OneSyllableUtter, letterSound.length/2);
			PatchyAnimAndLength nameWord = new PatchyAnimAndLength (GetCorrectSyllableAnimation (nameOfCurrentWord), nameOfWord.length);
			PatchyAnimAndLength ThWord = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.TheWordStartsWithOne, theWord.length);
			PatchyAnimAndLength starts = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.ThreeSyllableUtter, startsWith.length);
			PatchyAnimAndLength canYou = new PatchyAnimAndLength (PatchyAnimationController.TypeOfAnimation.CanYouFindOne, canYouFind.length);


			PlayAnimations (lSound, lSound2, nameWord, ThWord, nameWord, starts, lSound, lSound2, canYou, nameWord);

		}

		/// <summary>
		/// resets all necessary values that will begin a new group
		/// </summary>
		private void ResetValues()
		{
			//reset letters array
			letters = null;
			//letter index should begin again at 0
			letterIndex = 0;

		}

		/// <summary>
		/// Plays list of animations.
		/// </summary>
		/// <param name="animations">Animation&length objects.</param>
		private void PlayAnimations(params PatchyAnimAndLength[] animations)
		{

			float delaySum = 0;
			for (int i = 0; i < animations.Length; i++)
			{
				PatchyAnimationController.ReferenceAnimationController.StartAnimation (animations [i].animation, animations [i].lengthOfAnimation, 0, delaySum);
				delaySum += animations [i].lengthOfAnimation;
			}
				

		}

		/// <summary>
		/// coroutine that loops until there is no audio playing, then proceeds to scatter pictures
		/// </summary>
		private IEnumerator PrepScatterPictures()
		{
			//While audio is playing, wait the length of the audio
			while (isAudioPlaying) 
			{
				yield return null;

			}

			//Scatters pictures around the area
			ScatterPictures ();

		}

		/// <summary>
		/// coroutine that waits until any audio is done, then plays a reward audio clip
		/// </summary>
		private IEnumerator PrepRewardClip(AudioClip clip)
		{
			//While audio is playing, wait the length of the audio
			while (isAudioPlaying) 
			{
				yield return null;

			}
			//Triggers patchy animation that mimiks speaking a two syllable superlative

			PatchyAnimationController.ReferenceAnimationController.StartAnimation (PatchyAnimationController.TypeOfAnimation.Super, clip.length);
			SoundController.Reference.Playclip (clip);
		}

		/// <summary>
		/// loops until there is no audio playing, then proceeds to intiialize the instance
		/// </summary>
		private IEnumerator PrepNewInit()
		{
			//While audio is playing, wait the length of the audio
			while (isAudioPlaying) 
			{
				yield return null;

			}

			//Wait for one second after audio before transitioning to the news letter
			yield return new WaitForSeconds(1);

			//initialize instance again
			Init ();
		}

		/// <summary>
		/// coroutine that loops until no audio is playing, then updates to a new excercise
		/// </summary>
		private IEnumerator PrepNewGroup()
		{
			//While audio is playing, wait the length of the audio
			while (isAudioPlaying) 
			{
				yield return null;
			
			}
				
			//Wait for one second after audio before transitioning to the next group
			yield return new WaitForSeconds (1);

			// change excercise group index to the next
			excerciseGroupIndex += 1;
			//reset necessary values
			ResetValues ();
			//initialize the instance
			Init ();
		}

		/// <summary>
		/// dissapears the letter
		/// </summary>
		/// Reezo's code
		/// Edited by Edd
		private IEnumerator DisappaerCoroutine(GameObject objectThatwillbeVanish , float timedelay , float howFastAlphaWillbeDisappear){

			//When any audio is playing Disappear Coroutine will not work
			while (isAudioPlaying) 
			{

				yield return null;
			}
				
			//get the image associated the gameobject
			Text attachedImage =  objectThatwillbeVanish.GetComponent<Text>() ;

			//current color .
			Color currentcolor = attachedImage.color; 

			//alpha of the image
			float alpha = attachedImage.color.a;


			//Debug.Log ("current alpha" + alpha);

			//Iterate over the loop 
			while (alpha > 0.4f) 
			{
				//Debug.Log ("Running alpha" + alpha);

				//reduce alpha over time 
				alpha -= Time.deltaTime * howFastAlphaWillbeDisappear ;

				//current color 
				Color c = attachedImage.color;

				//Set the color 
				c.a = alpha ;

				//Set the color 
				attachedImage.color = c;


				//Ieterate over a delay 
				yield return new WaitForSeconds (timedelay) ;
			}

			//Deactivate the gameobject when it is done .
			objectThatwillbeVanish.SetActive (false);
		}

		/// <summary>
		/// coroutine that waits until any current audio is done playing, then waits for a few seconds before revealing the correct picture
		/// </summary>
		private IEnumerator LightPictureWhenClickNotRecieved()
		{
			//while audio is playing, do not progress
			while (isAudioPlaying) 
			{
				yield return null;
			}

			//Waits for clicktimegap amount of seconds
			yield return new WaitForSeconds (clickTimeGap);

			//index 0 of wordForLetterPrefab is always assigned to the correct photo for the specific letter

			Behaviour halo = (Behaviour)picturePrefabs [0].GetComponent ("Halo");
			//enabling the halo will highlight the picture

			halo.enabled = true;
		}
		 
		/// <summary>
		/// Gets the correct syllable animation based on word
		/// </summary>
		/// <returns>The correct syllable animation.</returns>
		/// <param name="word">The word you want to correspond the animation with</param>
		private PatchyAnimationController.TypeOfAnimation GetCorrectSyllableAnimation(string word)
		{
			switch (GetNumberOfSyllables (word))
			{
				case 1:
					return PatchyAnimationController.TypeOfAnimation.SingleWordUtterAnimation;

				case 2:
					return PatchyAnimationController.TypeOfAnimation.TwoSyllableUtter;
				
				case 3:
					return PatchyAnimationController.TypeOfAnimation.ThreeSyllableUtter;

				default:
					return PatchyAnimationController.TypeOfAnimation.ThreeSyllableUtter;

			}
		
		}

		/// <summary>
		/// will add necessary components and properties to the pictures
		/// will scatter the pictures around the scene
		/// will start the coroutine to wait 15 seconds between clicks unless interrupted
		/// </summary>
		private void ScatterPictures()
		{
			GameObject currentLetterWord, randomLetterWord1, randomLetterWord2;

			#region creating three wordToLetterObjects
			//creates character array of the alphabet
			char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

		
			//List to store the alphabet for easier access
			List<string> alphabet = new List<string> ();
			//adds all characters in array as strings to list 
			for (int i = 0; i < alpha.Length; i++)
				alphabet.Add (alpha [i].ToString());
			
			//removes the current letter from the alphabet (we don't want the picture for this letter to appear twice)
			alphabet.Remove (letterGroup [currentLetterIndex].name.Substring(0,1));
		
			//currentLetterWord is the word that is correct.
			//will be set to index 0 of wordToLetterGameObjects
			currentLetterWord = createWordToLetterObject (letterGroup [currentLetterIndex].name.Substring(0,1), picturePrefabs[0]);

			//reference to currentLetterWord halo 
			//ONLY INDEX 0 HAS THIS BAHAVIOUR ATTACHED
			Behaviour halo = (Behaviour)currentLetterWord.GetComponent("Halo");
			if(halo != null)
				halo.enabled = false;
			//random letter among the alphabet
			int randomLetter = Random.Range (0, alphabet.Count);

			//first randomLetterObject
			//will be set to index 1 of wordToLetterGameObjects
			randomLetterWord1 = createWordToLetterObject (alphabet[randomLetter], picturePrefabs[1]);
			//We do not want any repeats
			alphabet.Remove (alphabet [randomLetter]);
			//create a new random
			randomLetter = Random.Range (0, alphabet.Count);

			//second randomLetterObject
			//will be set to index 2 of wordToLetterGameObjects
			randomLetterWord2 = createWordToLetterObject (alphabet[randomLetter], picturePrefabs[2]);

			#endregion

			#region set wordToLetterObjects positions


			//These objects were not active since Init, set them to active now
			currentLetterWord.SetActive (true);
			randomLetterWord1.SetActive (true);
			randomLetterWord2.SetActive (true);

			//sets each of the three letterWords to a random position
			SetRandomPosition(currentLetterWord);
			SetRandomPosition(randomLetterWord1);
			SetRandomPosition(randomLetterWord2);

			#endregion

			//Gives each of the pictures a shadow object that will act as an outline
			for (int i = 0; i < picturePrefabs.Length; i++) 
			{
				pictureShadow [i].SetActive (true);
				pictureShadow [i].GetComponent<RectTransform> ().position = picturePrefabs [i].GetComponent<RectTransform> ().position;
				pictureShadow [i].GetComponent<Image> ().sprite = picturePrefabs [i].GetComponent<Image> ().sprite;
			}

			willSoonLight = StartCoroutine (LightPictureWhenClickNotRecieved ());
		}

		/// <summary>
		/// creates an object that corresponds to the first letter of (letter)
		/// </summary>
		/// <value> letter should be the focused letter </value>
		/// <value> prefab should be the game object that will be modified </value>
		private GameObject createWordToLetterObject(string letter, GameObject prefab)
		{
			//completeobject first copies prefab
			GameObject completeObject = prefab;
			Sprite usedSprite = FindCorrespondingSprite (letter, wordPics);
			//completeobject changes its image to the corresponding sprite
			completeObject.GetComponent<Image> ().sprite = usedSprite;

			//Changes object name to the name of the sprite
			completeObject.name = usedSprite.name;
			return completeObject;
		}

		/// <summary>
		/// looks for a sprite in the loaded resources that corresponds to the first letter of (letter)
		/// should return a sprite. If not there, returns null
		/// </summary>
		private Sprite FindCorrespondingSprite(string letter, Sprite[] spriteArray)
		{
			
			//if a sprite's first character starts with (letter)'s first character, return that sprite
			foreach (Sprite sp in spriteArray)
				if (sp.name.ToLower ().Substring (0,1).Equals (letter.Substring (0,1).ToLower ()))
					return sp;

			//if does not return sprite, it is not available
			Debug.Log ("Failed to find a corresponding photo for " + letter);
			//return null if not found a match
			return null;
		}

		/// <summary>
		/// Gets the number of syllables in word.
		/// </summary>
		/// <returns>The number of syllables.</returns>
		/// <param name="word">The word to be checked for syllables</param>
		private int GetNumberOfSyllables(string word)
		{
			word = word.ToLower ();
			char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };

			int numSyllables = 0;
			//must start at true, if first letter of word is vowel, we want to add it
			bool recentWasConsonant = true;

			for (int i = 0; i < word.Length; i++)
			{
				//if current letter is vowel
				if (word.Substring (i, 1).IndexOfAny(vowels) >= 0)
				{	//If it is y, and is not the last letter of the string
					if (word.Substring (i, 1).Equals ("y") && i < word.Length - 1)
					{
						//if recent was a vowel, and this is a y, and next is a vowel, add another syllable
						if (word.Substring (i + 1, 1).IndexOfAny (vowels) >= 0)
						{
							numSyllables++;
							recentWasConsonant = true;
						} 
						else
							recentWasConsonant = false;
					}
					else //else if a vowel, but not y
						if (recentWasConsonant)
						{
							numSyllables++;
							recentWasConsonant = false;
						}

				} 
				else
					recentWasConsonant = true;

			}

			//if word ends with silent e, but is not "le", which would add a syllable, take 1 off from the count
			if ((word.EndsWith ("e") || word.EndsWith ("es") || word.EndsWith ("ed")) && !word.EndsWith ("le"))
				numSyllables--;

			Debug.Log (word + " has " + numSyllables + " Syllables.");
			return numSyllables;
		}

		/// <summary>
		/// sets letterWord to a random position
		/// uses the list spawnPoints to determine which position will be used
		/// </summary>
		private void SetRandomPosition(GameObject letterWord)
		{
			//initializes an index representing a random spawn point within our spawnPoints list
			int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
			//letterWord's position becomes the chosen random spawn point
			letterWord.GetComponent<RectTransform>().position = spawnPoints[randomSpawnPoint].position;

			//the specific spawn point is removed from the list so that it would not be used again
			spawnPoints.Remove(spawnPoints[randomSpawnPoint]);
		}
		#endregion

	}

	/// <summary>
	/// Class representing patchy animation and its length
	/// </summary>
	public class PatchyAnimAndLength
	{
		/// <summary>
		/// The animation for this object
		/// </summary>
		public PatchyAnimationController.TypeOfAnimation animation;

		/// <summary>
		/// The length of the animation.
		/// </summary>
		public float lengthOfAnimation;

		/// <summary>
		/// Initializes a new instance of the <see cref="Level4.PatchyAnimAndLength"/> class.
		/// </summary>
		/// <param name="anim">The animation.</param>
		/// <param name="length">the length of the animation.</param>
		public PatchyAnimAndLength(PatchyAnimationController.TypeOfAnimation anim, float length = 0)
		{
			//set class variables
			animation = anim;
			lengthOfAnimation = length;
		}
	}
}