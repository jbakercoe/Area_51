using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelEleven;
using UnityEngine;

public class Word : MonoBehaviour
{
    /// <summary>
    /// Sound of word .
    /// </summary>
    public AudioClip SoundOfWord;

    /// <summary>
    /// The audio clip for idiots.
    /// </summary>
    public AudioClip NowYouSaytheSound;

    /// <summary>
    /// say good job to the idiots.
    /// </summary>
    public AudioClip GoodJob;

    /// <summary>
    /// Name of the word.
    /// </summary>
    public string NameOfword;

    /// <summary>
    /// Array contain alphabet names
    /// </summary>
    private char[] _alphabetName;

    /// <summary>
    /// List of alphabet .
    /// </summary>
    public List<Alphabet> AlphabetList;

    /// <summary>
    /// contail small At.
    /// </summary>
    public GameObject ATsmallPanel;

    /// <summary>
    /// Egg Game Object.
    /// </summary>
    /// <returns></returns>
    public GameObject EggGameObject;

    /// <summary>
    /// Onword Complete .
    /// </summary>
    /// <returns></returns>
    public Action OnWordComplete;

    /// <summary>
    /// Alphbet gain this color after completing the first transition.
    /// </summary>
    public Color AlphabetTransitionColorOne;

    /// <summary>
    /// Alphabet gain this color after completeing the second transition.
    /// </summary>
    public Color AlphabetTransitionColorTwo;

    /// <summary>
    /// Color of the Alphabet when it is in word.
    /// </summary>
    public Color AlphabetTransitionColorFinal;


    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        //call init method.
        Init();
        //Hook up the event.
        Alphabet.AlphabetDisappeared += MovetoNextAlphabet;
    }

    public void Init()
    {
        //propulate the character array.
        _alphabetName = NameOfword.ToCharArray();
        //Initialize the first alphabet.
        InitializeTheAlphabet(_alphabetName[0]);
    }
    /// <summary>
    /// Initialize the alphabet with name.
    /// </summary>
    /// <param name="withName">pass the character </param>
    private void InitializeTheAlphabet(char withName)
    {
        //convert the char in string.
        var alphabetName = withName.ToString();

        //find the alphabet with the same name.
        Alphabet selectedalphabet = null;
        
        //iterate over loop
        foreach (var item in AlphabetList)
        {
            if (item.Name == alphabetName)
            {
                selectedalphabet = item;
            }
            else
            {
                continue;
            }
        }
        //if it is null
        if(selectedalphabet==null) return;
        //call init method on alphabet
        selectedalphabet.Init();
    }
    /// <summary>
    /// Move to next Alphabet.
    /// </summary>
    /// <param name="sourceObject">Not used</param>
    /// <param name="eventArgs">Not Used</param>
    private void MovetoNextAlphabet(object sourceObject, EventArgs eventArgs)
    {
        //Name of the alphabet.
        var selectedAlphabet = sourceObject as Alphabet;
        //if it is not alphabet just return
        if (selectedAlphabet == null) return;
        //Name of the alphabet.
        var nameOfTheAlphabetCompleted = selectedAlphabet.Name;
        //last character to string.
        var lastAlphabetName = _alphabetName[1].ToString();

        if (nameOfTheAlphabetCompleted != lastAlphabetName)
        {
            InitializeTheAlphabet(_alphabetName[1]);
            return;
        }
        else
        {
            //Last letter has been shown.
            InitiateAlphabetForWord();
        }
    }
    /// <summary>
    /// Make alphabets ready for move and make the word
    /// </summary>
    private void InitiateAlphabetForWord()
    {
        foreach (var item in AlphabetList)
        {
            item.GetComponent<Alphabet>().MakeAlphabetReadyForWord();

        }

        //Hook the ienumerator.
        var createWordIenumerator = CreateWord(3);
        //start the coroutine.
        StartCoroutine(createWordIenumerator);

    }

    /// <summary>
    /// Create WordHolder.
    /// Bring alphabets closer.
    /// </summary>
    /// <param name="withNumberOfMoves">this number indicate how many times sound will be played.
    /// if total distance is 100 and number of times you want to play the sound is 4 then the 
    /// alphabet will move 25 unit each time and sound will be plyed for every movement.</param>
    /// <returns></returns>
    private IEnumerator CreateWord(int withNumberOfMoves)
    {
        //process of getting first alphabet.
        //get name of first alphabet
        var firstalphabetNmae = _alphabetName[0].ToString();
        //first alphabet.
        Alphabet firstAlphabet = null;
        //get first alphabet.
        foreach (var item in AlphabetList)
        {
            if (item.Name != firstalphabetNmae) continue;
            firstAlphabet = item;
            break;
        }
        //break it if null.
        if (firstAlphabet == null)
            yield break;

        //process of getting second Alphabet.
        //get name of second alphabet
        var secondalphabetNmae = _alphabetName[1].ToString();
        //first alphabet.
        Alphabet secondAlphabet = null;
        //get first alphabet.
        foreach (var item in AlphabetList)
        {
            if (item.Name != secondalphabetNmae) continue;
            secondAlphabet = item;
            break;
        }
        //break it if null.
        if (secondAlphabet == null)
            yield break;

        //that amount of unit will moved on single short.
        var pecentageOfTotalDistance = ((1f / withNumberOfMoves) * 100);
        //percentage for first alphabet
        var pecentageOfTotalDistanceForFirstAlphabet = pecentageOfTotalDistance;
        //percentage for second alphabet
        var pecentageOfTotalDistanceForSecondAlphabet = pecentageOfTotalDistance;

        //set a double counter.
        var counter = withNumberOfMoves * 2;
        //loop counter.
        var loopcounter = 1;
        while (loopcounter++ <= counter)
        {
            //even
            if (FindOddOrEven(loopcounter))
            {

                //move alphabet.
                firstAlphabet.MoveAlphabetOverTime(firstAlphabet.SoundOfAlphabet.length, pecentageOfTotalDistanceForFirstAlphabet,Getcolor(loopcounter));
                //certain percentage.
                pecentageOfTotalDistanceForFirstAlphabet += pecentageOfTotalDistanceForFirstAlphabet;
                //play the sound
                SoundController.Reference.Playclip((firstAlphabet.SoundOfAlphabet));
                //wait for the sound to complete.
                yield return new WaitForSeconds(firstAlphabet.SoundOfAlphabet.length);
            }
            //odd
            else
            {

                secondAlphabet.MoveAlphabetOverTime(secondAlphabet.SoundOfAlphabet.length, pecentageOfTotalDistanceForSecondAlphabet,Getcolor(loopcounter));
                pecentageOfTotalDistanceForSecondAlphabet += pecentageOfTotalDistanceForSecondAlphabet;
                SoundController.Reference.Playclip((secondAlphabet.SoundOfAlphabet));
                yield return new WaitForSeconds(secondAlphabet.SoundOfAlphabet.length);
            }
        }
        //play all those sounds.
        SoundController.Reference.Playclip(SoundOfWord, SoundOfWord, NowYouSaytheSound);
        //wait for 10 seconds
        yield return new WaitForSeconds(10f);
        //say good job to the idiots.
        SoundController.Reference.Playclip(GoodJob);
        //Hide all alphabets.
        firstAlphabet.Hide();
        secondAlphabet.Hide();
        //set panel active with small word
        ATsmallPanel.SetActive(true);
        //activate the eggs.
        EggGameObject.SetActive(true);
    }

    /// <summary>
    /// if odd return false.
    /// if even return true.
    /// </summary>
    /// <returns></returns>
    private bool FindOddOrEven(int number)
    {
        return number % 2 == 0;
    }
    /// <summary>
    /// Get the color for each iteration.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private Color Getcolor(int number)
    {
        Color cr;

        switch (number)
        {
            case 1:
                 cr = AlphabetTransitionColorOne;
                return cr;
            case 2:
                cr = AlphabetTransitionColorOne;
                return cr;
            case 3:
                cr=AlphabetTransitionColorTwo;
                return cr;
            case 4:
                cr = AlphabetTransitionColorTwo;
                return cr;
            case 5:
                cr = AlphabetTransitionColorFinal;
                return cr;
            case 6:
                cr = AlphabetTransitionColorFinal;
                return cr;
            default:
                cr = AlphabetTransitionColorFinal;
                return cr;
        }

       
    }
    
   
}