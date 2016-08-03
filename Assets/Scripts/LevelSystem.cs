using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSystem : MonoBehaviour {

    //Basic debug level system. Obviously real game will have a better system ;)
    void Update()
    {
        for(int i = 0; i < 10; i++)
        {
            if(Input.GetKeyDown(i.ToString()))
            {
                if(i < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(i);
                    Debug.Log("Loading Scene: " + i);
                }
                else
                {
                    Debug.Log("Scene " + i + " not in this build!");
                }
            }
        }
    }
}
