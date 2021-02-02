using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody enemyRigidbody;

    private bool isBeingHooked = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
    }

    public void HookEnemy(Vector3 shootDir, float moveSpeed)
    {
        isBeingHooked = true;
        enemyRigidbody.AddForce(-shootDir * moveSpeed, ForceMode.Impulse);

        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(-shootDir));
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