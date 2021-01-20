using UnityEngine;

public class TeleporterAreaOfEffect : MonoBehaviour
{
    private float _time;
    private const float TimeThreshold = 5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();

        Debug.Log("Player has entered the teleportation radius!");
        _time = Time.unscaledTime;
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();

        if (_time + TimeThreshold > Time.unscaledTime)
        {
            Debug.Log("Player is waiting for teleportation...");
        }
        else
        {
            Debug.Log("Player should have teleported!");
            player.TeleportTo(new Vector3(0f, 0f, 150f));
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     PlayerScript player = other.GetComponent<PlayerScript>();
    //
    //     if (_time + TimeThreshold > Time.unscaledTime)
    //     {
    //         Debug.Log("Player is leaving - should not have teleported!");
    //     }
    //     else
    //     {
    //         Debug.Log("Player is leaving - should have teleported!");
    //     }
    // }
}