using UnityEngine;
using System.Collections;

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

                if(forwardCollisionHit)
                {
                    if (forwardCollisionHit.transform.gameObject.tag != "Player")
                    {
                        Debug.Log("Collision Object Detected!");
                        state = State.STATIONARY;
                        transform.Translate(Vector2.up * forwardCollisionHit.distance);
                    }
                }
                Debug.DrawRay(probeOrigin.transform.position, transform.up * probeDistance, Color.yellow, 1 * Time.deltaTime, false);
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
        //Set state
        state = State.STATIONARY;

        //Get collision point
        RaycastHit2D[] rayHits = Physics2D.CircleCastAll(transform.position, 0.1f, transform.up, 2.3f);
        RaycastHit2D rayHit = rayHits.Length > 1 ? rayHits[1] : rayHits[0];

        //Debug to check if the collision is good or bad
        if (rayHit.distance > 0)
        {
            Debug.Log("GOOD COLLISION!");
        }
        else
        {
            Debug.Log("BAD COLLISION!");
        }

        Debug.Log("\n\n");

        //Set rotation
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);

        //Set position
        RaycastHit2D closestBackHit = GetClosestRaycastHit2D("CollisionObject", transform.position, -transform.up, 2f);
        float moveDistance = (playerLength / 2) - closestBackHit.distance;
        transform.Translate(Vector2.up * moveDistance);
    }

    //Gets closest Raycast2D
    RaycastHit2D GetClosestRaycastHit2D(string compareTag, Vector2 origin, Vector2 direction, float distance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask, minDepth, maxDepth);

        if(hits.Length > 0)
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
                    }
                }
                else
                {
                    if (closestBackDistance > hit.distance)
                    {
                        closestHit = hit;
                    }
                }
            }
            return closestHit;
        }
        return new RaycastHit2D();
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
