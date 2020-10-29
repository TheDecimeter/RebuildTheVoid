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
    public float OffAngle;
    public float followSpeed;

    private void Start()
    {
        Follow();
        Point();
    }
    
    void Update()
    {
        Follow();
    }

    private void FixedUpdate()
    {
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
        Vector3 toTarget = Target.position - Camera.transform.position;
        float offAngle = Mathf.Abs(Vector3.Angle(toTarget, Camera.transform.forward));
        if (offAngle > OffAngle)
        {
            Calculator.LookAt(Target);
            Camera.rotation = Quaternion.Slerp(Camera.rotation, Calculator.rotation, Time.fixedDeltaTime*followSpeed*(offAngle-OffAngle));
        }
        else
        {
            Calculator.LookAt(Target);
            Camera.rotation = Quaternion.Slerp(Camera.rotation, Calculator.rotation, Time.fixedDeltaTime);
        }
    }
}
