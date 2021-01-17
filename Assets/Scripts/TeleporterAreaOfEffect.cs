using System;
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
            Debug.Log("Player would have teleported!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();

        if (_time + TimeThreshold > Time.unscaledTime)
        {
            Debug.Log("Player is leaving - would not have teleported!");
        }
        else
        {
            Debug.Log("Player is leaving - would have teleported!");
        }
    }
}