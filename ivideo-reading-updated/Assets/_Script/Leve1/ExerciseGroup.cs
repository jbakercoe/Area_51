using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Exercise list.
/// This class contain all the list of Exercise .
/// </summary>
public class ExerciseGroup : MonoBehaviour
{

	#region public field and property

	//creating reference 
	public static  ExerciseGroup reference ;

	//List that contain all the exercise level gameobject 
    public List<GameObject> Exercises = new List<GameObject>();




	//All the letters are divided into six Group;
	public enum GroupValue:int
	{

		Group1 = 1,
		Group2 = 2,
		Group3 = 3,
		Group4 = 4,
		Group5 = 5,
		Group6 = 6
	};




	//It will show the current Group that is running .
	//This value will be set to 1 for start 
	public GroupValue CurrentGroup = GroupValue.Group1 ; 

	/// <summary>
	/// Gets the current letter.
	/// In the form of child index ;
	/// </summary>
	/// <value>The current letter.</value>
	public int CurrentLetter{
		get	{
			return currentVisibleLetterIndex;
		}

		private set{
			currentVisibleLetterIndex = value;
		}
	}

	// Gets the number of letters contain in a group.
	public int NumberOflettersContanInaGroup { 
		get {
			return ( Exercises [(int)CurrentGroup - 1].transform.childCount - 1 ); 
		} 
	}


	#endregion


	#region of private field 


	//Letter that will be visible at start .
	[SerializeField]
	private int currentVisibleLetterIndex = 1 ;





	#endregion


	#region unity message system 

	//create a singletone reference 
	private void Awake(){

		if (reference == null) {

			reference = this;
		} else {
			Destroy (this);
		}
			
	}

	private void Start() {

		/*
		//Activate the current Group ;
		int currentgroupindex = (int)CurrentGroup ;
		int correspondinglistindex = currentgroupindex - 1;
		Exercises [correspondinglistindex].SetActive (true);
		*/

		DeactivateOtherGroupAtstartup (CurrentGroup);
		ActivateOnlytheFirstletterOfaGroup() ;

		
	}

	#endregion


	public void ActivateNextGroup(){

		//check the index of current group
		int currentGroupvalue = (int)CurrentGroup ;
		//Running in list .
		//list index must be one less than group value .
		int indexOfGroup = currentGroupvalue - 1 ;

		//deactivate the group.
		Exercises[indexOfGroup].SetActive(false);

		//Activate Next Group
		//if not last group 
		if (Exercises [indexOfGroup + 1] != null) {
			Exercises [indexOfGroup + 1].SetActive (true);

		}

		//set next group
		//check upper limit of group
		int upperlimitofGroup = 6;

		if (currentGroupvalue != upperlimitofGroup) {

			CurrentGroup = (GroupValue)(currentGroupvalue+1);
		} else {
			Debug.Log ("Running Last group , No more groups available");
		}

		//Activate it's first letter
		ActivateOnlytheFirstletterOfaGroup() ;
	}

	/// <summary>
	/// Deactivates the other group at startup.
	/// Activate Groups that marked as Current group .
	/// Deactivate other Group .
	/// </summary>
	/// <param name="current">Current.</param>
	private void DeactivateOtherGroupAtstartup( GroupValue current){

		//Get current group index .
		int currentindex = (int)current;
		int listindex = currentindex - 1;
		//iterate over a for loop .
		for (int i = 0; i < Exercises.Count; i++) {
			//first check 
			//the game object is active and it is not the current group thet need to activate .
			if (Exercises [i].activeInHierarchy) {//game object active 
				
				if (i != listindex) {
						Exercises [i].SetActive (false);
					} else {
						continue;
					}


				} 

				else {
						
					//gameobject not active .
				if (i == listindex) {
						Exercises [i].SetActive (true);
					} else {
						continue;
					}


				}
			
		}


	}



	/// <summary>
	/// Activate next letter .
	/// </summary>
	/// <returns>The nextletter.</returns>
	public void ActivateNextletter() {
		
		int nextletterindex = CurrentLetter + 1;

		if (nextletterindex > NumberOflettersContanInaGroup) {


			//Reset Values
			CurrentLetter = 0;

			//Move to next gorup ;
			ActivateNextGroup ();



			return;
			
		}




		if (nextletterindex <= NumberOflettersContanInaGroup) {

			//Deactivate the current letter
			Exercises [(int)CurrentGroup - 1].transform.GetChild(CurrentLetter).gameObject.SetActive(false);

			//increment the current 
			CurrentLetter = CurrentLetter + 1 ;

			//Activate the current letter
			Exercises [(int)CurrentGroup - 1].transform.GetChild(CurrentLetter).gameObject.SetActive(true);

		}
	}


	/// <summary>
	/// Activates the only the first letter of a group.
	/// Other letter will be deactivated if by mistake activated in unity  editor .
	/// </summary>
	public void ActivateOnlytheFirstletterOfaGroup(){
		
		Debug.Log ("NumberOflettersContanInaGroup"+NumberOflettersContanInaGroup);
		Debug.Log ("Current letter :" + CurrentLetter);

		for (int i = 0; i <= NumberOflettersContanInaGroup; i++) {

			if (i == CurrentLetter) {
				Exercises [(int)CurrentGroup - 1].transform.GetChild (i).gameObject.SetActive (true);
			} else {
				if (Exercises [(int)CurrentGroup - 1].transform.GetChild (i).gameObject.activeInHierarchy) {
					
					Exercises [(int)CurrentGroup - 1].transform.GetChild (i).gameObject.SetActive (false);
				}

			}
		}
	}


}