using UnityEngine;
using System.Collections;

public class Flyer : MonoBehaviour {

    public bool begin = false;
    public float speed = 10f;

	void Update () {
	    if(begin)
        {
            transform.Translate(Vector2.up * Time.deltaTime * speed);
        }
	}
}
