using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnRailsCamera : MonoBehaviour {

    Vector3 newTransform;
    Quaternion newRotation;
    float newZoom;

    [SerializeField]
    [Range(0f,10f)]
    public float lerpSpeed;

    void Awake()
    {
        newTransform = transform.position;
        newRotation = transform.rotation;
        newZoom = GetComponent<Camera>().fieldOfView;
    }

    void Start()
    {
        transform.position = new Vector3(500, 500, -10);
    }

    public void GotoNode(Vector3 trans, Quaternion rot, float z)
    {
        Debug.Log("UPDATE");
        newTransform = trans;
        newRotation = rot;
        newZoom = z;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, newTransform, 5 * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, 5 * Time.deltaTime);
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, newZoom, 5 * Time.deltaTime);
        GetComponent<CameraShake>().UpdatePos();
    }
}