using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    // Reference to main game camera
    [FormerlySerializedAs("MainCamera")] public Camera mainCamera;

    // Layers to consider when calculating world position from mouse position for movement commands
    public LayerMask movementLayerMask;
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
    private Vector3 dashDestination;
    private const float MaxDashDistance = 20f;
    private const bool MustDashMaxDistance = true;

    private float dashSpeed = 20f;
    private float dashTime = 0.5f;

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
        dashDestination = Vector3.zero;
    }

    public void TeleportPlayer(Vector3 location)
    {
        targetDestination = location;
        dashDestination = location;
        transform.position = location;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
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
            StartCoroutine(Dash());
        }
    }

    // FixedUpdate is called by the Physics System
    private void FixedUpdate()
    {
        // Dash priority higher than normal walking priority
        if (isDashing)
        {
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
            movementLayerMask))
        {
            // Compute new target destination of player
            targetDestination = new Vector3(hit.point.x, playerRigidbody.position.y, hit.point.z);
            isWalking = true;
        }
    }

    private bool CanMove(Vector3 dir, float distance)
    {
        return !Physics.Raycast(playerRigidbody.position, dir, distance, dashingCollisionLayers);
    }

    private bool TryMove(Vector3 baseMoveDir, float distance)
    {
        Vector3 moveDir = baseMoveDir;
        bool canMove = CanMove(moveDir, distance);
        // if (!canMove)
        // {
        //     // Cannot move diagonally
        //     moveDir = new Vector3(baseMoveDir.x, 0f, 0f).normalized;
        //     canMove = moveDir.x != 0f && CanMove(moveDir, distance);
        //     if (!canMove)
        //     {
        //         // Cannot move horizontally
        //         moveDir = new Vector3(0f, 0f, baseMoveDir.z).normalized;
        //         canMove = moveDir.y != 0f && CanMove(moveDir, distance);
        //     }
        // }

        if (canMove)
        {
            playerRigidbody.position += moveDir * distance;
        }

        return canMove;
    }

    private IEnumerator Dash()
    {
        Debug.Log("Starting the dash");
        isDashing = true;
        float startTime = Time.time;

        RaycastHit hit;
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
            out hit, Mathf.Infinity, movementLayerMask);

        //targetDestination = new Vector3(hit.point.x, playerRigidbody.position.y, hit.point.z);
        // Compute new mouse location on the ground
        Vector3 location = new Vector3(hit.point.x, playerRigidbody.position.y, hit.point.z);
        Vector3 dashMovement = location - playerRigidbody.position;

        while (Time.time < startTime + dashTime)
        {
            // Turn towards the intended destination
            Quaternion intendedLookDir = Quaternion.LookRotation(dashMovement);
            playerRigidbody.rotation = Quaternion.RotateTowards(
                playerRigidbody.rotation,
                intendedLookDir,
                RotationSpeed * Time.deltaTime);

            if (!TryMove(dashMovement.normalized, dashSpeed * Time.deltaTime))
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Dash is over");
        targetDestination = transform.position;
        isWalking = false;
        isDashing = false;
    }
}