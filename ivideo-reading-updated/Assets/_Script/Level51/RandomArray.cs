using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomArray {

    #region Summary
    /*
     * This class holds the functions to return the provided array in random order.
     * If you need a shuffle algorithm for a different type, just copy and paste an existing function 
     * and change the return and parameter types
     */
    #endregion


    /// <summary>
    /// Returns array with contents in a random order
    /// </summary>
    /// <param name="transforms">The array of transforms to be randomized</param>
    /// <returns>Array of same transforms as provided, but in random order</returns>
    public static Transform[] ShuffleArray(Transform[] transforms)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform tmp = transforms[i];
            int r = Random.Range(i, transforms.Length);
            transforms[i] = transforms[r];
            transforms[r] = tmp;
        }
        return transforms;

    }


    /// <summary>
    /// Returns array with contents in a random order
    /// </summary>
    /// <param name="gameObjects">The array of gameObjects to be randomized</param>
    /// <returns>Array of same gameObjects as provided, but in random order</returns>
    public static GameObject[] ShuffleArray(GameObject[] gameObjects)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject tmp = gameObjects[i];
            int r = Random.Range(i, gameObjects.Length);
            gameObjects[i] = gameObjects[r];
            gameObjects[r] = tmp;
        }
        return gameObjects;

    }

}
