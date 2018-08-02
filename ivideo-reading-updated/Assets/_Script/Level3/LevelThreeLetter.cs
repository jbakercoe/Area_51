using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Level3
{
	public class LevelThreeLetter : MonoBehaviour
	{
		#region for Fields 
		//setup delegates to listen for letter clicked
		public delegate void LetterClickHandler(GameObject clickedLetter);
		public event LetterClickHandler LetterClicked;

		//Has this letter been picked up?
		public bool picked;

		#endregion

		#region unity message system 


		// Use this for initialization .
		private void OnEnable()
		{
			//this saves the designers from having to assign this in the inspector
			GetComponent<Button>().onClick.AddListener(delegate { Selected(); });
		}
		#endregion

		public void Selected()
		{
			OnLetterClicked();
		}

		/// <summary>
		/// For notifying subscribers of the click event
		/// </summary>
		protected void OnLetterClicked()
		{
			if (LetterClicked != null)
			{
				LetterClicked(gameObject);
			}
		}
	}
}

