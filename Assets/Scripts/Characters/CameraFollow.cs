using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Transform Camera;
    public Transform Calculator;
    public float MaxFollowDistance;
    public float MinFollowDistance;

    // Update is called once per frame
    void Update()
    {
        Follow();
        Point();
    }

    private void Follow()
    {
        Vector3 dir = transform.position - Target.position;
        if (dir.magnitude > MaxFollowDistance)
        {
            dir = new Vector3(dir.x, 0, dir.z).normalized;
            transform.position = Target.position + dir * MaxFollowDistance;
        }
        else if (dir.magnitude < MinFollowDistance)
        {
            dir = new Vector3(dir.x, 0, dir.z).normalized;
            transform.position = Target.position + dir * MinFollowDistance;
        }
    }

    private void Point()
    {
        //Camera.LookAt(Target);
        Calculator.LookAt(Target);
        Camera.rotation = Quaternion.Slerp(Camera.rotation, Calculator.rotation, 0.9f);
    }
}
