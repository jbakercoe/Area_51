using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using UnityEngine.Events ;
using UnityEngine.EventSystems;
using System ;
using Random = System.Random ;

/// <summary>
/// Level two controller.
/// </summary>
namespace Level2
{
	/// <summary>
	/// Level two controller.
	/// This method is responsible for controlling level 2 . 
	/// </summary>
	public class LevelTwoController : MonoBehaviour {

		#region for public fields

		/// <summary>
		/// The exercise gameobject.
		/// This list holds all Exercise gameobject in which letters are child Gameobject .
		/// </summary>
		public List<GameObject>  ExerciseGameobject ;


		/// <summary>
		/// The time interval coroutine time gap for iteration .
		/// </summary>
		[Range(0,10)]
		[Tooltip("WaitForSeconds with this Interval")]
		public float ClickTimeGap ;


		/// <summary>
		/// The lit.
		/// lit colot = click state .
		/// Unlit color when not clicked state .
		/// </summary>
		public Color32 Bright, Drim;

		/// <summary>
		/// Gets the attached audio clip.
		/// </summary>
		/// <value>The attached audio source.</value>
		public AudioClip choose, yourTurn ,IwilldoitFirst ,Demosound;


		/// <summary>
		/// The reward audio clip.
		/// </summary>
		public AudioClip[] RewardAudioclip = new AudioClip[3] ; 


		/// <summary>
		/// Exercise groups.
		/// </summary>
		public enum ExerciseGroup : int
		{
			ExerciseOne = 1 ,
			ExerciseTwo = 2 ,
			ExerciseThree = 3 ,
			ExerciseFour = 4 ,
			ExerciseFive = 5 ,
			ExerciseSix = 6 

		};


		/// <summary>
		/// The current exercise group.
		/// </summary>
		[Header("Default Group One")]
		public ExerciseGroup CurrentExerciseGroup = ExerciseGroup .ExerciseOne ;

		/// <summary>
		/// Gets the path of the Audio Clip .
		/// </summary>
		public readonly string PathForAudioAsset = "Letter/" ;

		/// <summary>
		/// The hand image.
		/// </summary>
		[System.Obsolete("Property is not used in game no longer ",true)]
		public GameObject HandImage;

		/// <summary>
		/// The hand image initial position.
		/// </summary>
		[System.Obsolete("Property is not used in game no longer ",true)]
		public Vector2 HandInitialPosition;


		/// <summary>
		/// The event system for UI.
		/// </summary>
		[System.Obsolete("Property is not used in game no longer ",true)]
		public EventSystem RunningEventSystem;


		/// <summary>
		/// The letter will be clicked ondemo.
		/// A list that keep track of letter that will be clicked on demo for different group .
		/// </summary>
		public List<GameObject> LetterWillBeClickedOndemo ;


		/// <summary>
		/// Gets the hand image end position.
		/// </summary>
		/// <value>The hand end position.</value>
		[System.Obsolete("Property is not used in game no longer ",true)]
		public Vector2 HandEndPosition {
			
			get{ return(LetterWillBeClickedOndemo [CurrentExerciseGroupIndex].GetComponent<RectTransform> ().anchoredPosition); }
		}

		/// <summary>
		/// Gets the letter that will be clicked on demo.
		/// </summary>
		/// <value>The letter thatwillbe clicked on demo.</value>
		[System.Obsolete("Property is not used in game no longer ",true)]
		public GameObject LetterClickedOnDemo {
			get { return RunningEventSystem.firstSelectedGameObject; }
			private set { RunningEventSystem.firstSelectedGameObject = value; }
		}



		/// <summary>
		/// Gets the index of the current exercise group.
		/// </summary>
		/// <value>The index of the current exercise group.</value>
		public int CurrentExerciseGroupIndex {

			get{ int something = (int)CurrentExerciseGroup;
				return (something - 1);			
			}
		}

		/// <summary>
		/// Gets the attached audio source.
		/// </summary>
		/// <value>The attached audio source.</value>
		public AudioSource AttachedAudioSource {
			 get{ return this.gameObject.GetComponent<AudioSource> (); }
		}


		/// <summary>
		/// Gets a value indicating whether this instance is audio playing.
		/// </summary>
		/// <value><c>true</c> if this instance is audio playing; otherwise, <c>false</c>.</value>
		public bool	IsAudioPlaying {

			get { return (AttachedAudioSource.isPlaying); }
		}



		/// <summary>
		/// Gets the length of current audio clip.
		/// </summary>
		/// <value>The length of current audio clip.</value>
		public float LengthOfCurrentAudioClip {
			
			get{ return AttachedAudioSource.clip.length; }
		}

		/// <summary>
		/// Gets a value indicating whether this Letter is clicked or not.
		/// </summary>
		/// <value><c>true</c> if this instance is click received; otherwise, <c>false</c>.</value>
		[System.Obsolete("Property is not used in game no longer ",true)]
		public bool IsClickReceived {

			get {return isClickReceived; }
			private set { isClickReceived = value; }
			
		}

		/// <summary>
		/// Gets a value indicating whether letters are shaking or not .
		/// </summary>
		/// <value><c>true</c> if I sshaking; otherwise, <c>false</c>.</value>
		public bool ISshaking {
			get{ return isShaking; }
			private set{  isShaking = value; }
		}
		/// <summary>
		/// Gets the name of button clicked.
		/// </summary>
		/// <value>The name of button clicked.</value>
		public string NameOfButtonClicked {
			get{ 
				//detect first click by demo
				if (NumberOfLetterClicked == 1 && NumberOfTimeLevelISRunning == 1) {
					return LetterWillBeClickedOndemo[CurrentExerciseGroupIndex].name;
				}
				else{
				} return (EventSystem.current.currentSelectedGameObject.name); } 
		}
		/// <summary>
		/// Get the button that is clicked.
		/// </summary>
		/// <value>The button thatis clicked.</value>
		public GameObject ButtonThatisClicked{

			get {//Detect First click by demo
				//Debug.Log("NumberOfLetterClicked"+NumberOfLetterClicked);
				//Debug.Log ("NumberOfTimeLevelISRunning" + NumberOfTimeLevelISRunning);

				if (NumberOfLetterClicked == 1 && NumberOfTimeLevelISRunning == 1) {
					return LetterWillBeClickedOndemo [CurrentExerciseGroupIndex];
				} else {

					return (EventSystem.current.currentSelectedGameObject);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Level2.LevelTwoController"/> is disappearing.
		/// </summary>
		/// <value><c>true</c> if disappearing; otherwise, <c>false</c>.</value>
		public bool Disappearing {
			get { return disappearing; }
		private	set {disappearing = value; }
		}

		/// <summary>
		/// Gets the number of letter clicked.
		/// </summary>
		/// <value>The number of letter clicked.</value>
		public int NumberOfLetterClicked {
			get{ return (numberOfLetterClicked); }
			private	set{ numberOfLetterClicked = value; }
		}

		/// <summary>
		/// Gets the number of time level iS running.
		/// </summary>
		/// <value>The number of time level IS running.</value>
		public int NumberOfTimeLevelISRunning {
			get{ return numberOftimeLevelIsRunning;}
			private set{ 

				if(numberOftimeLevelIsRunning < 2)
							numberOftimeLevelIsRunning = value ;
				else
					Debug.Log("Same level can not be loaded mor than twice");
			}
		}


		/// <summary>
		/// Gets a value indicating whether Demo hand is running or not .
		/// </summary>
		/// <value><c>true</c> if this instance is demo running; otherwise, <c>false</c>.</value>
		[System.Obsolete("This property is depricated no longer used in game",true)]
		public bool IsDemoRunning {
			get{ return (isDemoRunning) ; } 
			private set{ isDemoRunning = value ;}
		}
		//when Demo just started number of letter clicked is zero .
		//And Number of Time level running is 1 only .
		//So the entire condition should be false to run the clip .
		//
		public bool ISDemoComplete {
			
			get {  if (!(NumberOfLetterClicked == 0 && NumberOfTimeLevelISRunning == 1))
				return true;
			else
				return false;
			}
				
		}

		#endregion


		#region for private fields
		/// <summary>
		/// player clicked
		/// </summary>.
		[System.Obsolete("Field is Obsolete and no longer used",true)]
		private bool isClickReceived = false;


		/// <summary>
		/// The onclick received.
		/// </summary>
		private UnityAction OnclickReceived;

		/// <summary>
		/// The  letter is shaking.
		/// </summary>
		private bool isShaking = false ;

		/// <summary>
		/// The shake letter coroutine.
		/// </summary>
		private IEnumerator shakeLetterCoroutine ;

		/// <summary>
		/// The disappearing.
		/// </summary>
		private bool disappearing = false ;

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
		private int numberOftimeLevelIsRunning = 1 ;

		[SerializeField]
		/// <summary>
		/// The letters position.
		/// </summary>
		private List<Vector2> lettersPosition;

		/// <summary>
		/// There is an audio which is  waiting to be played.
		/// </summary>
		[System.Obsolete("Property is not used in game no longer ",true)]
		private bool isAudioWaitingTobePlayed = false;



		/// <summary>
		/// The demo is  running.
		/// </summary>
		[System.Obsolete("This property is depricated no longer used in game",true)]
		private bool isDemoRunning = false; 

		#endregion




		#region For unity Message system

		/// <summary>
		/// Awake this instance.
		/// </summary>
		private void Awake(){
			
			//Register the actions
			OnclickReceived +=OnClickLetter ;
			//OnclickReceived += OnclickLevelUpdate;

			
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		private void Start(){
			
			Init ();
		}
		#endregion




		#region For Functions

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init(){


			//Set color for all the letters
			//SetColour (Bright, GameObject.FindGameObjectsWithTag ("LetterImage"));
			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject [CurrentExerciseGroupIndex].transform.childCount);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++) {
				//Get the gameobject.
				GameObject go = ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (i).transform.gameObject;
				SetColour (Bright, go);
			}
			//Activate particular group
			ActivateGroup(CurrentExerciseGroupIndex) ;
			//Add listener
			AttachListenerWithButton() ;
			//Start the Method for shaking letter
			shakeLetterCoroutine = ShakeLetterWhenClickIsNotReceived(10f);
			StartCoroutine (shakeLetterCoroutine);
			//Start the demo
			//Only for Fresh level demo will be shown
			if (NumberOfTimeLevelISRunning == 1) {
				
				Demo ();
			}

		}

		/// <summary>
		/// Sets the letter for demo.
		/// </summary>
		/// <param name="group">Group.</param>
		[System.Obsolete("Method is not used in game no longer ",true)]
		public void SetTheLetterForDemo(int group){
			
			LetterClickedOnDemo = LetterWillBeClickedOndemo [group];
		}

		/// <summary>
		/// Demo this instance.
		/// This method Do the First demo of click the letter 
		/// </summary>
		public void Demo(){

			//Depricated
			//Set which letter you want to show as demo
			//SetTheLetterForDemo(CurrentExerciseGroupIndex) ;


			//play the demo sound
			Play(Demosound,true);

			//Play the sound I will do it first
			//Play(IwilldoitFirst,false);

			//Press the letter
			//After all the clip is done 
			Invoke("OnClickLetter",(Demosound.length/*+IwilldoitFirst.length*/));


			/* Depricated
			//Run the hand Image
			IEnumerator handMoventRoutine = MoveHand(0.3f,350f);


			//Start it
			StartCoroutine(handMoventRoutine);
			*/


		}

		[System.Obsolete("This Method is depricated no longer used in game",true)]
		private IEnumerator MoveHand(float delay , float speedOfMovingHand){
			
			//Make it true when demo starts 
			IsDemoRunning = true;

			//Activate Hand
			if (!HandImage.activeInHierarchy) {
				HandImage.SetActive (true);
			}


			//Find difference 

			float dist = Vector3.Distance (HandEndPosition, HandInitialPosition);

			//Debug.Log ("Distance between two vector"+dist);


			while (  Vector3.Distance (HandInitialPosition,HandImage.GetComponent<RectTransform> ().anchoredPosition) <= dist ) {

				//Debug.Log ("Moving hand " + Vector3.Distance (HandEndPosition, HandImage.GetComponent<RectTransform> ().anchoredPosition));


				HandImage.GetComponent<RectTransform> ().anchoredPosition = 
					new Vector2 (HandImage.GetComponent<RectTransform> ().anchoredPosition.x,
					HandImage.GetComponent<RectTransform> ().anchoredPosition.y + Time.deltaTime * speedOfMovingHand);

				yield return new WaitForSeconds (delay);
					
				
			}


			//Send Hand Back to its previous position .
			HandImage.GetComponent<RectTransform>().anchoredPosition = HandInitialPosition ;


			//Deactivate the hand image when it is done .
			HandImage.SetActive(false);


			//Demo is over
			IsDemoRunning = false ;


			//Press the letter
			OnClickLetter() ;

		}

		/// <summary>
		/// Sets the next exercise group.
		/// </summary>
		private void SetNextExerciseGroup(){
			if (CurrentExerciseGroupIndex < 5) {
				//First one added for make it same as enum index .which has been deducted by one earlier .
				//Second one added for increment by next .
				CurrentExerciseGroup = (ExerciseGroup) ((CurrentExerciseGroupIndex + 1) + 1) ;
			} else {
				Debug.Log ("No next group is found . Running last group . Level totally completed");
			}
		}

		/// <summary>
		/// Activates the group.
		/// </summary>
		private void ActivateGroup(int index){

			//Activate the parent .
			ExerciseGameobject [index].SetActive (true);
			//Activate the child
			int childcount = ExerciseGameobject [index].transform.childCount;
			for (int i = 0; i < childcount; i++) {

				ExerciseGameobject [index].transform.GetChild (i).gameObject.SetActive (true);
			}

			//Deactivate other group
			for (int i = 0; i < ExerciseGameobject.Count -1 ; i++) {

				if (i == index)
					continue;
				else {

					if (ExerciseGameobject [i].activeInHierarchy)
						ExerciseGameobject [i].SetActive (false);
				}
				
			}



		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="colorwillbeSet">Colorwillbe set.</param>
		/// <param name="list">List.</param>
		private void SetColour( Color32 colorwillbeSet ,params GameObject[] list ){

			//iterate over the list and set the colour
			foreach (var item in list) {
				item.GetComponent<Image> ().color = colorwillbeSet;
			}

		}


		/// <summary>
		/// Shake the the letters when click is not received.
		/// </summary>
		/// <returns>The letter when click is not received.</returns>
		/// <param name="timeInterval">Time interval.</param>
		private IEnumerator ShakeLetterWhenClickIsNotReceived(float timeInterval){


			//this coroutine will run over the game .
			//same as update .
			while (true) {
				//when Audio is playing do not shake the letters.
				//When a player is disappering do not shake the letters .
				if (!IsAudioPlaying && !Disappearing && ISDemoComplete /*&& !IsDemoRunning*/) {
					//play the sound

					//Do not play the sound at the begining .
					//At the very beging .
					//Demo condition .
					if(ISDemoComplete)
						Play (choose, false);
					
					//Shake the letter 
					//We do not want to shake the letter at the very begining .
					//gameWillbeStarted is a bool which remain true before shake the letter is called for once .
					if(!gameWillbeStarted)
						ShaketheLetters (true);

					//Change the value
					//if true then false 
					//if false then remain false 
					gameWillbeStarted = gameWillbeStarted ? false : false ;
				}
				//come back and shake the letter after 10 second delay
				yield return new WaitForSeconds (10f);
			}

		}
		/// <summary>
		/// Play the specified Audio clip and immidiae.
		/// </summary>
		/// <param name="clipWillbePlayed">Clip willbe played.</param>
		/// <param name="immidiae">If set to <c>true</c> immidiae.</param>
		public void Play(AudioClip clipWillbePlayed , bool immidiae){

			//Require to play now
			if (immidiae) {

				if (IsAudioPlaying) {
					//urgent
					//Stop the audio playing
					AttachedAudioSource.Stop ();
					//change the clip
					AttachedAudioSource.clip = clipWillbePlayed;
					//play
					AttachedAudioSource.Play ();
				} else {

					//change the clip
					AttachedAudioSource.clip = clipWillbePlayed;
					//play
					AttachedAudioSource.Play ();
				}
				
			}

			//Do not require to play now
			if (!immidiae) {

				if (IsAudioPlaying) {
					//not urgent
					//Debug.Log ("An Audio is playing.....wait");
					IEnumerator Waitandplay = PlayNextClip(clipWillbePlayed);
					StartCoroutine (Waitandplay);
				} else {
					//change the clip
					AttachedAudioSource.clip = clipWillbePlayed;
					//play
					AttachedAudioSource.Play ();
				}
				
			}
		}
		/// <summary>
		/// play the next clip.
		/// If currently any audio clip is playing then wait for some time u.
		/// Let it be finish first .
		/// Then play the clip
		/// </summary>
		/// <returns>The next.</returns>
		/// <param name="clipWillbePlayed">Clip willbe played.</param>
		private IEnumerator PlayNextClip(AudioClip clipWillbePlayed){

			//isAudioWaitingTobePlayed = true;

			while (IsAudioPlaying) {

				yield return new WaitForSeconds (LengthOfCurrentAudioClip);
			}

			Play (clipWillbePlayed, false);

			//No audio is here to be played next
			//isAudioWaitingTobePlayed = false ;


		}


		/// <summary>
		/// Arbitaries the index of array.
		/// </summary>
		/// <returns>The index of array.</returns>
		/// <param name="lengthofArray">Lengthof array.</param>
		private int ArbitaryIndexOfArray(int lengthofArray){

			int lowerBound = 0;
			int upperbound = 10000;
			int getaRandomnumber = UnityEngine.Random.Range (lowerBound, upperbound);
			int normalized = getaRandomnumber % lengthofArray;
			return normalized;

		}

		/// <summary>
		/// Shakethes the letters.
		/// </summary>
		private void ShaketheLetters( bool shake ){
			Debug.Log ("Method invoked Shake the letter " + shake);
			//change the animation state only when oppsite animation is running.
			//there is no need to iterate over the all child every time .
			//When oppsite animation is not running
			if (ISshaking == !shake ) {
					//find out all child gameobject of the parent.
					int numberofchild = ExerciseGameobject [CurrentExerciseGroupIndex].transform.childCount;

					//Iterate over children
					for (int num = 0; num < numberofchild; num++) {

						//There should be child and it should be active in Hierarchy .
						if (ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject != null &&
						  	ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject.activeInHierarchy  ) {
							ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject.GetComponent<Animator> ().SetBool ("ShakeEm", shake);
						}
				
					}

					ISshaking = shake;
				}

			return;

		}
			
		/// <summary>
		/// Attachs the listener with button.
		/// </summary>
		public void AttachListenerWithButton(){

			//find out all child gameobject of the parent.
			int numberofchild = ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount;

			//Iterate over child 
			for (int num = 0; num < numberofchild ; num++) {

				//There should be child and it should be active in Hierarchy .
				if (ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject != null) {
					ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
					ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject.GetComponent<Button> ().onClick.AddListener (OnclickReceived);
				}
			}


		}




		/// <summary>
		/// Raises the click letter event.
		/// </summary>

		public void OnClickLetter( ){

			//Debug.Log ("On click method Invoked");

			//Do not take click when letter is disappearing or audio is playing
			if (!Disappearing  /* &&!IsAudioPlaying  && !IsDemoRunning*/) {

				//Number of letter clicked will be incremented
				NumberOfLetterClicked = NumberOfLetterClicked + 1;

				//Disappearing started
				Disappearing = true;

				//Set Focous
				FocousThisGameobject (ButtonThatisClicked);

				//If animation is playing make it stop .
				ShaketheLetters (false);

				//Do not allow to take any farther click 
				ButtonThatisClicked.GetComponent<Button> ().enabled = false;

		

				//Load Audioclip .
				//Play the audioclip .
				AudioClip LetterAudioClip = null;
				try {
					LetterAudioClip = LoadAudioClipFromResources (NameOfButtonClicked);
				} catch (NullReferenceException nullRefEx) {
					Debug.Log ("Exception received");
				} finally {
					if (LetterAudioClip != null)
						Play (LetterAudioClip, true);
				}



				//Wait to complete the current audio clip
				//Play the sound you do it .
				Play (yourTurn, false);


				//Start the coroutine That will Disappear the gameobject

				//Debug.Log("Name of clip "+AttachedAudioSource.clip.name);

				//Load an Ienumerator
				IEnumerator vanishtheLetter = DisappaerCoroutine (ButtonThatisClicked, 0.2f, 2f);

				//Start the routine
				StartCoroutine (vanishtheLetter);

			}
			return;

		}

		/// <summary>
		/// Raises the level update event.
		/// Change or modify the position of letters if in same level .
		/// Change 
		/// </summary>
		public void OnclickLevelUpdate(){


			Debug.Log ("Onclick level update is called");


			if (NumberOfLetterClicked >= 4) {
				
				//Play Reward Audio
				Play (RewardAudioclip [ArbitaryIndexOfArray (RewardAudioclip.Length)], false);

				//stop the coroutine that shake the letter
				StopCoroutine(shakeLetterCoroutine);

				//Advance safety
				StopAllCoroutines() ;


				if (NumberOfTimeLevelISRunning == 2) {
					//Go to the Next level.
					Invoke("OnLevelComplete",1f);
					//OnLevelComplete() ;
				}

				if (NumberOfTimeLevelISRunning == 1) {
					//increment is first
					NumberOfTimeLevelISRunning = NumberOfTimeLevelISRunning + 1;
					//Restart the level parameters and jumble words.
					Invoke("OnLevelRestart",1f);
					//OnLevelRestart() ;
				}
				
			}
		}

		/// <summary>
		/// Raises the level restart event.
		/// </summary>
		public void OnLevelRestart(){

			//Debug.Log ("OnLevelRestart is called");

			//When level will be restarted  some The value must be reset .
			//There fields will be reset
			//No property .
			numberOfLetterClicked = 0 ;
			isShaking = false;
			disappearing = false;
			gameWillbeStarted = true;

			//get position of child 
			GetLettersPosition() ;


			//Jumble the word
			JumbleletterPosition() ;


			//Init the process
			Init() ;


		}
		/// <summary>
		/// Raises the level complete event.
		/// </summary>
		public void OnLevelComplete(){

			//Debug.Log ("OnLevelComplete is called");

			//When level will be complete  some The value must be reset .
			//There fields will be reset
			//No property .
			numberOftimeLevelIsRunning = 1;
			numberOfLetterClicked = 0 ;
			isShaking = false;
			disappearing = false;
			gameWillbeStarted = true;

			//increment the index
			SetNextExerciseGroup() ;

			//init
			Init() ;

		}

		/// <summary>
		/// Loads the audio clip from resources.
		/// </summary>
		/// <returns>The audio clip from resources.</returns>
		/// <param name="nameOfClip">Name of clip.</param>
		private AudioClip LoadAudioClipFromResources(string nameOfClip){

			//Clip that will be loaded on fly .
			string clipwillbeloadedFrom = string.Concat(PathForAudioAsset,nameOfClip,"Sound");
			//load from the resources .
			AudioClip loaded = Resources.Load (clipwillbeloadedFrom, typeof(AudioClip))as AudioClip;
			//check null
			if (loaded != null) {
				
				return(loaded);
			} else {

				Debug.Log ("Unable to load audio clip ");
				Debug.Log ("Check this path===>" + clipwillbeloadedFrom);
				Debug.Log ("Check the name of the clip" + nameOfClip);

				//Normal exception handel .
				throw new NullReferenceException ("Check the debug log");

			}

		}



		/// <summary>
		/// Focouses this gameobject.
		/// </summary>
		/// <param name="name">Name.</param>
		private void FocousThisGameobject(GameObject selected){

			//collect the gameobject from the scene .
			GameObject thisGameobject = selected ;

			//Make other gameobject darker
			//find out all child gameobject of the parent.
			int numberofchild = ExerciseGameobject[CurrentExerciseGroupIndex].transform.childCount;

			//Iterate over children
			for (int num = 0; num < numberofchild ; num++) {

				//There should be child and it should be active in Hierarchy and it should not be the gameobject that we have selected
				if (ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject != null && 
					ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject != thisGameobject &&
					ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject.activeInHierarchy ) {
					ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (num).gameObject.GetComponent<Image> ().color = Drim;
				}
			}

		}
		/// <summary>
		/// Disappaers the letter 
		/// </summary>
		private IEnumerator DisappaerCoroutine(GameObject objectThatwillbeVanish , float timedelay , float howFastAlphaWillbeDisappear){

			//When any audio is playing Disappear Coroutine will not work
			while (IsAudioPlaying) {
				
				yield return new WaitForSeconds (LengthOfCurrentAudioClip);
			}



			//get the image associated the gameobject
			Image attachedImage =  objectThatwillbeVanish.GetComponent<Image>() ;

			//current color .
			Color currentcolor = attachedImage.color;

			//alpha of the image
			float alpha = currentcolor.a;


			//Debug.Log ("current alpha" + alpha);

			//Iterate over the loop 
			while (alpha > 0.4f) {

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

			//Refun it's own alpha value 

			//set a new color
			Color newColor = Bright ;

			//Set color 
			attachedImage.color = newColor ;

			//Let the button accept the responce
			objectThatwillbeVanish.GetComponent<Button>().enabled = true ;

			//revert colour of the remaining gameobjects
			SetColour (Bright, GameObject.FindGameObjectsWithTag ("LetterImage"));

			//disappearing ended 
			Disappearing = false;

			//Update the level
			OnclickLevelUpdate();
		}

		/// <summary>
		/// Gets the letters position.
		/// This method keep track of the position in Rect Transform  of the child  for current exercise .
		/// </summary>
		public void GetLettersPosition() {

			//Debug.Log ("GetLettersPosition is called");
			//DeActivateAllchild ();

			ReactivateChild ();

			//Initialize the list .
			lettersPosition = new List<Vector2> ();
			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject [CurrentExerciseGroupIndex].transform.childCount);
			//Debug.Log ("numberofchild===>"+numberofchild);
			//iterate over the loop .
			for (int i = 0; i < numberofchild ; i++) {
				//assign the value to the loop .
				lettersPosition.Add( ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (i).GetComponent<RectTransform>().anchoredPosition);
			}
			return;
		}




		/// <summary>
		/// Jumbleletters the position.
		/// Fisher and Yates' method of shuffle is used .
		/// Modern approach 
		/// </summary>
		private void JumbleletterPosition () {

			//Debug.Log ("JumbleletterPosition is called");

			if (lettersPosition != null) {

				//Fist find out length of the list
				int lengthOfList = (lettersPosition .Count - 1) ;

				//iterate over the list 
				for (int i = 0; i < lengthOfList; i++) {

					//chose any item of list
					Random rand = new Random() ;

					//HACK:
					//Range in which we will find out the number .
					//when i = 0 ; predictedNumber [0,lengthOfList] ;
					//when i = 1; predictedNumber [0,lengthOfList - 1] ;
					//when i = 2 ; predictedNumber [0,lengthOfList - 2] ;

					int predictedNumber = rand.Next(lengthOfList - i) ;

					//Get the element of predictedNumber .
					var predictedelement = lettersPosition [predictedNumber];

					//Get the last element of the list .
					var lastelemntOftheList = lettersPosition[lengthOfList -i] ;


					//Make a temporary variable .
					var temp = lastelemntOftheList ;


					//Make the swap .
					lettersPosition[lengthOfList - i] = predictedelement ;
					lettersPosition [predictedNumber] = temp;

				}

				SetLetterPosition ();

			}
		}
		/// <summary>
		/// Sets the letter position after shuffling .
		/// </summary>
		private void SetLetterPosition(){

			//Debug.Log ("SetLetterPosition is called");

			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject [CurrentExerciseGroupIndex].transform.childCount);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++) {
				//Get the gameobject.
				GameObject go = ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (i).transform.gameObject;
				//set the value.

				//get current anchorposition
				/*
				Vector2 currentAnchor = go.GetComponent<RectTransform> ().anchoredPosition ;

				Vector2 updatedAnchor = lettersPosition [i];

				go.GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (currentAnchor, updatedAnchor, 0.5f);
				*/

				go.GetComponent<RectTransform> ().anchoredPosition = lettersPosition [i];
			}

			//ReactivateChild ();
		}



		/// <summary>
		/// Deactivates the allchild.
		/// </summary>
		private void DeActivateAllchild(){
			//Deactivate all the letters 
			//Debug.Log("DeActivateAllchild is called");

			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject [CurrentExerciseGroupIndex].transform.childCount);

			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++) {
				//Get the gameobject.
				GameObject go = ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (i).transform.gameObject;
				//Deactivate if active in editor
				if(go.activeInHierarchy)
					go.SetActive(false);
			}
		}



		/// <summary>
		/// Reactivates the child.
		/// </summary>
		private void ReactivateChild(){
			//Deactivate all the letters 

			//Debug.Log("ReactivateChild is called");

			//Find out number of child for current group .
			int numberofchild = (ExerciseGameobject [CurrentExerciseGroupIndex].transform.childCount);
			//iterate over the loop .
			for (int i = 0; i < numberofchild; i++) {

				ExerciseGameobject [CurrentExerciseGroupIndex].transform.GetChild (i).transform.gameObject.SetActive (true);
			}

		}
		#endregion

		}


}

