using UnityEngine;
using System.Collections;

public class GhostEnemy : MonoBehaviour {

    CircleCollider2D cCol;
    SpriteRenderer sRend;

    void Awake()
    {
        cCol = GetComponent<CircleCollider2D>();
        sRend = GetComponent<SpriteRenderer>();
        cCol.enabled = false;
    }

	void Start () {
        StartCoroutine(TurnIntoEnemy());
	}
	
	IEnumerator TurnIntoEnemy()
    {
        yield return new WaitForSeconds(2);
        sRend.color = Color.red;
        cCol.enabled = true;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
