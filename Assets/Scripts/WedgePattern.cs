using UnityEngine;
using System.Collections;

public class WedgePattern : MonoBehaviour {

    public float speed = 10f;

	void Update()
    {
        transform.Translate(-Vector2.up * Time.deltaTime * speed);

        if(transform.position.y <= -100)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SpawnNewObstacle();
            Destroy(this.gameObject);
        }
    }
}
