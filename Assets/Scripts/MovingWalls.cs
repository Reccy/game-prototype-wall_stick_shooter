using UnityEngine;
using System.Collections;

public class MovingWalls : MonoBehaviour {

    bool begin = false;
    float startTime;
    public float intensity = 10f;
    public float speed = 2f;

	void Start () {
        StartCoroutine(WaitCoroutine());
	}
	
    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(2);
        begin = true;
        startTime = Time.timeSinceLevelLoad;
        StartCoroutine(EndCoroutine());
    }

    IEnumerator EndCoroutine()
    {
        yield return new WaitForSeconds(5);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SpawnNewObstacle();
        Destroy(this.gameObject);
    }

	void Update () {
	    if(begin)
        {
            transform.position = new Vector2(Mathf.Sin((Time.timeSinceLevelLoad - startTime) * speed) * intensity, transform.position.y);
            Debug.Log("X pos: " + transform.position.x + " || " + Time.timeSinceLevelLoad);
        }
	}
}
