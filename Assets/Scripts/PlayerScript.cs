using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    // Reference to main game camera
    [FormerlySerializedAs("MainCamera")] public Camera mainCamera;

    // Layers to consider when calculating world position from mouse position for movement commands
    public LayerMask movementLayerMask;

    // Reference to the RigidBody for this player
    private Rigidbody _rigidbody;

    /* Normal walking movement parameters */
    private bool _isWalking; // If the player is walking 
    private Vector3 _walkDestination; // Where the player is trying to walk/move to
    private const float WalkingMovementSpeed = 8f; // How quickly player can walk in units/second
    private const float RotationSpeed = 1080f; // How quickly player can turn in degrees/second

    /* Dash movement parameters */
    private bool _isDashing;
    private const float DashingMovementSpeed = 50f; // How quickly player can walk in units/second
    private const float MaxDashDistance = 8f; // How far, at most, can the dash can carry the player
    private const float MinDashDistance = 4f; // How far, at least, can the dash can carry the player
    private Vector3 _dashDestination;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize RigidBody object
        _rigidbody = GetComponent<Rigidbody>();

        // Set position to center, not inside the ground
        transform.position = new Vector3(0, 1, 0);

        // Set target destination to player's own position initially
        _isWalking = false;
        _walkDestination = Vector3.zero;

        // Set dash status to not dashing
        _isDashing = false;
        _dashDestination = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is holding left-click down this frame
        if (Input.GetMouseButton(0))
        {
            OnMouse1Click();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpaceBarPressed();
        }
    }

    // FixedUpdate is called by the Physics System
    private void FixedUpdate()
    {
        // Dash priority higher than normal walking priority
        if (_isDashing)
        {
            // BlinkDash();
            NormalDash();
        }
        else if (_isWalking)
        {
            Walk();
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
            _walkDestination = new Vector3(hit.point.x, _rigidbody.position.y, hit.point.z);
            _isWalking = true;
        }
    }

    // Handle what happens when the space bar is pressed
    private void OnSpaceBarPressed()
    {
        // Caches the rigidbody position
        Vector3 currentPosition = _rigidbody.position;

        // Raycast to the onscreen location of mouse -> get global coordinates of that on the ground
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
            out hit, Mathf.Infinity, movementLayerMask))
        {
            // Compute new mouse location on the ground
            Vector3 location = new Vector3(hit.point.x, currentPosition.y, hit.point.z);
            // Compute movement vector based on input location
            Vector3 dashMovement = location - currentPosition;

            /* Correct dash movement for dash range boundaries */
            // Cap dash movement if trying to go too far
            if (dashMovement.sqrMagnitude > MaxDashDistance * MaxDashDistance)
                dashMovement = dashMovement.normalized * MaxDashDistance;
            // Check against the dash distance floor as well
            else if (dashMovement.sqrMagnitude < MinDashDistance * MinDashDistance)
                dashMovement = dashMovement.normalized * MinDashDistance;

            // Set the computed dash destination
            _dashDestination = currentPosition + dashMovement;
            // Instantly turn to that direction
            _rigidbody.rotation = Quaternion.LookRotation(_dashDestination - _rigidbody.position);
            // Set dashing status
            _isDashing = true;
        }
    }

    private void Walk()
    {
        // Compute how the player would move to get there in one step
        Vector3 movement = _walkDestination - _rigidbody.position;
        movement = new Vector3(movement.x, 0, movement.z);

        // If destination is farther than the player can move since the last frame...
        if (movement.magnitude > WalkingMovementSpeed * Time.deltaTime)
        {
            // Turn towards the intended destination
            Quaternion intendedLookDir = Quaternion.LookRotation(movement);
            _rigidbody.rotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                intendedLookDir,
                RotationSpeed * Time.deltaTime);
            // Move the maximum possible distance in the needed direction
            _rigidbody.MovePosition(_rigidbody.position + WalkingMovementSpeed * Time.deltaTime * movement.normalized);
        }
        else
        {
            // Will arrive at destination, so instant turn towards destination
            _rigidbody.rotation = Quaternion.LookRotation(movement);
            // Arrive at the destination.
            _rigidbody.position = _walkDestination;
            _isWalking = false;
        }
    }

    private void BlinkDash()
    {
        // Instantly turn to that direction
        _rigidbody.rotation = Quaternion.LookRotation(_dashDestination - _rigidbody.position);
        // Teleport to that location
        _rigidbody.MovePosition(_dashDestination);
        // Player is no longer dashing
        _isDashing = false;
    }

    private void NormalDash()
    {
        // Compute how the player would move to get there in one step
        Vector3 movement = _dashDestination - _rigidbody.position;
        movement = new Vector3(movement.x, 0, movement.z);

        // If destination is farther than the player can move since the last frame...
        if (movement.magnitude > DashingMovementSpeed * Time.deltaTime)
        {
            // Move the maximum possible distance in the needed direction
            _rigidbody.MovePosition(_rigidbody.position + DashingMovementSpeed * Time.deltaTime * movement.normalized);
        }
        else
        {
            // Arrive at the destination.
            _rigidbody.position = _dashDestination;
            _isDashing = false;
        }
    }
}