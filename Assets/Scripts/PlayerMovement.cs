using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    Camera cam; //Scene camera
    LineRenderer lineRenderer; //Line renderer
    Vector2 mousePosition; //Mouse position (world space)
    public float playerSpeed = 150f;

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
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        UpdateLineRenderer();
        ManageInput();
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

    //Manages player input
    void ManageInput()
    {
        switch(state)
        {
            case State.STATIONARY:
                //Left click - Jump
                if (Input.GetMouseButtonDown(0))
                {
                    //Update state
                    state = State.MOVING;
                    //Disables the line renderer
                    DisableLineRenderer();
                    //Get angle
                    transform.eulerAngles = transform.eulerAngles.x < mousePosition.x - transform.position.x ? new Vector3(0, 0, 360 - Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up)) : new Vector3(0, 0, Vector2.Angle(mousePosition - (Vector2)transform.position, Vector2.up));
                }
                break;
            case State.MOVING:
                //Move forward
                transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);
                if (Input.GetMouseButtonDown(0))
                {
                    //Update state
                    state = State.STATIONARY;
                    //Enables the line renderer
                    EnableLineRenderer();
                }
                break;
        }
    }
}
