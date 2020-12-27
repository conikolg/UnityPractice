using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private const float CardinalSpeed = 6f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set position to center, not inside the ground
        transform.position = new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void Update()
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