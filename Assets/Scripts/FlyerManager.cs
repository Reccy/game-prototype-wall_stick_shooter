using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyerManager : MonoBehaviour {

    List<GameObject> flyers;

	void Start () {
        flyers = new List<GameObject>();

        foreach (Transform child in transform)
        {
            Debug.Log(child.gameObject.name);
            flyers.Add(child.gameObject);
        }

        StartCoroutine(LaunchFlyers());
	}
	
	IEnumerator LaunchFlyers()
    {
        yield return new WaitForSeconds(3f);
        foreach (GameObject flyer in flyers)
        {
            flyer.GetComponent<Flyer>().begin = true;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1.5f);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SpawnNewBulletPattern();
    }
}
