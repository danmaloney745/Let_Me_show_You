using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour {

    public Button btnCreate;
    //public Button btnShow;

    public int sceneToLoad;

    public void loadScene()
    {
        if (btnCreate.name == "Create")
        {
            sceneToLoad = 1;
        }

        if(btnCreate.name == "Show")
        {
            sceneToLoad = 2;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
