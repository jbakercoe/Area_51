using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour {

    #region Summary 

    /*
     * Detects clicks, controls animation
     */

    #endregion
        
    const float speed = .5f;

    Vector3 startPosition;

    // delegate function to broadcast a letter click
    // Makes it so this script can work with any step, any level
    public delegate void OnLetterClick(GameObject letter);
    public static event OnLetterClick NotifyLetterObservers;

    void Start()
    {
        // set startPosition = to position
        startPosition = transform.position;
    }

    /// <summary>
    /// Choose a letter to move it into word position
    /// </summary>
    /// <param name="location">the position to be moved to</param>
    public void ChooseLetter(Vector3 location)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(MoveLetter(location));
    }

    /// <summary>
    /// Moves the letter to supplied location
    /// </summary>
    /// <param name="location"></param>
    IEnumerator MoveLetter(Vector3 location, float moveSpeed = speed)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.position, location);

        // moves letter to within .01 of target location
        while (Mathf.Abs(Vector3.Distance(transform.position, location)) > .01f)
        {
            float distCovered = (Time.time - startTime) * moveSpeed;

            float fracJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, location, fracJourney);

            yield return null;
        }
    }

    public void ResetLocation()
    {
        transform.position = startPosition;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    /// <summary>
    /// This works. IDK why other one doesn't
    /// </summary>
    void OnMouseDown()
    {
        NotifyLetterObservers(this.gameObject);
    }

}
