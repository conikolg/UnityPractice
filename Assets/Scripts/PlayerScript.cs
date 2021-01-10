using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    // Reference to main game camera
    [FormerlySerializedAs("MainCamera")] public Camera mainCamera;

    // Layers to consider when calculating world position from mouse position for movement commands
    public LayerMask movementLayerMask;

    // How far can the player move in any direction using the arrow keys
    private const float OmnidirectionalSpeed = 8f;

    // How quickly the player model can turn
    private const float RotationSpeed = 1000f;

    // Reference to the RigidBody for this player
    private Rigidbody _rigidbody;

    // Where the player is trying to walk/move to
    private bool _isWalking;
    private Vector3 _targetDestination;

    // Where the player is dashing to
    private bool _isDashing;
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
        _targetDestination = transform.position;

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
    }

    // FixedUpdate is called by the Physics System
    private void FixedUpdate()
    {
        if (_isWalking)
        {
            // Compute how the player would move to get there in one step
            Vector3 movement = _targetDestination - _rigidbody.position;
            movement = new Vector3(movement.x, 0, movement.z);

            // If destination is farther than the player can move since the last frame...
            if (movement.magnitude > OmnidirectionalSpeed * Time.deltaTime)
            {
                // Turn towards the intended destination
                Quaternion intendedLookDir = Quaternion.LookRotation(movement);
                _rigidbody.rotation = Quaternion.RotateTowards(
                    _rigidbody.rotation,
                    intendedLookDir,
                    RotationSpeed * Time.deltaTime);
                // Move the maximum possible distance in the needed direction
                _rigidbody.MovePosition(_rigidbody.position +
                                        OmnidirectionalSpeed * Time.deltaTime * movement.normalized);
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
}