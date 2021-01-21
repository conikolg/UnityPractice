using UnityEngine;

public class TeleporterAreaOfEffect : MonoBehaviour
{
    private float _time;
    private const float TimeThreshold = 3f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();

        if (player != null)
            _time = Time.unscaledTime;
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();

        if (player != null)
        {
            if (_time + TimeThreshold <= Time.unscaledTime)
            {
                player.TeleportTo(new Vector3(0f, 0f, 150f));
            }
        }
    }
}