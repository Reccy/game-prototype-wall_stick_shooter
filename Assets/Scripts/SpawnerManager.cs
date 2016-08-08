using UnityEngine;
using System.Collections;

public class SpawnerManager : MonoBehaviour {

    public GameObject ghostEnemy;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 3; i++ )
        {
            Instantiate(ghostEnemy, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
            yield return new WaitForSeconds(4f);
        }
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SpawnNewBulletPattern();
        Destroy(this.gameObject);
    }
}
