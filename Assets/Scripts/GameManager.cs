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
        Debug.Log("Spawning new Obstacle at " + Time.timeSinceLevelLoad);
        rand = Random.Range(0, obstacles.Count);
        Instantiate(obstacles[rand], transform.position, Quaternion.identity);
        Debug.Log("Intantiating at index: " + rand);
    }

    public void SpawnNewBulletPattern()
    {
        Debug.Log("Spawning new Bullet Pattern at " + Time.timeSinceLevelLoad);
        rand = Random.Range(0, bulletPatterns.Count - 1);
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
