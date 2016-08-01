using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour {

    private GameObject spawnPoint;
    float speed, ang;
    private bool spawned;

	void Start () {
        spawned = false;
        spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
    }

	void Update () {
        //Get rotation
        ang = Vector2.Angle(spawnPoint.transform.position - transform.position, Vector2.up);
        transform.eulerAngles = transform.eulerAngles.x < spawnPoint.transform.position.x - transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(spawnPoint.transform.position - transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(spawnPoint.transform.position - transform.position, Vector2.up));

        //Get speed
        speed = Vector2.Distance(transform.position, spawnPoint.transform.position) * 2;

        //Transform
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        if(speed < 20 && !spawned)
        {
            spawnPoint.GetComponent<PlayerSpawn>().SpawnPlayer();
            spawned = true;
        }

        if(speed < 1)
        {
            Destroy(gameObject);
        }
	}
}
