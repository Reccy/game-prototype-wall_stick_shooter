using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

    Camera cam; //Scene camera
    LineRenderer lineRenderer; //Line renderer
    Vector2 mousePosition; //Mouse position (world space)
    public GameObject probe, probeOrigin; //Probe to check forward collisions
    public float playerLength, playerWidth; //Player dimensions
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

                //Moves probe forward
                probe.transform.position = probeOrigin.transform.position;
                probe.transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);

                //Distance between probes (Movement distance this frame)
                float probeDistance = Mathf.Abs(Vector2.Distance(probeOrigin.transform.position, probe.transform.position));

                //Check if player will collide with object
                RaycastHit2D forwardCollisionHit = GetClosestRaycastHit2D("CollisionObject", probeOrigin.transform.position, transform.up, probeDistance);

                //If player collides with object, correct their position
                if(forwardCollisionHit)
                {
                    if (forwardCollisionHit.transform.gameObject.tag != "Player")
                    {
                        state = State.STATIONARY;
                        transform.Translate(Vector2.up * forwardCollisionHit.distance);
                    }
                }
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
            StopCharacterMovement();
        }
    }

    void StopCharacterMovement()
    {
        //Get collision point
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(transform.position, transform.up, 2.4f);
        RaycastHit2D rayHit = SortRaycastHit2D("CollisionObject", rayHits);

        //Debug to check if the collision failed or succeeded
        if (rayHit.distance <= 0)
        {
            //Failed Collision - Needs extra handling
            Debug.Log("BAD COLLISION! Calling RaycastCircle();");

            //Get new closest point
            rayHit = RaycastCircle("CollisionObject", transform.position, 360, 2.4f);
        }

        //Set state
        state = State.STATIONARY;

        //Set rotation
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);

        //Set position
        RaycastHit2D closestBackHit = GetClosestRaycastHit2D("CollisionObject", transform.position, -transform.up, 2f);
        float moveDistance = (playerLength / 2) - closestBackHit.distance;
        transform.Translate(Vector2.up * moveDistance);
    }

    //Sorts Raycast2D array to get closest Raycast2D
    RaycastHit2D SortRaycastHit2D(string compareTag, RaycastHit2D[] hits)
    {
        if (hits.Length > 0)
        {
            RaycastHit2D closestHit = hits[0];
            float closestBackDistance = int.MaxValue;
            foreach (RaycastHit2D hit in hits)
            {
                if (compareTag != null)
                {
                    if (hit.collider.gameObject.CompareTag(compareTag) && (closestBackDistance > hit.distance))
                    {
                        closestHit = hit;
                        closestBackDistance = hit.distance;
                    }
                }
                else
                {
                    if (closestBackDistance > hit.distance)
                    {
                        closestHit = hit;
                        closestBackDistance = hit.distance;
                    }
                }
            }
            return closestHit;
        }
        return new RaycastHit2D();
    }

    //Gets 360 Raycast
    RaycastHit2D RaycastCircle(string compareTag, Vector2 origin, int segments = 360, float radius = Mathf.Infinity)
    {
        List<RaycastHit2D> rayHitsList = new List<RaycastHit2D>();

        for(int i = 0; i < segments; i++)
        {
            float angle = (360 / segments) * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
            RaycastHit2D rayHit = GetClosestRaycastHit2D(compareTag, origin, dir, radius);
            if(rayHit.transform.gameObject.CompareTag(compareTag))
            {
                rayHitsList.Add(rayHit);
            }
            
            //Pretty Rainbow Debug Method! :D
            /*
            if(i == 0)
            {
                Debug.DrawRay(origin, dir * 100, Color.HSVToRGB(0, 1, 1), 10, false);
            }
            else
            {
                Debug.DrawRay(origin, dir * 100, Color.HSVToRGB((float)i / (float)segments, 1, 1), 10, false);
            }
            */
        }

        RaycastHit2D[] rayHitsArray = rayHitsList.ToArray();
        return SortRaycastHit2D(compareTag, rayHitsArray);
    }

    //Gets closest Raycast2D
    RaycastHit2D GetClosestRaycastHit2D(string compareTag, Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask, minDepth, maxDepth);
        return SortRaycastHit2D(compareTag, hits);
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
