using UnityEngine;
using System.Collections;

public class TargetSpawn : MonoBehaviour {

    public GameObject bullet;
    public int segments = 24;

	void Start () {
        Vector2 dir = transform.up * 100f;
        Quaternion rot = Quaternion.Euler(new Vector3(0,0,0));
        float centralAngle = 360 / segments;

	    for(int i = 1; i <= segments; i++)
        {
            rot = Quaternion.Euler(new Vector3(0, 0, (centralAngle * i)));
            GameObject tempGameObject = (GameObject)Instantiate(bullet, transform.position + (rot * dir), Quaternion.identity);
            tempGameObject.transform.parent = this.transform;
        }
	}

    void Update ()
    {
        int children = 0;
        foreach (Transform child in transform)
        {
            children++;
        }
        if(children == 0)
        {
            GameObject.FindGameObjectWithTag("TargetManager").GetComponent<TargetManager>().Finish();
            Destroy(this.gameObject);
        }
    }
}
