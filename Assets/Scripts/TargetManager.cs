using UnityEngine;
using System.Collections;

public class TargetManager : MonoBehaviour {

    public GameObject bulletSpawn;

	void Start()
    {
        StartCoroutine(LaunchTargets());
    }

    IEnumerator LaunchTargets()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(bulletSpawn, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
    }

    public void Finish()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SpawnNewBulletPattern();
        Destroy(this.gameObject);
    }
}
