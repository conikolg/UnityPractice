using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    // Reference to main game camera
    [FormerlySerializedAs("MainCamera")] public Camera mainCamera;

    // Which movement type to use
    public bool moveWithMouseClick = true;
    public LayerMask movementLayerMask;

    // How fast the player can move in cardinal directions using arrow keys
    private const float CardinalSpeed = 6f;

    // How far can the player move in any direction using the arrow keys
    private const float OmnidirectionalSpeed = 8f;

    // Where the player is trying to go
    private Vector3 targetDestination;

    // Start is called before the first frame update
    void Start()
    {
        // Set position to center, not inside the ground
        transform.position = new Vector3(0, 1, 0);

        // Set target destination to player's own position initially
        targetDestination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Process movement based on mouse input
        if (moveWithMouseClick)
        {
            // Check if the player is holding left-click down this frame
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast to the onscreen location -> get global coordinates of that click on the ground
                RaycastHit hit;
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, movementLayerMask))
                {
                    // Compute new target destination of player
                    targetDestination = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
                }
            }

            /* Move the player as needed regardless */
            // Compute how the player would move to get there in one step
            Vector3 movement = targetDestination - transform.position;
            // Cap movement in this frame to the maximum possible distance moved this frame
            if (movement.magnitude > OmnidirectionalSpeed * Time.deltaTime)
            {
                movement = movement.normalized * OmnidirectionalSpeed;
            }

            // Move the player
            transform.Translate(movement * Time.deltaTime);
        }
        
        // Process movement based on keyboard input
        else
        {
            // Movement based on arrow keys - smoothing enabled by default
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(
                CardinalSpeed * horizontal * Time.deltaTime,
                0,
                CardinalSpeed * vertical * Time.deltaTime
            ));
        }
    }
}