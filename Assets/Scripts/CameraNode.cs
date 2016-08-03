using UnityEngine;
using System.Collections;

public class CameraNode : MonoBehaviour {

    private Camera thisCam;
    public bool originalNode = false;
    private bool containsSpawn = false;
    private bool mainCamLock = false;

    void Start()
    {
        thisCam = GetComponent<Camera>();
    }

    void OnTriggerEnter2D(Collider2D colObj)
    {
        if(colObj.gameObject.transform.CompareTag("Player"))
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OnRailsCamera>().SetNormal();
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OnRailsCamera>().GotoNode(transform.position, transform.rotation, thisCam.orthographicSize);
        }
        
        if(colObj.gameObject.transform.CompareTag("PlayerSpawn"))
        {
            containsSpawn = true;
        }
    }
    
    void OnTriggerStay2D(Collider2D colObj)
    {
        if (colObj.gameObject.transform.CompareTag("MainCamera"))
        {
            if (containsSpawn && !mainCamLock)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OnRailsCamera>().SetNormal();
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OnRailsCamera>().GotoNode(transform.position, transform.rotation, thisCam.orthographicSize);
                mainCamLock = true;
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D colObj)
    {
        if (colObj.gameObject.transform.CompareTag("MainCamera"))
        {
            if (containsSpawn)
            {
                mainCamLock = false;
            }
        }
    }
}
