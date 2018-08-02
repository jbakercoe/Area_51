using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    /// <summary>
    /// Main scene user interface controller.
    /// This script Controll the UI of only the main Scene .
    /// </summary>
    public class MainSceneUiController : MonoBehaviour
    {
        #region for public field

        public GameObject NeedToplayWithSoundGameObject;
        public GameObject ClicktoStartGameObject;
        public GameObject LevelScrollViewGameObject;
        public GameObject ProgressBar;
        public GameObject ProgressbarFillerGameObject;
        public GameObject MainMenu; // Main menu contains all levels .
        public Text ProgressBarText;
        //Buttons for levels.
        public Button MountainLevel;
        public Button DesertLevel;
        public Button SwampyLevel;
        public Button VolcanoLevel;
        //Levels for Buttons.
        public GameObject MoutainLevelPanel;
        public GameObject DesertLevelPanel;
        public GameObject SwampyLevelPanel;
        public GameObject VolconoLevelPanel;
        //Back button
        public GameObject BackButton;

        #endregion

        #region privaate field

        //This value is calculated with respect to (filler bar empty  -  full position ) / 100 
        private readonly float fillerbarUnit = 4.44f;

        #endregion

        #region for unity function

        private void Awake()
        {
            //lets call init
            InitMainmenu();
            //Click to start Button action
            ClicktoStartGameObject.GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    ClicktoStartGameObject.SetActive(false);
                    NeedToplayWithSoundGameObject.SetActive(false);
                    //LevelScrollViewGameObject.SetActive(true); // no more scrolling.
                    MainMenu.SetActive(true);
                    //Bind Buttons with actions.
                    MountainLevel.onClick.AddListener(() =>
                    {
                        
                        MoutainLevelPanel.SetActive(true);
                        MainMenu.SetActive(false);
                        BackButton.SetActive(true);
                    });
                    DesertLevel.onClick.AddListener(() =>
                    {   DesertLevelPanel.SetActive(true);
                        MainMenu.SetActive(false);
                        BackButton.SetActive(true);
                    });
                    SwampyLevel.onClick.AddListener(() =>
                    {
                        SwampyLevelPanel.SetActive(true);
                        MainMenu.SetActive(false);
                        BackButton.SetActive(true);
                    });
                    VolcanoLevel.onClick.AddListener(() =>
                    {
                        VolconoLevelPanel.SetActive(true);
                        MainMenu.SetActive(false);
                        BackButton.SetActive(true);
                    });
                });
            //Add back button action
            BackButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                MainMenu.SetActive(true);
                MoutainLevelPanel.SetActive(false);
                DesertLevelPanel.SetActive(false);
                SwampyLevelPanel.SetActive(false);
                VolconoLevelPanel.SetActive(false);
                BackButton.SetActive(false);

            });

            //Latch to the levelload event
            LevelCell.LevelLoadedEvent += OnSceneLoaded;
            
        }

        #endregion

        #region Normal Function

        /// <summary>
        /// Init all Main menu properties at the begining.
        /// </summary>
        private void InitMainmenu()
        {
            //click to start button should be active
            if (!ClicktoStartGameObject.activeInHierarchy)
            {
                ClicktoStartGameObject.SetActive(true);
            }

            //need to play with sound should be active
            if (!NeedToplayWithSoundGameObject.activeInHierarchy)
            {
                NeedToplayWithSoundGameObject.SetActive(true);
            }

            //Level scroll view should not be active
            if (LevelScrollViewGameObject.activeInHierarchy)
            {
                LevelScrollViewGameObject.SetActive(false);
            }

            //Make progress bar deactivated
            if (ProgressBar.activeInHierarchy)
            {
                ProgressBar.SetActive(false);
            }
        }

        private void OnSceneLoaded(string sceene)
        {
            //Level scroll view should  be deactivated
            if (LevelScrollViewGameObject.activeInHierarchy)
            {
                LevelScrollViewGameObject.SetActive(false);
            }

            //progress bar should be activated
            if (!ProgressBar.activeInHierarchy)
            {
                ProgressBar.SetActive(true);
            }

            IEnumerator loadsceneIenumer = LoadSceneeEnumerator(sceene);
            StartCoroutine(loadsceneIenumer);
        }

        private IEnumerator LoadSceneeEnumerator(string sceene)
        {
            
            //controll the loading bar 
            float progressOffsetVector2X =  ProgressbarFillerGameObject.GetComponent<RectTransform>().offsetMax.x;
            float progressOffsetVector2Y =  ProgressbarFillerGameObject.GetComponent<RectTransform>().offsetMax.y;
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceene);
            while (!loadScene.isDone)
            {
                //Debug.Log("loadScene.progress" + loadScene.progress);
               //Debug.Log("loadScene.progress in percentage-->" + (int)(loadScene.progress* 100));
               //Debug.Log("increment in bar "+ ((int)(loadScene.progress * 100)*fillerbarUnit));
                ProgressBarText.text = ((int) (loadScene.progress * 100)).ToString() + "%";

                ProgressbarFillerGameObject.GetComponent<RectTransform>().offsetMax = new Vector2(
                    (progressOffsetVector2X + ((int)(loadScene.progress * 100) * fillerbarUnit)), progressOffsetVector2Y);
                yield return null;
            }
        }

        #endregion
    }
}