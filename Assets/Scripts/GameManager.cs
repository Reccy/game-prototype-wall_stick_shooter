using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public enum GameMode { PLATFORMER, BULLET_HELL };
    public GameMode gameMode;
    public List<GameObject> obstacles;
    public List<GameObject> bulletPatterns;
    int rand;

    void Start()
    {
        StartCoroutine(BeginStartCountdown());
    }

    IEnumerator BeginStartCountdown()
    {
        yield return new WaitForSeconds(3);
        SpawnNewObstacle();
        yield return new WaitForSeconds(3);
        SpawnNewBulletPattern();
    }

    public void SpawnNewObstacle()
    {
        rand = Random.Range(0, obstacles.Count);
        Instantiate(obstacles[rand], transform.position, Quaternion.identity);
    }

    public void SpawnNewBulletPattern()
    {
        rand = Random.Range(0, bulletPatterns.Count);
        Instantiate(bulletPatterns[rand], transform.position, Quaternion.identity);
    }

    //Manage global input
    void Update()
    {
        //Resets level
        if(Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
