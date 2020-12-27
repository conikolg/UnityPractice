using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /* Set the parameters of the transform object for the ground plane */
        // Set the position to the origin
        transform.position = Vector3.zero;
        // Set scale
        transform.localScale = new Vector3(10, 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
