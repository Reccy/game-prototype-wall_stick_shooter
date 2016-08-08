using UnityEngine;
using System.Collections;

public class SpinBullet : MonoBehaviour {

    public float speed = 1000f;
    public float moveSpeed = 10f;
    private Vector3 originTransform;
    private Quaternion rotation;

    void Start ()
    {
        originTransform = transform.parent.position;
        Debug.Log(originTransform);
        rotation = originTransform.x > transform.position.x ? Quaternion.Euler(0,0,360 - Vector2.Angle(originTransform - transform.position, Vector2.up)) : Quaternion.Euler(0,0,Vector2.Angle(originTransform - transform.position, Vector2.up));
    }

	void Update () {
        transform.Translate(rotation * (Vector2.up * Time.deltaTime * moveSpeed), Space.World);
        transform.Rotate(Vector3.forward * Time.deltaTime * speed);

        float distance = Vector2.Distance(transform.position, originTransform);
        if(distance < 0.3f)
        {
            Destroy(this.gameObject);
        }
	}
}
