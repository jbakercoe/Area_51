using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class LevelCell : MonoBehaviour
    {
        #region public Field
        //Level Load button
        public Button Playlevel;
        //delegate that load level
        public delegate void LoadLevelDelegate( string attachedScene);
        //event that load level.
        public static event LoadLevelDelegate LevelLoadedEvent;
        //Scene that will be loaded for this Level
        [Header("Scene That will be Loaded")] public Scene SceneWillLoaded;

        public enum Scene : int
        {
            Level1 = 1,
            Level2 = 2,
            Level3 = 3,
            Level4 = 4,
            Level5 = 5,
            Level6 = 6,
            Level7 = 7,
            Level8 = 8,
            Level9 = 9,
            Level10 = 10,
            Level11 = 11,
            Level12 = 12,
            Level13 = 13,
            Level14 = 14,
            Level15 = 15,
            Level16 = 16


        };
        #endregion
        #region for Unity Message

        public void Awake()
        {
            //load the start button
            Playlevel.onClick.AddListener(() => { OnLoadScene(SceneWillLoaded.ToString()); });

        }

        #endregion

        #region Normal Function

        /// <summary>
        /// Method that fire the event for loading level.
        /// </summary>
        private void OnLoadScene(String scene)
        {
            if (LevelLoadedEvent != null)
            {
                LevelLoadedEvent(scene);
            }
            else
            {
                Debug.Log("No event attched");
            }
        }

        #endregion
    }
}