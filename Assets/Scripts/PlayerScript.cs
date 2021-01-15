using System.Collections;
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
    private Vector3 _targetDestination; // Where the player is trying to walk/move to
    private const float MovementSpeed = 8f; // How quickly player can move in units/second
    private const float RotationSpeed = 1080f; // How quickly player can turn in degrees/second

    /* Dash movement parameters */
    private bool _isDashing;
    private Vector3 _dashDestination;
    private const float MaxDashDistance = 20f;
    private const bool MustDashMaxDistance = true;

    private float dashSpeed = 20f;
    private float dashTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize RigidBody object
        _rigidbody = GetComponent<Rigidbody>();

        // Set position to center, not inside the ground
        transform.position = new Vector3(0, 1, 0);

        // Set target destination to player's own position initially
        _isWalking = false;
        _targetDestination = Vector3.zero;

        // Set dash status to not dashing
        _isDashing = false;
        _dashDestination = Vector3.zero;
    }

    public void TeleportPlayer(Vector3 location)
    {
        _targetDestination = location;
        _dashDestination = location;
        transform.position = location;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDashing)
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
        if (_isDashing)
        {
        }
        else if (_isWalking)
        {
            // Compute how the player would move to get there in one step
            Vector3 movement = _targetDestination - _rigidbody.position;
            movement = new Vector3(movement.x, 0, movement.z);

            // If destination is farther than the player can move since the last frame...
            if (movement.magnitude > MovementSpeed * Time.deltaTime)
            {
                // Turn towards the intended destination
                Quaternion intendedLookDir = Quaternion.LookRotation(movement);
                _rigidbody.rotation = Quaternion.RotateTowards(
                    _rigidbody.rotation,
                    intendedLookDir,
                    RotationSpeed * Time.deltaTime);
                // Move the maximum possible distance in the needed direction
                _rigidbody.MovePosition(_rigidbody.position +
                                        MovementSpeed * Time.deltaTime * movement.normalized);
            }
            else
            {
                // Will arrive at destination, so instant turn towards destination
                _rigidbody.rotation = Quaternion.LookRotation(movement);
                // Arrive at the destination.
                _rigidbody.position = _targetDestination;
                _isWalking = false;
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
            _targetDestination = new Vector3(hit.point.x, _rigidbody.position.y, hit.point.z);
            _isWalking = true;
        }
    }

    private bool CanMove(Vector3 dir, float distance)
    {
        var position = transform.position;
        return !Physics.Raycast(position, dir, distance);
    }

    private bool TryMove(Vector3 baseMoveDir, float distance)
    {
        Vector3 moveDir = baseMoveDir;
        bool canMove = CanMove(moveDir, distance);
        if (!canMove)
        {
            // Cannot move diagonally
            moveDir = new Vector3(baseMoveDir.x, 0f).normalized;
            canMove = moveDir.x != 0f && CanMove(moveDir, distance);
            if (!canMove)
            {
                // Cannot move horizontally
                moveDir = new Vector3(0f, baseMoveDir.y).normalized;
                canMove = moveDir.y != 0f && CanMove(moveDir, distance);
            }
        }

        if (canMove)
        {
            _rigidbody.position += moveDir * distance;
        }

        return canMove;
    }

    private IEnumerator Dash()
    {
        Debug.Log("Starting the dash");
        _isDashing = true;
        float startTime = Time.time;

        RaycastHit hit;
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),
            out hit, Mathf.Infinity, movementLayerMask);

        //_targetDestination = new Vector3(hit.point.x, _rigidbody.position.y, hit.point.z);
        // Compute new mouse location on the ground
        Vector3 location = new Vector3(hit.point.x, _rigidbody.position.y, hit.point.z);
        Vector3 dashMovement = location - _rigidbody.position;

        while (Time.time < startTime + dashTime)
        {
            // Turn towards the intended destination
            Quaternion intendedLookDir = Quaternion.LookRotation(dashMovement);
            _rigidbody.rotation = Quaternion.RotateTowards(
                _rigidbody.rotation,
                intendedLookDir,
                RotationSpeed * Time.deltaTime);

            if (!TryMove(dashMovement.normalized, MovementSpeed * 2 * Time.deltaTime))
            {
                break;
            }

            yield return null;
        }

        Debug.Log("Dash is over");
        _targetDestination = transform.position;
        _isWalking = false;
        _isDashing = false;
    }
}