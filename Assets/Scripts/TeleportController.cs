using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    private float elpasedTime;

    public Vector3 teleportLocation;

    // Update is called once per frame
    private void Update()
    {
        elpasedTime += Time.deltaTime;

        if (elpasedTime > 10)
        {
            elpasedTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
        if (player != null)
        {
            elpasedTime = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript player = other.gameObject.GetComponent<PlayerScript>();
            // Debug.Log(other.tag);
            // Debug.Log("inisde tp");
            // Debug.Log(elpasedTime);
            if (elpasedTime > 2)
            {
                player.TeleportPlayer(teleportLocation);
                elpasedTime = 0;
            }
        }
    }
}