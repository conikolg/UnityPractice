using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Transform pfBulletPhysics;

    // Reference to main game camera
    public Camera mainCamera;

    // Layers to consider when calculating world position from mouse position for movement commands
    public LayerMask mouseClickLayerMask;
    public LayerMask dashingCollisionLayers;

    // Reference to the RigidBody for this player
    private Rigidbody playerRigidbody;

    /* Normal walking movement parameters */
    private bool isWalking; // If the player is walking 
    private Vector3 targetDestination; // Where the player is trying to walk/move to
    private const float MovementSpeed = 8f; // How quickly player can move in units/second
    private const float RotationSpeed = 1080f; // How quickly player can turn in degrees/second

    /* Dash movement parameters */
    private bool isDashing;
    private float dashSpeed = 20f;
    private float dashTime = 0.5f;
    private Vector3 dashLocation;
    private Vector3 dashMovement;
    private float startingDashTime;

    private bool isHooking;
    private bool startHook;
    private float hookSpeed = 20f;
    private float hookTime = 0.5f;
    private Vector3 hookLocation;
    private Vector3 hookMovement;
    private float startingHookTime;

    public delegate void EndHook();

    public EndHook endHook;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize RigidBody object
        playerRigidbody = GetComponent<Rigidbody>();

        // Set position to center, not inside the ground
        transform.position = new Vector3(0, 1, 0);

        // Set target destination to player's own position initially
        isWalking = false;
        targetDestination = Vector3.zero;

        // Set dash status to not dashing
        isDashing = false;

        endHook = EndHookMethod;
    }

    public void TeleportPlayer(Vector3 location)
    {
        targetDestination = location;
        transform.position = location;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing || isHooking || startHook)
        {
            return;
        }

        // Check if the player is holding left-click down this frame
        if (Input.GetMouseButton(0))
        {
            OnMouse1Click();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDash();
            //StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            startHook = true;
            //StartHook();
        }
    }

    // FixedUpdate is called by the Physics System
    private void FixedUpdate()
    {
        if (startHook)
        {
            StartHook();
        }

        if (isHooking)
        {
            // stand still during hook
        }
        else if (isDashing)
        {
            if (Time.time < startingDashTime + dashTime)
            {
                // Turn towards the intended destination
                Quaternion intendedLookDir = Quaternion.LookRotation(dashMovement);
                playerRigidbody.rotation = Quaternion.RotateTowards(
                    playerRigidbody.rotation,
                    intendedLookDir,
                    RotationSpeed * Time.deltaTime);

                if (!TryMove(dashMovement.normalized, dashSpeed * Time.deltaTime))
                {
                    // Hit an obstacle so end dash
                    EndDash();
                }
            }
            else
            {
                // Dash time over so end dash
                EndDash();
            }
        }
        else if (isWalking)
        {
            // Compute how the player would move to get there in one step
            Vector3 movement = targetDestination - playerRigidbody.position;
            movement = new Vector3(movement.x, 0, movement.z);

            // If destination is farther than the player can move since the last frame...
            if (movement.magnitude > MovementSpeed * Time.deltaTime)
            {
                // Turn towards the intended destination
                Quaternion intendedLookDir = Quaternion.LookRotation(movement);
                playerRigidbody.rotation = Quaternion.RotateTowards(
                    playerRigidbody.rotation,
                    intendedLookDir,
                    RotationSpeed * Time.deltaTime);
                // Move the maximum possible distance in the needed direction
                playerRigidbody.MovePosition(playerRigidbody.position +
                                             MovementSpeed * Time.deltaTime * movement.normalized);
            }
            else
            {
                // Will arrive at destination, so instant turn towards destination
                playerRigidbody.rotation = Quaternion.LookRotation(movement);
                // Arrive at the destination.
                playerRigidbody.position = targetDestination;
                isWalking = false;
            }
        }
    }

    // Handle what happens when Mouse 1 is clicked
    private void OnMouse1Click()
    {
        // Raycast to the onscreen location -> get global coordinates of that click on the ground
        RaycastHit hit;
        if (Physics.Raycast(
            mainCamera.ScreenPointToRay(Input.mousePosition),
            out hit,
            Mathf.Infinity,
            mouseClickLayerMask))
        {
            // Compute new target destination of player
            targetDestination = new Vector3(hit.point.x, playerRigidbody.position.y, hit.point.z);
            isWalking = true;
        }
    }

    private bool CanMove(Vector3 dir, float distance)
    {
        Debug.DrawRay(playerRigidbody.position, dir, Color.red, 2f);
        return !Physics.Raycast(playerRigidbody.position, dir, distance, dashingCollisionLayers);
    }

    private bool TryMove(Vector3 baseMoveDir, float distance)
    {
        Vector3 moveDir = baseMoveDir;
        bool canMove = CanMove(moveDir, distance);

        if (canMove)
        {
            playerRigidbody.position += moveDir * distance;
        }

        return canMove;
    }

    private void EndDash()
    {
        //print("Dash is over");
        targetDestination = transform.position;
        isWalking = false;
        isDashing = false;
    }

    private void StartDash()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
            out hit, Mathf.Infinity, mouseClickLayerMask))
        {
            startingDashTime = Time.time;
            isDashing = true;
            //print("Starting the dash");

            //targetDestination = new Vector3(hit.point.x, playerRigidbody.position.y, hit.point.z);
            // Compute new mouse location on the ground
            var playerRigidbodyPosition = playerRigidbody.position;
            dashLocation = new Vector3(hit.point.x, playerRigidbodyPosition.y, hit.point.z);
            dashMovement = dashLocation - playerRigidbodyPosition;
        }
    }

    private void StartHook()
    {
        // Make hook child object visible, and start moving it and checking for its collision
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
            out hit, Mathf.Infinity, mouseClickLayerMask))
        {
            startingHookTime = Time.time;
            isHooking = true;
            hookLocation = new Vector3(hit.point.x, playerRigidbody.position.y, hit.point.z);
            hookMovement = hookLocation - playerRigidbody.position;

            Quaternion intendedLookDir = Quaternion.LookRotation(hookMovement);
            playerRigidbody.rotation = intendedLookDir;
            
            Transform bulletTransform = Instantiate(pfBulletPhysics,
                playerRigidbody.position + intendedLookDir * Vector3.forward * 2,
                Quaternion.identity);
            Vector3 shootDir = (hookMovement.normalized);
            bulletTransform.GetComponent<BulletPhysics>().Setup(shootDir, endHook);
            
            startHook = false;
            isHooking = true;
        }
    }

    private void EndHookMethod()
    {
        isHooking = false;
        isWalking = false;
        isDashing = false;
        startHook = false;
        Debug.Log("End hook is called");
    }
}