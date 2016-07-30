using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    Camera cam; //Scene camera
    LineRenderer lineRenderer; //Line renderer
    Vector2 mousePosition; //Mouse position (world space)
    public float playerLength, playerWidth;
    public float playerSpeed = 150f; //Player's speed

    enum State {STATIONARY, MOVING};
    State state;

    void Start()
    {
        //Gets object references
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lineRenderer = GetComponent<LineRenderer>();
        state = State.STATIONARY;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.up, Color.green, 0, false);
        Debug.DrawRay(transform.position, -transform.up, Color.red, 0, false);

        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        ManageInput();
        UpdateLineRenderer();
    }

    void LateUpdate()
    {
        ManageMovement();
    }

    void ManageMovement()
    {
        switch (state)
        {
            case State.STATIONARY:
                //Re-enables line renderer
                if (!lineRenderer.enabled)
                    EnableLineRenderer();
                break;
            case State.MOVING:
                if (lineRenderer.enabled)
                    DisableLineRenderer();

                //Move forward
                transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);
                break;
        }
    }
    
    //Manages player input
    void ManageInput()
    {
        switch (state)
        {
            case State.STATIONARY:
                //Left click - Jump
                if (Input.GetMouseButtonDown(0))
                {
                    //Update state
                    state = State.MOVING;
                    //Get angle
                    transform.eulerAngles = transform.eulerAngles.x < mousePosition.x - transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up));
                }
                break;
            case State.MOVING:
                if (Input.GetMouseButtonDown(0))
                {
                    //Update state
                    state = State.STATIONARY;
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D colObj)
    {
        if(colObj.gameObject.CompareTag("CollisionObject"))
        {
            state = State.STATIONARY;
            
            //Send raycast
            //RaycastHit2D[] rayHits = Physics2D.RaycastAll(transform.position, transform.up, 2.3f);
            RaycastHit2D[] rayHits = Physics2D.CircleCastAll(transform.position, 0.1f, transform.up, 2.3f);
            RaycastHit2D rayHit = rayHits.Length > 1 ? rayHits[1] : rayHits[0];
            //float closestDistance = int.MaxValue;
           /* RaycastHit2D closestHit = rayHits[0];
            Debug.Log("Size of rayHits: " + rayHits.Length);
            */
            //Get closest hit
            /*foreach(RaycastHit2D rayHit in rayHits)
            {
                Debug.Log("Checking rayHit: " + rayHit.transform.gameObject.name);
                Debug.Log("rayHit.distance = " + rayHit.distance);
                Debug.Log("Closest Distance = " + closestDistance);
                Debug.Log("Collider Tag = " + rayHit.collider.gameObject.tag);
                Debug.Log("Collision Point = " + rayHit.point);
                if(rayHit.collider.gameObject.CompareTag("CollisionObject") && (closestDistance > rayHit.distance))
                {
                    closestHit = rayHit;
                    closestDistance = rayHit.distance;
                    Debug.Log("New Closest Hit: " + closestHit.transform.gameObject.name);
                }
                Debug.Log("\n");
            }
            Debug.Log("Final Closest Hit: " + closestHit.transform.gameObject.name);
            */

            //Check that plane is close to plater
            /*Debug.Log(Time.time + " - Distance: " + closestHit.distance);
            Debug.Log(Time.time + " - Normal: " + closestHit.normal);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, closestHit.normal);
            transform.position = closestHit.point;
            transform.Translate(Vector2.up * 1.4f);*/
            Debug.Log("\nNEW HIT!");
            Debug.Log(Time.time + " - Checking rayHit: " + rayHit.transform.gameObject.name);
            Debug.Log(Time.time + " - rayHit.distance = " + rayHit.distance);
            Debug.Log(Time.time + " - Collider Tag = " + rayHit.collider.gameObject.tag);
            Debug.Log(Time.time + " - Collision Point = " + rayHit.point);
            Debug.Log(Time.time + " - Normal: " + rayHit.normal);
            Debug.Log(Time.time + " - Normal (Angle): " + Quaternion.FromToRotation(Vector3.up, rayHit.normal));
            Debug.Log(Time.time + " - Distance: " + rayHit.distance);
            if(rayHit.distance > 0)
            {
                Debug.Log("GOOD COLLISION!");
            }
            else
            {
                Debug.Log("BAD COLLISION!");
            }
            
            transform.rotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);

            RaycastHit2D[] backHits = Physics2D.RaycastAll(transform.position, -transform.up, 2f);
            RaycastHit2D closestBackHit = backHits[0];
            float closestBackDistance = int.MaxValue;
            foreach (RaycastHit2D backHit in backHits)
            {
                if (backHit.collider.gameObject.CompareTag("CollisionObject") && (closestBackDistance > backHit.distance))
                {
                    closestBackHit = backHit;
                }
            }

            Debug.Log("Player Length / 2: " + playerLength / 2 + " || Closest Distance: " + closestBackHit.distance + " || Name: " + closestBackHit.transform.gameObject.name);
            float moveDistance = (playerLength / 2) - closestBackHit.distance;
            Debug.Log("Move: " + moveDistance);
            Debug.Log(transform.forward * moveDistance);
            //transform.Translate(Vector2.up * moveDistance);
        }
    }

    //Updates the line renderer
    void UpdateLineRenderer()
    {
        lineRenderer.SetPositions(new Vector3[] {transform.position, mousePosition});
    }

    //Disables the line renderer
    void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }

    //Enables the line renderer
    void EnableLineRenderer()
    {
        lineRenderer.enabled = true;
    }
}
