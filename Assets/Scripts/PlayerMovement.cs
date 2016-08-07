using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

    Camera cam; //Scene camera
    LineRenderer lineRenderer; //Line renderer
    Vector2 mousePosition; //Mouse position (world space)
    bool canJump; //If the player is able to jump
    public GameObject probe, probeOrigin; //Probe to check forward collisions
    private GameObject audioManager; //Audio manager
    public float playerLength, playerWidth; //Player dimensions
    public float playerSpeed = 150f; //Player's speed
    ParticleSystem particleSys; //Particle system
    public GameObject soul; //Player's soul (spooky!)
    private bool jumpRedirected; //If player has used slo-mo ability
    public GameObject blastObj; //Blast object

    public enum State {STATIONARY, MOVING, JUMP_CHARGE};
    public State state;

    void Awake()
    {
        state = State.STATIONARY;
    }

    void Start()
    {
        //Gets object references
        canJump = false;
        jumpRedirected = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        lineRenderer = GetComponent<LineRenderer>();
        particleSys = transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
    }

    void Update()
    {
        UpdateMousePosition();
        UpdateLineRenderer();
        ManageInput();
    }

    void LateUpdate()
    {
        ManageMovement();
    }

    void UpdateMousePosition()
    {
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D canJumpHit = GetClosestRaycastHit2D("CollisionObject", transform.position, mousePosition - (Vector2)transform.position, 2 * playerLength);
        if (canJumpHit.transform.gameObject.CompareTag("Player") || canJumpHit.transform.gameObject.CompareTag("CameraNode"))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
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
                ResolveCollisions();
                break;
            case State.JUMP_CHARGE:
                ResolveCollisions();
                break;
        }
    }
    
    //Resolved player collisions
    void ResolveCollisions()
    {
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
                cam.GetComponent<CameraShake>().shakeDuration = 0.2f;
                audioManager.GetComponent<AudioManager>().PlayOneShot("ScreenShake", 0.5f);
            }
        }
    }

    //Manages player input
    void ManageInput()
    {
        switch (state)
        {
            case State.STATIONARY:
                Time.timeScale = 1;

                //Left click - Jump
                if (Input.GetMouseButtonDown(0))
                {
                    DisableLineRenderer();
                    jumpRedirected = false;

                    //If jump angle is okay
                    if(canJump)
                    {
                        //Update state
                        state = State.MOVING;
                        //Get angle
                        transform.eulerAngles = transform.eulerAngles.x < mousePosition.x - transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up));
                        audioManager.GetComponent<AudioManager>().PlayOneShot("JumpSound", 0.2f);
                    }
                    else
                    {
                        Debug.Log("Movement Error: Mouse angle less than 80 degrees!");
                    }
                }
                break;
            case State.MOVING:
                if(Input.GetMouseButtonDown(0) && !jumpRedirected)
                {
                    state = State.JUMP_CHARGE;
                }
                break;
            case State.JUMP_CHARGE:
                if(Time.timeScale > 0.1f)
                {
                    Time.timeScale = Time.timeScale - 0.2f;
                }
                else
                {
                    Time.timeScale = 0.1f;
                }

                EnableLineRenderer();

                if (Input.GetMouseButtonUp(0))
                {
                    Time.timeScale = 1;
                    DisableLineRenderer();
                    jumpRedirected = true;
   
                    //Get angle
                    transform.eulerAngles = transform.eulerAngles.x < mousePosition.x - transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up));
                    audioManager.GetComponent<AudioManager>().PlayOneShot("JumpSound", 0.2f);

                    state = State.MOVING;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Time.timeScale = 1;
                    DisableLineRenderer();
                    jumpRedirected = true;

                    ShootBlast();
                    state = State.MOVING;
                }
                break;
        }
    }

    void ShootBlast()
    {
        Instantiate(blastObj, transform.position, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D colObj)
    {
        if(colObj.gameObject.CompareTag("CollisionObject"))
        {
            StopCharacterMovement();
        }
        else if(colObj.gameObject.CompareTag("EnemyObject") || colObj.gameObject.CompareTag("JumpingEnemy"))
        {
            Die();
        }
        else if(colObj.gameObject.CompareTag("LevelEnd"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void StopCharacterMovement()
    {
        //Get collision point
        RaycastHit2D rayHit = RaycastCircle("CollisionObject", transform.position, 360, 2f);
        if(rayHit.transform.gameObject.CompareTag("CollisionObject"))
        {
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
        }
        
    }

    //The player dies
    public void Die()
    {
        Time.timeScale = 1;
        cam.GetComponent<CameraShake>().shakeDuration = 0.2f;
        audioManager.GetComponent<AudioManager>().PlayOneShot("DeathSound", 0.8f);
        Instantiate(soul, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
        Color c1;
        Color c2;

        if (canJump)
        {
            //Able to jump
            c1 = Color.blue;
            c2 = Color.blue;
        }
        else
        {
            //Unable to jump
            c1 = Color.red;
            c2 = Color.red;
        }

        lineRenderer.SetColors(c1, c2);
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
