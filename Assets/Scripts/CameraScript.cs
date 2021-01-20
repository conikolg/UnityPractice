 using System;
 using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Smooth camera taken and modified heavily from here
    // https://gamedev.stackexchange.com/questions/114742/how-can-i-make-camera-to-follow-smoothly

    // The target to follow
    public Transform target;

    // Calculate height and distance from player with hypotenuse and angle
    private float hypotenuse = 17;
    private float angleOfDepression = 65;
    private bool inDegrees = true;

    // How quickly should changes in height or direction be applied
    private float yDamping = 50;
    private float zDamping = 50;
    private float xDamping = 50;
    // public float rotationDamping;

    // Where the camera should be positioned
    private Vector3 _positionRelativeToTarget;

    public void Start()
    {
        // Ensure angle is in radians
        float angleOfDepressionRadians = inDegrees ? angleOfDepression * Mathf.Deg2Rad : angleOfDepression;
        // Calculate distances away from player
        float horizontalDistance = Mathf.Cos(angleOfDepressionRadians) * hypotenuse;
        float verticalDistance = Mathf.Sin(angleOfDepressionRadians) * hypotenuse;
        _positionRelativeToTarget = Vector3.back * horizontalDistance + Vector3.up * verticalDistance;

        // Point at target - rotate around x axis
        transform.rotation = Quaternion.AngleAxis(angleOfDepressionRadians*Mathf.Rad2Deg, Vector3.right);
    }

    void LateUpdate()
    {
        var startTime = DateTime.Now;
        
        if (target)
        {
            Vector3 currentPosition = transform.position;
            Vector3 wantedPosition = target.transform.position + _positionRelativeToTarget;
            float newX = Mathf.Lerp(currentPosition.x, wantedPosition.x, xDamping * Time.deltaTime);
            float newY = Mathf.Lerp(currentPosition.y, wantedPosition.y, yDamping * Time.deltaTime);
            float newZ = Mathf.Lerp(currentPosition.z, wantedPosition.z, zDamping * Time.deltaTime);
            
            // Calculate the current rotation angles
            // wantedRotationAngle = target.eulerAngles.y;
            // // currentRotationAngle = transform.eulerAngles.y;
            // Damp the rotation around the y-axis
            // currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
            // Convert the angle into a rotation
            // currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set new position of camera
            transform.position = new Vector3(newX, newY, newZ);
        }
        
        var elapsed = (DateTime.Now - startTime).Milliseconds;
        print("CameraScript LateUpdate: " + elapsed);
    }
}