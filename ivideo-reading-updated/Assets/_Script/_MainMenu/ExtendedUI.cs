using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class ExtendedUI : MonoBehaviour
    {
        #region public variable
        /// <summary>
        /// The speed in which alpha fade in or out.
        /// </summary>
        [Header("Alpha Fading Speed")]
        public float FadingSpeed;
        /// <summary>
        /// This panel stop user to interact once user press the main menu button.
        /// </summary>
        [Space(5f)] [Header("Panel That will not allow to interact")]
        public GameObject InterActionPanel;
        /// <summary>
        /// Pauseed.
        /// </summary>
        [Space(5f)] [Header("Is the Game paused")]
        public bool Ispaused = false;
        #endregion

        #region private Variable
        /// <summary>
        /// Audio Listener for sound.
        /// </summary>
        private AudioListener _mainAudioListener;
        #endregion

        #region Unity Function

        /// <summary>
        /// On enable .
        /// </summary>
        private void OnEnable()
        {
            //Attach an event.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

      
        /// <summary>
        /// When a new level is Loaded.
        /// </summary>
        /// <param name="level"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
            //if the level that is loaded is mainmenu
            if (scene.buildIndex == 0)
            {
               //Future uses.
            }
        }

        #endregion

        #region Normal Function

        /// <summary>
        /// Pause The Application.
        /// </summary>
        public void Pause()
        {
            //set up the pause bool.
            Ispaused = !Ispaused;
            //search for audio listener.
            _mainAudioListener = Camera.main.GetComponent<AudioListener>();
            //If audio listener is found.
            if (_mainAudioListener != null) _mainAudioListener.enabled = !Ispaused;
            //Set up the time scale;
            Time.timeScale = Ispaused ? 0.0F : 1.0F;
        }

        /// <summary>
        /// Unload the Application.
        /// </summary>
        private void UnloadScene()
        {
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Go to the main menu.
        /// </summary>
        public void GotoHome()
        {
            //hok up.
            var fadingIenumerator = Fade(1);
            //start coroutine.
            StartCoroutine(fadingIenumerator);
        }

        /// <summary>
        /// Fade in or out the panel.
        /// </summary>
        public IEnumerator Fade(int within)
        {
            if (within == 1)
            {
                //Activate panel to stop interaction.
                InterActionPanel.SetActive(true);
                //panel color.
                var panelColorAlpha = InterActionPanel.GetComponent<Image>().color.a;
                //While loop
                while (panelColorAlpha <= 1f)
                {
                    //increase alpha over time 
                    panelColorAlpha += Time.deltaTime * FadingSpeed;
                    //genarate a same color with different alpha
                    var updatedColor = InterActionPanel.GetComponent<Image>().color;
                    //update the alpha
                    updatedColor.a = panelColorAlpha;
                    //Set the color
                    InterActionPanel.GetComponent<Image>().color = updatedColor;
                    //Ieterate over 
                    yield return  null;
                }

                //Unload the scene.
                UnloadScene();

            }


            if (within == -1)
            {
                //Activate panel to stop interaction.
                InterActionPanel.SetActive(true);
                //panel color.
                var panelColorAlpha = InterActionPanel.GetComponent<Image>().color.a;
                //While loop
                while (panelColorAlpha >= 1f)
                {
                    //increase alpha over time 
                    panelColorAlpha -= Time.deltaTime * FadingSpeed;
                    //genarate a same color with different alpha
                    var updatedColor = InterActionPanel.GetComponent<Image>().color;
                    //update the alpha
                    updatedColor.a = panelColorAlpha;
                    //Set the color
                    InterActionPanel.GetComponent<Image>().color = updatedColor;
                    //Ieterate over 
                    yield return null;
                }

                //Deactivate the panel.
                InterActionPanel.SetActive(false);

            }



        }
        

        #endregion
    }
}