using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

    bool canJump; //If the player is able to jump
    public GameObject probe, probeOrigin; //Probe to check forward collisions
    private GameObject audioManager; //Audio manager
    public float playerLength, playerWidth; //Player dimensions
    public float playerSpeed = 150f; //Player's speed
    public List<GameObject> jumpVectors; //Jump positions
    private int jumpVectorIndex; //Current index
    public float jumpDelay = 0; //Delay per jump
    ParticleSystem particleSys; //Particle system

    public enum State { STATIONARY, MOVING };
    public State state;

    void Awake()
    {
        state = State.STATIONARY;
    }

    void Start()
    {
        //Gets object references
        StartCoroutine("Jump");
        canJump = false;
        particleSys = transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
    }

    void Update()
    {
        ManageInput();
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
                //Do nothing
                break;
            case State.MOVING:
                //Stop jumping
                canJump = false;

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
                if (forwardCollisionHit)
                {
                    if (forwardCollisionHit.transform.gameObject.CompareTag("CollisionObject"))
                    {
                        state = State.STATIONARY;
                        transform.Translate(Vector2.up * forwardCollisionHit.distance);
                        particleSys.Play();
                        audioManager.GetComponent<AudioManager>().PlayOneShot("EnemyLand", 0.05f);
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
                //If enemy can jump
                if (canJump)
                {
                    StartCoroutine("Jump");
                }
                break;
            case State.MOVING:
                //Do nothing
                break;
        }
    }

    IEnumerator Jump()
    {
        canJump = false;
        //Wait a few seconds
        yield return new WaitForSeconds(jumpDelay);

        //Update state
        state = State.MOVING;
        //Get angle
        transform.eulerAngles = transform.eulerAngles.x < jumpVectors[jumpVectorIndex].transform.position.x - transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle((Vector2)jumpVectors[jumpVectorIndex].transform.position - (Vector2)transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle((Vector2)jumpVectors[jumpVectorIndex].transform.position - (Vector2)transform.position, Vector2.up));
        audioManager.GetComponent<AudioManager>().PlayOneShot("EnemyJump", 0.2f);

        //Update jump vector index
        jumpVectorIndex = jumpVectorIndex >= (jumpVectors.Count - 1) ? 0 : jumpVectorIndex + 1;
    }

    void OnTriggerEnter2D(Collider2D colObj)
    {
        if (colObj.gameObject.CompareTag("CollisionObject"))
        {
            StopCharacterMovement();
        }
    }

    void StopCharacterMovement()
    {
        //Get collision point
        RaycastHit2D rayHit = RaycastCircle("CollisionObject", transform.position, 360, 2f);

        //Set state
        state = State.STATIONARY;

        //Set first position to line up planes
        transform.position = rayHit.point;

        //Set rotation
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rayHit.normal);

        //Set final position
        RaycastHit2D closestBackHit = GetClosestRaycastHit2D("CollisionObject", transform.position, -transform.up, 2f);
        float moveDistance = (playerLength / 2) - closestBackHit.distance;
        transform.Translate(Vector2.up * moveDistance);

        //Character can jump again
        canJump = true;
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

        for (int i = 0; i < segments; i++)
        {
            float angle = (360 / segments) * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
            RaycastHit2D rayHit = GetClosestRaycastHit2D(compareTag, origin, dir, radius);
            if (rayHit.transform.gameObject.CompareTag(compareTag))
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
}
