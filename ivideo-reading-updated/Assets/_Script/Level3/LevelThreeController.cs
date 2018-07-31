using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = System.Random;

/// <summary>
/// Level three controller.
/// </summary>
namespace Level3
{
	/// <summary>
	/// Level thee controller.
	/// This method is responsible for controlling level 2. 
	/// </summary>
	public class LevelThreeController : MonoBehaviour
	{

		#region for public fields

		/// <summary>
		/// The exercise gameobject.
		/// This list holds all Exercise gameobject in which letters are child Gameobject .
		/// </summary>
		public List<GameObject> ExerciseGameobject;


		/// <summary>
		/// The time interval coroutine time gap for iteration .
		/// </summary>
		[Range(0, 10)]
		[Tooltip("WaitForSeconds with this Interval")]
		public float ClickTimeGap;


		/// <summary>
		/// The lit.
		/// lit colot = click state .
		/// Unlit color when not clicked state .
		/// </summary>
		public Color32 Bright, Dim;

		/// <summary>
		/// Gets the attached audio clip.
		/// </summary>
		/// <value>The attached audio source.</value>
		public AudioClip yourTurn, IwilldoitFirst, loss, say,letstryagain;

		


		/// <summary>
		/// The reward audio clip.
		/// </summary>
		public AudioClip[] RewardAudioclip = new AudioClip[3];


		/// <summary>
		/// Exercise groups.
		/// </summary>
		public enum ExerciseGroup : int
		{
			ExerciseOne = 1,
			ExerciseTwo = 2,
			ExerciseThree = 3,
			ExerciseFour = 4,
			ExerciseFive = 5,
			ExerciseSix = 6

		};


		/// <summary>
		/// The current exercise group.
		/// </summary>
		[Header("Default Group One")]
		public ExerciseGroup CurrentExerciseGroup = ExerciseGroup.ExerciseOne;

		/// <summary>
		/// The path of letter's sound.
		/// </summary>
		public readonly string PathForLetterSound = "Letter/";

		/// <summary>
		/// Path of the find commands.
		/// </summary>
		public readonly string PathForFindAudioClip = "Level3/";

		/// <summary>
		/// The hand image.
		/// </summary>
		public GameObject HandImage;

		/// <summary>
		/// The hand image initial position.
		/// </summary>
		public Vector2 HandInitialPosition;





		/// <summary>
		/// The letter will be clicked ondemo.
		/// A list that keep track of letter that will be clicked on demo for different group .
		/// </summary>
		public List<GameObject> LetterWillBeClickedOndemo;





		/// <summary>
		/// Gets the index of the current exercise group.
		/// </summary>
		/// <value>The index of the current exercise group.</value>
		public int CurrentExerciseGroupIndex
		{

			get
			{
				int something = (int)CurrentExerciseGroup;
				return (something - 1);
			}
		}

		/// <summary>
		/// Gets the attached audio source.
		/// </summary>
		/// <value>The attached audio source.</value>
		public AudioSource AttachedAudioSource
		{
			get { return this.gameObject.GetComponent<AudioSource>(); }
		}


		/// <summary>
		/// Gets a value indicating whether this instance is audio playing.
		/// </summary>
		/// <value><c>true</c> if this instance is audio playing; otherwise, <c>false</c>.</value>
		public bool IsAudioPlaying
		{

			get { return (AttachedAudioSource.isPlaying); }
		}



		/// <summary>
		/// Gets the length of current audio clip.
		/// </summary>
		/// <value>The length of current audio clip.</value>
		public float LengthOfCurrentAudioClip
		{

			get { return AttachedAudioSource.clip.length; }
		}



		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Level2.LevelTwoController"/> is disappearing.
		/// </summary>
		/// <value><c>true</c> if disappearing; otherwise, <c>false</c>.</value>
		public bool Disappearing
		{
			get { return disappearing; }
			private set { disappearing = value; }
		}

		/// <summary>
		/// Gets the number of letter clicked.
		/// </summary>
		/// <value>The number of letter clicked.</value>
		public int NumberOfLetterClicked
		{
			get { return (numberOfLetterClicked); }
			private set { numberOfLetterClicked = value; }
		}

		/// <summary>
		/// Gets the number of time level iS running.
		/// </summary>
		/// <value>The number of time level IS running.</value>
		public int NumberOfTimeLevelISRunning
		{
			get { return numberOftimeLevelIsRunning; }
			private set
			{
				numberOftimeLevelIsRunning = value;
			}
		}



		//when Demo just started number of letter clicked is zero .
		//And Number of Time level running is 1 only .
		//So the entire condition should be false to run the clip .
		//
		public bool ISDemoComplete
		{

			get
			{
				if (!(NumberOfLetterClicked == 0 && NumberOfTimeLevelISRunning == 1))
					return true;
				else
					return false;
			}

		}

		#endregion


		#region for private fields


		/// <summary>
		/// audio clip for the find command
		/// </summary>
		AudioClip find;


		/// <summary>
		/// The disappearing.
		/// </summary>
		private bool disappearing = false;

		/// <summary>
		/// This bool is true when game is started .
		/// And becomes false after the game started .
		/// </summary>
		private bool gameWillbeStarted = true;

		/// <summary>
		/// The number of letter clicked.
		/// Keep track of number of letter clicked .
		/// </summary>
		private int numberOfLetterClicked = 0;

		/// <summary>
		/// The number of time level is running.
		/// </summary>
		private int numberOftimeLevelIsRunning = 1;

		[SerializeField]
		/// <summary>
		/// The letters position.
		/// </summary>
		private List<Vector2> lettersPosition;

		/// <summary>
		/// holds the target letter which the child should touch
		/// </summary>
		private GameObject target;


		/// <summary>
		/// stores the current number of mistakes the child has made on the current letter
		/// </summary>
		private int errorCount = 0;
		private Dictionary<string, bool> pickedLetters;

		#endregion




		#region For unity Message system

		/// <summary>
		/// Awake this instance.
		/// </summary>
		private void Awake()
		{
			pickedLetters = new Dictionary<string, bool>();


			for (int i = 0; i < ExerciseGameobject.Count; i++)
			{
				int first = i;
				for (int j = 0; j < ExerciseGameobject[i].transform.childCount; j++)
				{
					int second = j;
					Debug.Log(ExerciseGameobject[i].transform.GetChild(j).gameObject.name);
					ExerciseGameobject[i].transform.GetChild(j).gameObject.SetActive(true);
					ExerciseGameobject[i].transform.GetChild(j).gameObject.GetComponent<Button>().onClick.AddListener(delegate 
					{ OnLetterClicked(ExerciseGameobject[first].transform.GetChild(second).gameObject); });
					pickedLetters.Add(ExerciseGameobject[i].transform.GetChild(j).gameObject.name, false);
				}
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		private void Start()
		{

			Init();
		}
		#endregion




		#region For Functions


		/// <summary>
		/// Subscribed event triggered by player clicking on a letter. Checks if the letter is the
		/// target or not, and checks how many errors the child has made
		/// </summary>
		/// <param name="letter"></param>
		protected void OnLetterClicked(GameObject letter)
		{
			StopAllCoroutines();
			AttachedAudioSource.Stop();
			if (letter == target)
			{
				//reset the errors
				errorCount = 0;
				//start the success party
				StartCoroutine(Success());
			}
			else
			{
				Debug.Log("Wrong");
				errorCount++;
				if (errorCount < 3)
				{
					//start the mistake coroutine
					StartCoroutine(Mistake());
				}
				else
				{
					//the child made too many mistakes, show them the letter, and make them repeat
					StartCoroutine(MoveLetterToCenter(target));
				}
			}

		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{


			//Set color for all the letters
			//SetColour (Bright, GameObject.FindGameObjectsWithTag ("LetterImage"));
			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++)
			{
				//Get the gameobject.
				GameObject go = ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).transform.gameObject;
				SetColour(Bright, go);
			}
			//Activate particular group
			ActivateGroup(CurrentExerciseGroupIndex);


			//Start the demo
			//Only for Fresh level demo will be shown
			if (NumberOfTimeLevelISRunning == 1)
			{

				StartCoroutine(Demo());
			}
			else
			{
				StartCoroutine(PickNextTarget());
			}

		}

		
		/// <summary>
		/// Demo this instance.
		/// This method Do the First demo of click the letter 
		/// </summary>
		public IEnumerator Demo()
		{

			//prevent the child from clicking on anything during the demo
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = false;
			}


			

			find = LoadAudioClipFromResources(PathForFindAudioClip,
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(0).gameObject.name);
			Play(find);

			yield return new WaitForSeconds(find.length);

			//Play the sound I will do it first
			Play(IwilldoitFirst);

			//Press the letter
			//After all the clip is done 
			yield return new WaitForSeconds(IwilldoitFirst.length);

			//Start the hand movement
			StartCoroutine(MoveHand());
			
		}


		private IEnumerator MoveHand()
		{



			//Activate Hand
			if (!HandImage.activeInHierarchy)
			{
				HandImage.SetActive(true);
			}

			//adjust the handspeed so that it always takes the same amount of time to find the demo letter
			float handSpeed = 10f / Vector3.Distance(LetterWillBeClickedOndemo[CurrentExerciseGroupIndex].transform.position,
				HandImage.transform.position);

			
			
			Vector3 t = new Vector3();
			yield return new WaitForSeconds(.5f);


			//move the hand
			while (Vector3.Distance(LetterWillBeClickedOndemo[CurrentExerciseGroupIndex].transform.position, HandImage.transform.position) >= 0.1f)
			{
				t = (LetterWillBeClickedOndemo[CurrentExerciseGroupIndex].transform.position - HandImage.transform.position).normalized 
					* handSpeed * Time.deltaTime;
				t.z = 0;
				HandImage.transform.position += t;
				yield return null;
			}

			FocusOnThisGameobject(LetterWillBeClickedOndemo[CurrentExerciseGroupIndex]);



			yield return new WaitForSeconds(.75f);
			//hide the hand
			HandImage.SetActive(false);

			Play(say);

			yield return new WaitForSeconds(say.length);

			AudioClip clip = LoadAudioClipFromResources(PathForLetterSound, LetterWillBeClickedOndemo[CurrentExerciseGroupIndex].name);
			Play(clip);

			yield return new WaitForSeconds(clip.length);




			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				SetColour(Bright, ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).gameObject);
			}
			
			
			
			//Send Hand Back to its previous position .
			HandImage.GetComponent<RectTransform>().anchoredPosition = HandInitialPosition;


			Play(yourTurn);

			yield return new WaitForSeconds(yourTurn.length);

			//allow the child to click on buttons after the demo
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = true;
			}

			ReactivateChild();

			StartCoroutine(PickNextTarget());

		}


		/// <summary>
		/// The student has made three mistakes in a row, so move the letter to the center, then have them repeat
		/// the sound. Lastly, pick a new letter
		/// </summary>
		/// <param name="letter"></param>
		/// <returns></returns>
		private IEnumerator MoveLetterToCenter(GameObject letter)
		{
			letter.GetComponent<Button>().enabled = false;
			//Debug.Log("Moving letter");
			Vector2 originalPosition = letter.GetComponent<RectTransform>().anchoredPosition;

			//scale the speed so that it always takes the same amount of time to move the letter
			float Speed = 20000f / Vector2.Distance(originalPosition,Vector2.zero);
			Vector2 t = new Vector2();

			//hide the other letters
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				
				if (ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).gameObject != letter)
				{
					//Debug.Log("Making other letters disappear " + ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).gameObject.name);
					StartCoroutine(DisappearCoroutine(ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).gameObject,
						0f, .5f));
				}
			}

			//move the letter
			while (Vector2.Distance(letter.GetComponent<RectTransform>().anchoredPosition, Vector2.zero) >= 1f)
			{
				t = (Vector2.zero - letter.GetComponent<RectTransform>().anchoredPosition).normalized * Speed * Time.deltaTime;
				letter.GetComponent<RectTransform>().anchoredPosition += t;
				yield return null;
			}

			//command to say
			Play(say);
			yield return new WaitForSeconds(say.length);

			//say the letter's sound
			AudioClip clip = LoadAudioClipFromResources(PathForLetterSound, letter.name);
			Play(clip);

			yield return new WaitForSeconds(clip.length);

			//return the letter to its original position
			letter.GetComponent<RectTransform>().anchoredPosition = originalPosition;

			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				SetColour(Bright, ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).gameObject);
			}

			ReactivateChild();

			//set the flag to true
			pickedLetters[letter.name] = true;
			letter.GetComponent<Button>().enabled = true;

			//increment the number
			NumberOfLetterClicked++;

			StartCoroutine(PickNextTarget());
		}

		/// <summary>
		/// Sets the next exercise group.
		/// </summary>
		private void SetNextExerciseGroup()
		{
			if (CurrentExerciseGroupIndex < 5)
			{
				//First one added for make it same as enum index .which has been deducted by one earlier .
				//Second one added for increment by next .
				CurrentExerciseGroup = (ExerciseGroup)((CurrentExerciseGroupIndex + 1) + 1);
			}
			else
			{
				Debug.Log("No next group is found . Running last group . Level totally completed");
			}
		}

		/// <summary>
		/// Activates the group.
		/// </summary>
		private void ActivateGroup(int index)
		{

			//Activate the parent .
			ExerciseGameobject[index].SetActive(true);
			//Activate the child
			int childcount = ExerciseGameobject[index].transform.childCount;
			for (int i = 0; i < childcount; i++)
			{

				ExerciseGameobject[index].transform.GetChild(i).gameObject.SetActive(true);
			}

			//Deactivate other group
			for (int i = 0; i < ExerciseGameobject.Count; i++)
			{

				if (i == index)
					continue;
				else
				{

					if (ExerciseGameobject[i].activeInHierarchy)
						ExerciseGameobject[i].SetActive(false);
				}

			}



		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="colorwillbeSet">Colorwillbe set.</param>
		/// <param name="list">List.</param>
		private void SetColour(Color32 colorwillbeSet, params GameObject[] list)
		{

			//iterate over the list and set the colour
			foreach (var item in list)
			{
				item.GetComponent<Image>().color = colorwillbeSet;
			}

		}



		/// <summary>
		/// Play the specified Audio clip and immediate.
		/// </summary>
		/// <param name="clipWillbePlayed">Clip willbe played.</param>
		public void Play(AudioClip clipWillbePlayed)
		{

			AttachedAudioSource.clip = clipWillbePlayed;
			//play
			AttachedAudioSource.Play();

		}



		/// <summary>
		/// Arbitaries the index of array.
		/// </summary>
		/// <returns>The index of array.</returns>
		/// <param name="lengthofArray">Lengthof array.</param>
		private int ArbitaryIndexOfArray(int lengthofArray)
		{

			int lowerBound = 0;
			int upperbound = 10000;
			int getaRandomnumber = UnityEngine.Random.Range(lowerBound, upperbound);
			int normalized = getaRandomnumber % lengthofArray;
			return normalized;

		}


		/// <summary>
		/// Raises the level update event.
		/// Change or modify the position of letters if in same level .
		/// Change 
		/// </summary>
		public void OnclickLevelUpdate()
		{


			Debug.Log("Onclick level update is called");


			if (NumberOfLetterClicked >= 4)
			{
				for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
				{
					ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = false;
				}
				//Play Reward Audio
				int rand = ArbitaryIndexOfArray(RewardAudioclip.Length);
				AudioClip clip = RewardAudioclip[rand];
				Play(clip);


				//Advance safety
				StopAllCoroutines();

				Debug.Log("Number of times ran " + NumberOfTimeLevelISRunning);
				if (NumberOfTimeLevelISRunning == 2)
				{
					//Go to the Next level.
					Invoke("OnLevelComplete", clip.length);
					//OnLevelComplete() ;
				}

				if (NumberOfTimeLevelISRunning < 2)
				{
					//increment is first
					NumberOfTimeLevelISRunning++;
					//Restart the level parameters and jumble words.
					Invoke("RestartLevel",clip.length);
					//OnLevelRestart() ;
				}

			}
		}


		public void RestartLevel()
		{
			StartCoroutine(OnLevelRestart());
		}
		/// <summary>
		/// Raises the level restart event.
		/// </summary>
		public IEnumerator OnLevelRestart()
		{

			Debug.Log ("OnLevelRestart is called");

			//When level will be restarted  some The value must be reset .
			//There fields will be reset
			//No property .
			errorCount = 0;
			numberOfLetterClicked = 0;
			disappearing = false;
			gameWillbeStarted = true;

			//get position of child 
			GetLettersPosition();


			//Jumble the word
			JumbleletterPosition();

			//reset the letters
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				pickedLetters[ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).name] = false;
			}

			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = false;
			}

			while (AttachedAudioSource.isPlaying)
			{
				yield return null;
			}

			Play(letstryagain);

			yield return new WaitForSeconds(letstryagain.length);

			//Init the process
			Init();


		}
		/// <summary>
		/// Raises the level complete event.
		/// </summary>
		public void OnLevelComplete()
		{

			Debug.Log ("OnLevelComplete is called");

			//When level will be complete  some The value must be reset .
			//There fields will be reset
			//No property .
			numberOftimeLevelIsRunning = 1;
			numberOfLetterClicked = 0;
			errorCount = 0;
			disappearing = false;
			gameWillbeStarted = true;

			//increment the index
			SetNextExerciseGroup();

			//init
			Init();

		}

		/// <summary>
		/// Loads the audio clip from resources.
		/// </summary>
		/// <returns>The audio clip from resources.</returns>
		/// <param name="path">Subfolder inside resources</param>
		/// <param name="nameOfClip">Name of clip.</param>
		private AudioClip LoadAudioClipFromResources(string path,string nameOfClip)
		{
			string clipwillbeloadedFrom;
			if (path == PathForLetterSound)
			{
				//Clip that will be loaded on fly .
				clipwillbeloadedFrom = string.Concat(PathForLetterSound, nameOfClip, "Sound");
				
				
			}
			else
			{
				//Clip that will be loaded on fly .
				clipwillbeloadedFrom = string.Concat(PathForFindAudioClip, "EX3" ,nameOfClip);
			
				
			}
			//load from the resources.
			AudioClip loaded = Resources.Load(clipwillbeloadedFrom, typeof(AudioClip)) as AudioClip;
			//check null
			if (loaded != null)
			{

				return loaded;
			}
			else
			{

				Debug.Log("Unable to load audio clip ");
				Debug.Log("Check this path===>" + clipwillbeloadedFrom);
				Debug.Log("Check the name of the clip" + nameOfClip);

				//Normal exception handel .
				throw new NullReferenceException("Check the debug log");

			}


		}



		/// <summary>
		/// Focuses this gameobject.
		/// </summary>
		/// <param name="name">Name.</param>
		private void FocusOnThisGameobject(GameObject selected)
		{

			//collect the gameobject from the scene .
			GameObject thisGameobject = selected;

			//Make other gameobject darker
			//find out all child gameobject of the parent.
			int numberofchild = ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount;

			//Iterate over children
			for (int num = 0; num < numberofchild; num++)
			{

				//There should be child and it should be active in Hierarchy and it should not be the gameobject that we have selected
				if (ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(num).gameObject != null &&
					ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(num).gameObject != thisGameobject &&
					ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(num).gameObject.activeInHierarchy)
				{
					ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(num).gameObject.GetComponent<Image>().color = Dim;
				}
			}

		}
		/// <summary>
		/// Disappaers the letter 
		/// </summary>
		private IEnumerator DisappearCoroutine(GameObject objectThatwillbeVanish, float timedelay, float howFastAlphaWillbeDisappear)
		{

			////When any audio is playing Disappear Coroutine will not work
			//while (IsAudioPlaying)
			//{

			//	yield return new WaitForSeconds(LengthOfCurrentAudioClip);
			//}

			if (IsAudioPlaying)
			{
				AttachedAudioSource.Stop();
			}

			//get the image associated the gameobject
			Image attachedImage = objectThatwillbeVanish.GetComponent<Image>();

			//current color .
			Color currentcolor = attachedImage.color;

			//alpha of the image
			float alpha = currentcolor.a;


			//Debug.Log ("current alpha" + alpha);

			//Iterate over the loop 
			while (alpha > 0.4f)
			{

				//Debug.Log ("Running alpha" + alpha);

				//reduce alpha over time 
				alpha -= Time.deltaTime * howFastAlphaWillbeDisappear;

				//current color 
				Color c = attachedImage.color;

				//Set the color 
				c.a = alpha;

				//Set the color 
				attachedImage.color = c;


				//Ieterate over a delay 
				yield return new WaitForSeconds(timedelay);
			}

			//Deactivate the gameobject when it is done .
			objectThatwillbeVanish.SetActive(false);

			//Refun it's own alpha value 

			//set a new color
			Color newColor = Bright;

			//Set color 
			attachedImage.color = newColor;

			//Let the button accept the responce
			objectThatwillbeVanish.GetComponent<Button>().enabled = true;

			//revert colour of the remaining gameobjects
			SetColour(Bright, GameObject.FindGameObjectsWithTag("LetterImage"));

			//disappearing ended 
			Disappearing = false;
		}

		/// <summary>
		/// Gets the letters position.
		/// This method keep track of the position in Rect Transform  of the child  for current exercise .
		/// </summary>
		public void GetLettersPosition()
		{

			//Debug.Log ("GetLettersPosition is called");
			//DeActivateAllchild ();

			ReactivateChild();

			//Initialize the list .
			lettersPosition = new List<Vector2>();
			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);
			//Debug.Log ("numberofchild===>"+numberofchild);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++)
			{
				//assign the value to the loop .
				lettersPosition.Add(ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
			}
			return;
		}




		/// <summary>
		/// Jumbleletters the position.
		/// Fisher and Yates' method of shuffle is used .
		/// Modern approach 
		/// </summary>
		private void JumbleletterPosition()
		{

			//Debug.Log ("JumbleletterPosition is called");

			if (lettersPosition != null)
			{

				//Fist find out length of the list
				int lengthOfList = (lettersPosition.Count - 1);

				//iterate over the list 
				for (int i = 0; i < lengthOfList; i++)
				{

					//chose any item of list
					Random rand = new Random();

					//HACK:
					//Range in which we will find out the number .
					//when i = 0 ; predictedNumber [0,lengthOfList] ;
					//when i = 1; predictedNumber [0,lengthOfList - 1] ;
					//when i = 2 ; predictedNumber [0,lengthOfList - 2] ;

					int predictedNumber = rand.Next(lengthOfList - i);

					//Get the element of predictedNumber .
					var predictedelement = lettersPosition[predictedNumber];

					//Get the last element of the list .
					var lastelemntOftheList = lettersPosition[lengthOfList - i];


					//Make a temporary variable .
					var temp = lastelemntOftheList;


					//Make the swap .
					lettersPosition[lengthOfList - i] = predictedelement;
					lettersPosition[predictedNumber] = temp;

				}

				SetLetterPosition();

			}
		}
		/// <summary>
		/// Sets the letter position after shuffling .
		/// </summary>
		private void SetLetterPosition()
		{

			//Debug.Log ("SetLetterPosition is called");

			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++)
			{
				//Get the gameobject.
				GameObject go = ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).transform.gameObject;
				//set the value.

				go.GetComponent<RectTransform>().anchoredPosition = lettersPosition[i];
			}

			//ReactivateChild ();
		}



		/// <summary>
		/// Deactivates the all children.
		/// </summary>
		private void DeActivateAllchildren()
		{
			//Deactivate all the letters 
			//Debug.Log("DeActivateAllchild is called");

			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);

			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++)
			{
				//Get the gameobject.
				GameObject go = ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).transform.gameObject;
				//Deactivate if active in editor
				if (go.activeInHierarchy)
					go.SetActive(false);
			}
		}



		/// <summary>
		/// Reactivates the child.
		/// </summary>
		private void ReactivateChild()
		{
			//Deactivate all the letters 

			//Debug.Log("ReactivateChild is called");

			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++)
			{

				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).transform.gameObject.SetActive(true);
			}

		}

		/// <summary>
		/// Checks if the current exercise is complete
		/// </summary>
		/// <returns></returns>
		private bool IsExerciseComplete()
		{
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				if (!pickedLetters[ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).name])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// The student made a mistake, so repeat the instruction
		/// </summary>
		/// <returns></returns>
		private IEnumerator Mistake()
		{
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = false;
			}
			Debug.Log("playing mistake");
			Play(loss);

			yield return new WaitForSeconds(loss.length);

			Play(find);
			yield return new WaitForSeconds(find.length/2f);
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = true;
			}
			StartCoroutine(WaitforClick());
		}


		/// <summary>
		/// The child has successfully picked the correct letter, so praise them, focus on the letter,
		/// then make it disappear and then pick the next letter
		/// </summary>
		/// <returns></returns>
		private IEnumerator Success()
		{
			//increment the number
			NumberOfLetterClicked++;
			Debug.Log("Clicked " + NumberOfLetterClicked +" Letters");

			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = false;
			}

			FocusOnThisGameobject(target);
			Play(RewardAudioclip[ArbitaryIndexOfArray(RewardAudioclip.Length)]);

			yield return new WaitForSeconds(say.length);

			Play(say);

			yield return new WaitForSeconds(say.length);

			//set the flag
			pickedLetters[target.name] = true;

			AudioClip clip = LoadAudioClipFromResources(PathForLetterSound, target.name);
			Play(clip);

			yield return new WaitForSeconds(clip.length);

			//reset color to bright and reactivate them
			for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
			{
				SetColour(Bright, ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).gameObject);
				ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = true;
			}


			StartCoroutine(PickNextTarget());

		}


		/// <summary>
		/// Repeat the instruction to find the target letter every five seconds
		/// </summary>
		/// <returns></returns>
		private IEnumerator WaitforClick()
		{
			yield return new WaitForSeconds(find.length);
			while (true)
			{
				yield return new WaitForSeconds(5f);

				Play(find);
				yield return new WaitForSeconds(find.length);
			}
			
		}

		/// <summary>
		/// Picks a new target letter at random from the unpicked letters
		/// </summary>
		private IEnumerator PickNextTarget()
		{
			OnclickLevelUpdate();

			if (IsAudioPlaying)
			{
				yield return null;

			}

			if (!IsExerciseComplete())
			{
				errorCount = 0;
				int random = ArbitaryIndexOfArray(ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);
				while (pickedLetters[ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(random).name])
				{
					random = ArbitaryIndexOfArray(ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount);
				}

				target = ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(random).gameObject;
				Debug.Log("target " + target.name);
				find = LoadAudioClipFromResources(PathForFindAudioClip, target.name);

				Play(find);
				for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
				{
					ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = false;
				}

				yield return new WaitForSeconds(find.length / 2f);

				for (int i = 0; i < ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount; i++)
				{
					ExerciseGameobject[CurrentExerciseGroupIndex].transform.GetChild(i).GetComponent<Button>().enabled = true;
				}


				StartCoroutine(WaitforClick());
			}
			
		}
		#endregion

	}


}

