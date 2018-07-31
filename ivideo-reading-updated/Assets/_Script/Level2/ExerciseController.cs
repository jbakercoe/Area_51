using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System ;

//We want to use random class of system .
using Random = System.Random ;
[Obsolete("This class is obsolate and no longer used in game",true)]
public class ExerciseController : MonoBehaviour 
{
	#region public field 
	public static ExerciseController ec;
	public List<GameObject> exercises;
	/// <summary>
	/// The exercise counter.
	/// This counter keep track of the number of times lettere will be appear .
	/// if the same exercise is running two times go to next exercise else jumble the word only .
	/// </summary>
	public int ExerciseCounter = 1 ;
	#endregion

	#region private field
	private int currentIndex;
	[SerializeField]
	private List<Vector2> lettersPosition;
	#endregion



	#region unity field

	// Use this for initialization
	private void Awake()
	{
		ec = this;
	}

	private void Start () 
	{
		//Set the current index for the fist index .
		currentIndex = 0;

		//Get the letter's position with respect exercise .
		GetLettersPosition ();

		//Deactivte other object .
		for (int i = 1; i < exercises.Count; i++)
			exercises [i].SetActive (false);
			
	}

	#endregion


	/// <summary>
	/// Advances the group.
	/// Go to the next Group .
	/// </summary>
	[Obsolete("This class is obsolate and no longer used in game",true)]
	public void AdvanceGroup()
	{

		if (ExerciseCounter == 2) {

			Debug.Log ("Go to the next level ");

			//Activate the particular group 
			exercises [currentIndex + 1].SetActive (true);

			//get its child's position .
			GetLettersPosition ();

			//let remainig gameobject deactivated .
			for (int i = 0; i < exercises.Count; i++) {
				if (i != currentIndex + 1)
					exercises [i].SetActive (false);
			}
			//increment the current index 
			currentIndex = currentIndex + 1;

			//Reset the value
			ExerciseCounter = 0 ;
		} else {

			Invoke ("ShowGroupTwice", 10f);
		}
	}

	public void ShowGroupTwice(){
		
		Debug.Log ("Run the same scene twice ");
		//First deactivate all child
		DeActivateAllchild ();
		//Jumble their position
		JumbleletterPosition ();
		//Re activate the child
		ReactivateChild ();
		//Increment the exercise counter 
		ExerciseCounter = ExerciseCounter + 1;
		//safe side invoke cancellation
		CancelInvoke ("ShowGroupTwice");

	}

	/// <summary>
	/// Gets the letters position.
	/// This method keep track of the position in Rect Transform  of the child  for current exercise .
	/// </summary>
	public void GetLettersPosition() {
		//Initialize the list .
		lettersPosition = new List<Vector2> ();
		//Find out number of child for current group .
		int numberofchild = (exercises [currentIndex].transform.childCount);
		//Debug.Log ("numberofchild===>"+numberofchild);
		//iterate over the loop .
		for (int i = 0; i < numberofchild ; i++) {
			//assign the value to the loop .
			//print child name
			//Debug.Log("Child"+i+"name:"+exercises [currentIndex].transform.GetChild (i));
			//Debug.Log ("Corresponding Rect transform" + exercises [currentIndex].transform.GetChild (i).GetComponent<RectTransform>().anchoredPosition);
			lettersPosition.Add( exercises [currentIndex].transform.GetChild (i).GetComponent<RectTransform>().anchoredPosition);
		}
		return;
	}




	/// <summary>
	/// Jumbleletters the position.
	/// Fisher and Yates' method of shuffle is used .
	/// Modern approach 
	/// </summary>
	private void JumbleletterPosition () {

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

		//Find out number of child for current group .
		int numberofchild = (exercises [currentIndex].transform.childCount);
		//iterate over the loop .
		for (int i = 0; i < numberofchild; i++) {
			//Get the gameobject.
			GameObject go = exercises [currentIndex].transform.GetChild (i).transform.gameObject;
			//set the value.
			go.GetComponent<RectTransform> ().anchoredPosition = lettersPosition [i];
		}
	}



	/// <summary>
	/// Deactivates the allchild.
	/// </summary>
	private void DeActivateAllchild(){
		//Deactivate all the letters 

		//Find out number of child for current group .
		int numberofchild = (exercises [currentIndex].transform.childCount);

		//iterate over the loop .
		for (int i = 0; i < numberofchild; i++) {
			//Get the gameobject.
			GameObject go = exercises [currentIndex].transform.GetChild (i).transform.gameObject;
			//Deactivate if active in editor
			if(go.activeInHierarchy)
				go.SetActive(false);
		}
	}



	/// <summary>
	/// Reactivates the child.
	/// </summary>
	private void ReactivateChild(){


		//Find out number of child for current group .
		int numberofchild = (exercises [currentIndex].transform.childCount);
		//iterate over the loop .
		for (int i = 0; i < numberofchild; i++) {

			exercises [currentIndex].transform.GetChild (i).transform.gameObject.SetActive (true);
		}
		
	}

}
