using UnityEngine;
using System.Collections;

namespace Completed
{
    using System.Collections.Generic;       //Allows us to use Lists. 
    using UnityEngine.UI;                   //Allows us to use UI.

    public class SceneManager : MonoBehaviour
    {
        public float sceneStartDelay = 2f;                      //Time to wait before starting level, in seconds.

        public static SceneManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector]

        private GameObject sceneImage;                          //Image to block out level as levels are being set up, background for levelText.
    
        private int scene = 1;                                  //Current level number, expressed in game as "Day 1".

        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Call the InitGame function to initialize the first level 
            InitScene();
        }

        //This is called each time a scene is loaded.
        void OnLevelWasLoaded(int index)
        {
            //Add one to our level number.
            scene++;
            //Call InitGame to initialize our level.
            InitScene();
        }

        //Initializes the game for each level.
        void InitScene()
        {
            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            doingSetup = true;

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke("HideSceneImage", sceneStartDelay);

        }


        //Hides black image used between levels
        void HideSceneImage()
        {

            //Set doingSetup to false allowing player to move again.
            doingSetup = false;
        }

        //Update is called every frame.
        void Update()
        {

        }

        //GameOver is called when the player reaches 0 food points
        public void SceneOver()
        {
            //Disable this GameManager.
            enabled = false;
        }

    }
}

