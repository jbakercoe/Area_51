using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LetterController : MonoBehaviour
{
    #region  field

    //public field
    public GameObject WaypointParent;
    public GameObject HandGameOject;
    public GameObject TraceLetterButton;
    public GameObject PointContainer;
    public GameObject Pen;
    public GameObject ColorPanel;
    public AudioClip CurrentAlphabetSound;
    public bool IsHelpRequired;
    public GameObject YourTurn;
    [System.Obsolete("Not used", true)]
    public GameObject Onepix;
    [Tooltip("This is the way point number where sound of current Alphabet will be played")]
    public int SoundWaypointNum;

    //private field
    private Camera Maincamera;
    private GameObject pencilBox;
    private GameObject currentPen;
    private Material UpdatedMaterial;
    private IEnumerator _startDraw;

    #endregion

    #region for property

    private bool IsreadyTowrite { get; set; }

    #endregion

    #region Unity Message system

    public void OnEnable()
    {
        if (ColorPanel.activeInHierarchy) ColorPanel.SetActive(false);
        ColorButton.colorchanged += OnColorChange;
        pencilBox = new GameObject("Pencil Box");
        pencilBox.transform.position = Vector3.zero;
        if (Maincamera == null) Maincamera = Camera.main;
        if (YourTurn != null) YourTurn.SetActive(false);
        if (PointContainer != null) PointContainer.SetActive(false);
        if (TraceLetterButton != null) TraceLetterButton.GetComponent<Button>().onClick.AddListener(ShowAnimation);

        //Coroutine Attached 
        //_startDraw = Draw ();
        _startDraw = DrawUsingLinerender();
    }

    private void OnColorChange(Color pressedButtonColor)
    {
        var material = new Material(Shader.Find("GUI/Text Shader")) {color = pressedButtonColor};
        UpdatedMaterial = material;
    }

    public void Start()
    {
        if (IsHelpRequired)
            Invoke("StartAnimationWithSound", 4f);
        else
            AfterAnimationIsShown();
    }

    public void OnDisable()
    {
        if (TraceLetterButton != null) TraceLetterButton.GetComponent<Button>().onClick.RemoveAllListeners();

        //if any invoke remain then it will be cancelled
        CancelInvoke();
    }

    #endregion

    #region For Normal Function 

    /// <summary>
    ///     Shows the animation.
    ///     Method responsible for runing animaton
    /// </summary>
    private void ShowAnimation()
    {
        //Rejected
        //TraceLetterButton.SetActive(false) ;
        var animationCoroutine = AnimateOverFrame(0.1f); // 0.5f previous value
        StartCoroutine(animationCoroutine);
    }

    /// <summary>
    ///     Animates the over frame.
    ///     Animates Hand over child object
    /// </summary>
    /// <returns>The over frame.</returns>
    /// <param name="timedelay">Timedelay.</param>
    private IEnumerator AnimateOverFrame(float timedelay)
    {
        var numberofchildren = WaypointParent.transform.childCount;
        var num = 0;

        //Formal discrete Movement
        //Obsolate
        /*
         * while (num < numberofchildren -1) {
        HandGameOject.GetComponent<RectTransform> ().position = new Vector2 (
            gameObject.transform.GetChild (num).GetComponent<RectTransform> ().position.x,
            gameObject.transform.GetChild (num).GetComponent<RectTransform> ().position.y);
            num++;
            yield return new WaitForSeconds (timedelay);
            }
            */
        while (num < numberofchildren - 1 && HandGameOject != null)
        {
            HandGameOject.GetComponent<RectTransform>().position = Vector2.Lerp(
                WaypointParent.transform.GetChild(num).GetComponent<RectTransform>().position,
                WaypointParent.transform.GetChild(num + 1).GetComponent<RectTransform>().position, Time.deltaTime);
            num++;
            //When hand reached to the way point number 8 .
            //That time sound the Alphabet will be played .
            if (num == SoundWaypointNum)
            {
                SoundManager.reference.Play(CurrentAlphabetSound, false,false);
                PatchyAnimationController.ReferenceAnimationController.StartAnimation(
                    PatchyAnimationController.TypeOfAnimation.SingleWordUtterAnimation, CurrentAlphabetSound.length/2);
            }

            //When hand reached to the way point number 8 .
            //That time sound the Alphabet will be played .
            if (num == SoundWaypointNum + 20)
            {
                SoundManager.reference.Play(CurrentAlphabetSound, false,false);
                PatchyAnimationController.ReferenceAnimationController.StartAnimation(
                    PatchyAnimationController.TypeOfAnimation.SingleWordUtterAnimation, CurrentAlphabetSound.length/2);
            }

            yield return new WaitForSeconds(timedelay);
        }

        Invoke("AfterAnimationIsShown", 2f);
    }

    /// <summary>
    ///     Afters the animation is shown.
    ///     Run this method with a delay to avoid jerk ness .
    /// </summary>
    private void AfterAnimationIsShown()
    {
        //Deactivate the hand image and button
        if (HandGameOject != null) HandGameOject.SetActive(false);

        //Activate your turn method
        //Rejected
        /*
        if (YourTurn != null) {
            YourTurn.SetActive (true);
        }
        */

        //Activate the mask image 
        if (PointContainer != null) PointContainer.SetActive(true);

        //Let the player Write
        IsreadyTowrite = true;

        //If plyer is Idle for 10 second 
        InvokeRepeating("FindIdleCondition", 10f, 10f);

        //Make color panel active
        ColorPanel.SetActive(true);

        //Start Coroutine that will write 
        //IEnumerator draw = Draw() ;
        //StartCoroutine (draw);

        //Play the sound to do it yourself
       

        
   
            SoundManager.reference.Play(SoundManager.reference.NowYouDoIt, false,true);

            PatchyAnimationController.ReferenceAnimationController.StartAnimation(
                PatchyAnimationController.TypeOfAnimation.NowYouDoItAnimation,
                SoundManager.reference.NowYouDoIt.length);


        StartCoroutine(_startDraw);
    }

    /// <summary>
    ///     Instantiates the one pixelimage.
    ///     writing element.
    /// </summary>
    /// <param name="position">Position.</param>
    [Obsolete("This method is obsolete", true)]
    private void InstantiateOnePixelimage(Vector2 position)
    {
        var newPosition = new Vector3(position.x, position.y, 0f);
        var newimage = Instantiate(Onepix, newPosition, Quaternion.identity);
        newimage.transform.SetParent(PointContainer.transform);
    }

    /// <summary>
    ///     Mouses  position -------> UIposition.
    /// </summary>
    /// <returns>The position to uiposition.</returns>
    /// <param name="position">Position.</param>
    [Obsolete("This method is obsolete", true)]
    private Vector2 MousePositionToUiposition(Vector2 position)
    {
        return Maincamera.ScreenToWorldPoint(position);
    }

    private IEnumerator DrawUsingLinerender()
    {
        while (IsreadyTowrite)
        {
            //If you press the mouse you can write again
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                var PenGameobject = Instantiate(Pen);
                PenGameobject.transform.SetParent(pencilBox.transform);
                currentPen = PenGameobject;
                //If color button is pressed and new mat is createtd .
                if (UpdatedMaterial != null) currentPen.GetComponent<TrailRenderer>().sharedMaterial = UpdatedMaterial;
                SoundManager.reference.PlayDrawAudio();
                CancelInvoke("RevokeWritePermissionAfter");
                CancelInvoke("FindIdleCondition");
            }

            //Let you draw when mouse is moving
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0))
                currentPen.GetComponent<DrawLine>().LineDraw();

            //Stop Drawing up to end point
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
            {
                
                SoundManager.reference.StopDrawAudio();
                currentPen.GetComponent<DrawLine>().LineDraw();
                Invoke("RevokeWritePermissionAfter", 5f);
            }

            yield return 0;
        }
    }

    [Obsolete("This method is obsolete", true)]
    private IEnumerator Draw()
    {
        //Touch Module For Iphone and Android
        //TODO:
        //Upgrade the method when usinng phones
        Debug.Log("Draw started");
        while (IsreadyTowrite)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                //Debug.Log ("Mouse Button pressed and Touch initiated ");

                //For editor
                if (Application.platform == RuntimePlatform.WebGLPlayer || Application.isEditor)
                    InstantiateOnePixelimage(MousePositionToUiposition(Input.mousePosition));

                //For phone
                if (Application.platform == RuntimePlatform.Android ||
                    Application.platform == RuntimePlatform.IPhonePlayer)
                    InstantiateOnePixelimage(Input.GetTouch(0).position);

                //When First Touch received deactivate your turn
                if (YourTurn.activeSelf) YourTurn.SetActive(false);
                CancelInvoke("RevokeWritePermissionAfter");
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetMouseButton(0))
            {
                //Debug.Log ("Mouse Button is in pressed condition and touched moved ");
                //Draawing will be performed in this section 
                if (Application.platform == RuntimePlatform.WebGLPlayer || Application.isEditor)
                    InstantiateOnePixelimage(MousePositionToUiposition(Input.mousePosition));
                if (Application.platform == RuntimePlatform.Android ||
                    Application.platform == RuntimePlatform.IPhonePlayer)
                    InstantiateOnePixelimage(Input.GetTouch(0).position);
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
            {
                //Debug.Log ("Touch and mouse both are realeased ");
                if (Application.platform == RuntimePlatform.WebGLPlayer || Application.isEditor)
                    InstantiateOnePixelimage(MousePositionToUiposition(Input.mousePosition));
                if (Application.platform == RuntimePlatform.Android ||
                    Application.platform == RuntimePlatform.IPhonePlayer)
                    InstantiateOnePixelimage(Input.GetTouch(0).position);
                Invoke("RevokeWritePermissionAfter", 5f);
            }

            yield return 0;
        }

        Debug.Log("Draw thread is over");
    }

    private void FindIdleCondition()
    {
        SoundManager.reference.Play(SoundManager.reference.TraceOnly, false,false);
        PatchyAnimationController.ReferenceAnimationController.StartAnimation(
            PatchyAnimationController.TypeOfAnimation.TraceTheLetterAnimation,
            1 * SoundManager.reference.TraceOnly.length);
    }

    /// <summary>
    ///     Revokes the write permission after.
    /// </summary>
    private void RevokeWritePermissionAfter()
    {
        //Do not allow to write
        IsreadyTowrite = false;
        //color panel deactivate
        ColorPanel.SetActive(false);
        //Play the sound of the audio of the current letter iff demo has not been shown in the begining
        if (!IsHelpRequired)
        {
            SoundManager.reference.Play(CurrentAlphabetSound, false,false);
            //A single sound track contain the same sound twice.
            //But animation is currently playing once.
            PatchyAnimationController.ReferenceAnimationController.StartAnimation(
                PatchyAnimationController.TypeOfAnimation.SingleWordUtterAnimation, CurrentAlphabetSound.length/2);

            //!.2f is the buffer
            Invoke("PlayNowYouSayIt", CurrentAlphabetSound.length + 1.2f);
        }

        //Reward Audio
        if (!IsHelpRequired)
            //3f is buffer
        {
            Invoke("PlayRewardAudioWithDelay",
                CurrentAlphabetSound.length + SoundManager.reference.NowYouSayit.length + 3f);
        }
        else
        {
            //Directly play the reward audio
            SoundManager.reference.PlayRewardAudio();
            Invoke("ActivateNextLetterWithDelay", SoundManager.reference.GetComponent<AudioSource>().clip.length);
        }

        //Destoy points
        DestroyAllPointerObject();
        //Destroy lines
        Destroy(pencilBox);
    }

    private void PlayRewardAudioWithDelay()
    {
        SoundManager.reference.PlayRewardAudio();
        //Show next letter after reward Audio is complete .
        Invoke("ActivateNextLetterWithDelay", SoundManager.reference.GetComponent<AudioSource>().clip.length);
    }

    private void PlayNowYouSayIt()
    {
       
        //Ask the player to say the sound
        SoundManager.reference.Play(SoundManager.reference.NowYouSayit, false,false);
        PatchyAnimationController.ReferenceAnimationController.StartAnimation(
            PatchyAnimationController.TypeOfAnimation.NowYouDoItAnimation, SoundManager.reference.NowYouDoIt.length);
    }

    /// <summary>
    ///     Starts the animation with sound.
    ///     This method start Animation of hand and play the corresponding sound
    /// </summary>
    private void StartAnimationWithSound()
    {
        SoundManager.reference.Play(SoundManager.reference.TraceTheAlphabet, false,false);
        //First anamation will get stop after  only 1/3 rd of total time of audio to complete .
        PatchyAnimationController.ReferenceAnimationController.StartAnimation(
            PatchyAnimationController.TypeOfAnimation.TraceTheLetterAnimation,
            1 * SoundManager.reference.TraceTheAlphabet.length / 3);

        //First anamation will get stop after only 2/3 rd of total time of audio to complete .
        PatchyAnimationController.ReferenceAnimationController.StartAnimation(
            PatchyAnimationController.TypeOfAnimation.IwillDoItAnimayion,
            2 * SoundManager.reference.TraceTheAlphabet.length / 3);

        //First anamation will get  full time of audio to complete .
        PatchyAnimationController.ReferenceAnimationController.StartAnimation(
            PatchyAnimationController.TypeOfAnimation.Watchit, SoundManager.reference.TraceTheAlphabet.length);

        //Invoke the method when your Audio clip time is over
        Invoke("ShowAnimation", SoundManager.reference.TraceTheAlphabet.length);
    }

    /// <summary>
    ///     Activates the next letter with delay.
    /// </summary>
    private void ActivateNextLetterWithDelay()
    {
        ExerciseGroup.reference.ActivateNextletter();
        CancelInvoke("ActivateNextLetterWithDelay");
    }

    private void DestroyAllPointerObject()
    {
        foreach (Transform child in PointContainer.transform) Destroy(child.gameObject);
    }

    #endregion
}