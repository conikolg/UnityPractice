using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class BulletPhysics : MonoBehaviour
{
    private float moveSpeed = 10f;
    private Vector3 shootDir;
    private Rigidbody hookRigidbody;

    private PlayerScript.EndHook endHook;
    private float timeToLive = 1f;

    private float deletionTime;

    public void Setup(Vector3 shootDir, PlayerScript.EndHook endHook)
    {
        this.endHook = endHook;
        hookRigidbody = GetComponent<Rigidbody>();
        this.shootDir = shootDir;

        hookRigidbody.AddForce(this.shootDir * moveSpeed, ForceMode.Impulse);

        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(shootDir));
        deletionTime = Time.time + timeToLive;

        StartCoroutine(DeleteBullet());
    }

    private IEnumerator DeleteBullet()
    {
        while (Time.time < deletionTime)
        {
            yield return null;
        }

        endHook();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // Hook enemy
            print("Hit enemy");
            hookRigidbody.AddForce(-shootDir * moveSpeed, ForceMode.Impulse);

            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(-shootDir));
            enemy.HookEnemy(shootDir, moveSpeed);
            endHook();
            Destroy(gameObject);
        }
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        var angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
        {
            n += 360;
        }

        return n;
    }
}