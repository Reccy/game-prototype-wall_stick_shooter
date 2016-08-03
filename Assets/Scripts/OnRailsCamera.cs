using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnRailsCamera : MonoBehaviour {

    Vector3 newTransform;
    Quaternion newRotation;
    float newZoom;
    CameraNode[] camNodes;
    enum CameraMode { NORMAL, SOUL, WAITING };
    private CameraMode camMode = CameraMode.NORMAL;
    Vector2 mousePosition;
    public float maxOffset = 2;
    float xOffset, yOffset;

    [SerializeField]
    private GameObject playerSoul;

    [SerializeField]
    [Range(0f,10f)]
    public float lerpSpeed;

    void Awake()
    {
        camNodes = GameObject.FindGameObjectWithTag("CameraNodes").GetComponentsInChildren<CameraNode>();

        //Set camera initial positions
        newTransform = camNodes[0].transform.position;
        newRotation = camNodes[0].transform.rotation;
        newZoom = camNodes[0].GetComponent<Camera>().orthographicSize;
        
        transform.position = newTransform;
        transform.rotation = newRotation;
        GetComponent<Camera>().orthographicSize = newZoom;
    }

    //Update lerp paramaters
    public void GotoNode(Vector3 trans, Quaternion rot, float z)
    {
        newTransform = trans;
        newRotation = rot;
        newZoom = z;
    }

    //Sets camera to follow player soul
    public void SetSoul(GameObject soulObj)
    {
        playerSoul = soulObj;
        newZoom = 40;
        camMode = CameraMode.SOUL;
    }

    //Resets camera
    public void SetNormal()
    {
        camMode = CameraMode.NORMAL;
    }

    //Update camera target paramaters
    void Update()
    {
        if(camMode == CameraMode.NORMAL)
        {
            mousePosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            xOffset = Mathf.Clamp(mousePosition.x - transform.position.x, -40, 40);
            xOffset = (xOffset / 40) * maxOffset;

            yOffset = Mathf.Clamp(mousePosition.y - transform.position.y, -25, 25);
            yOffset = (yOffset / 25) * maxOffset;

            transform.position = Vector3.Lerp(transform.position, new Vector3(newTransform.x + xOffset, newTransform.y + yOffset, newTransform.z) , 5 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, 5 * Time.deltaTime);
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, newZoom, 5 * Time.deltaTime);
        }
        else if(camMode == CameraMode.SOUL)
        {
            newTransform = new Vector3(playerSoul.transform.position.x, playerSoul.transform.position.y, transform.position.z);
            newRotation = Quaternion.EulerAngles(0, 0, 0);

            transform.position = Vector3.Lerp(transform.position, newTransform, 10 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, 10 * Time.deltaTime);
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, newZoom, 10 * Time.deltaTime);

        }
        GetComponent<CameraShake>().UpdatePos();
    }
}