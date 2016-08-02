using UnityEngine;
using System.Collections;

public class CameraNode : MonoBehaviour {
    public float Zoom = 40f;

    void OnTriggerEnter2D(Collider2D colObj)
    {
        if(colObj.gameObject.transform.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OnRailsCamera>().GotoNode(transform.position, transform.rotation, Zoom);
        }
    }
}
