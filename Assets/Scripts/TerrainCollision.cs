using UnityEngine;

public class TerrainCollision : MonoBehaviour
{
    private void OnCollisionStay(Collision other)
    {
        // Stop player movement upon collision
        PlayerScript player = other.collider.GetComponent<PlayerScript>();
        if (player != null)
        {
            player.StopAllMovement();
        }
    }
}