using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class WordAnimation.
/// <para>This script should be attached to the parent game object .
/// The child gameObject will be the letter .Consider the word "CAT" .
/// So there will be one Parent game object that will contain the script and three child
/// game object that will have the text as a component.The child game object should contain rigid body 2d and box collider 2D .  </para>
/// </summary>
public class WordAnimation : MonoBehaviour {

    private Action _animationComplete;
    /// <summary>
    /// Is the animation running ?
    /// </summary>
    public bool IsAnimationRunning = false;
    /// <summary>
    /// The animation for word .
    /// Static reference .
    /// </summary>
    public static WordAnimation AnimationForWord;

    /// <summary>
    /// letters .
    /// </summary>
    public GameObject[] Letters;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        if(AnimationForWord==null)
        AnimationForWord = this;
        Debug.Log(Screen.height);
    }

    /// <summary>
    /// Activates the rigid body.
    /// </summary>
    private void ActivateRigidBody()
    {
        Debug.Log("Child activation requested");
        //number of child.
        var chlidcount = gameObject.transform.childCount;
        Debug.Log("Child have " + chlidcount);
        //Activate the rigid body .
        for (var i = 0; i < chlidcount; i++)
        {
            //rigid body access .
            var rb = gameObject.transform.GetChild(i).GetComponent<Rigidbody2D>();

            //if rigid body is null .

            if (rb == null)
            {
                var gobject = gameObject.transform.GetChild(i).gameObject;
                //Add rigid body
                gobject.AddComponent<Rigidbody2D>();
                //Debug it.
                Debug.Log("Rigid body attached to game object  " + gobject.name);

            }
            //There is some rigid body.
            else
            {
                Debug.Log("Rigid body found on Gameobject with name " + rb.gameObject.name);
                //if rigid body's are not in dynamic condition.
                if (rb.bodyType != RigidbodyType2D.Dynamic)
                {
                    //change the 
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    Debug.Log("Status of rigid body has been changed.");
                }
                else
                {
                    continue;
                }
            }
        }
    }


    /// <summary>
    /// Throws the letters in a random direction.
    /// Tweak the vectors and powers to achieve more beautiful results .
    /// </summary>
    private IEnumerator ThrowLetters(float afterSomeTime)
    {
        //Before throwing the letters out of the screen.
        //wait for few times.
        yield return new WaitForSecondsRealtime(afterSomeTime);
        var power = 2000f;
        //number of child.
        var clidcount = gameObject.transform.childCount;
        //Access the rigid body .
        for (var i = 0; i < clidcount; i++)
        {
            //rigid body access .
            var rb = gameObject.transform.GetChild(i).GetComponent<Rigidbody2D>();

            if (i % 3 == 0)
            {
                rb.AddForce(new Vector2(-20,10) * power);
            }
            if (i % 3 == 1)
            {
                rb.AddForce(new Vector2(20, 10) * power);
            }
            if (i % 3 == 2)
            {
                rb.AddForce(new Vector2(20,-10) * power);
            }

           
        }
   
    }

    /// <summary>
    /// Explodes this instance.
    /// Words under the letters will explode .
    /// </summary>
    public virtual void Explode(Action onAnimationComplete)
    {
        //Hook the Action
        _animationComplete = () => onAnimationComplete();

        IsAnimationRunning = true;
        //Activate RB
        ActivateRigidBody();
        //Fire
        var letterthrower = ThrowLetters(afterSomeTime: 0.2f);
        //start it .
        StartCoroutine(letterthrower);
        //Invoke Destroy.
        Invoke("Destroy",2f);

    }

    /// <summary>
    /// Resets the letter position in word.
    /// </summary>
    private void Destroy()
    {
        //How many child ?
        var chlidcount = gameObject.transform.childCount;
        //Reset it
        for (var i = 0; i < chlidcount; i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
            
        }
        //genarates the childs
        for (var i = 0; i < chlidcount; i++)
        {
            var go = Instantiate(Letters[i]);
            go.transform.SetParent(this.gameObject.transform);
            go.SetActive(false);


        }
        IsAnimationRunning = false;

        //Raise the Animation complete event .
        _animationComplete.Invoke();

    }
}
