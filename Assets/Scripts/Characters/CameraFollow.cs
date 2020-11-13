using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public PlayerMovement Player;
    public Transform Camera;
    public Transform Calculator;
    public float MaxFollowDistance;
    public float MinFollowDistance;
    public float OffAngle;
    public float followSpeed;

    public float CenterCamSpeed;
    private float centerCamDelay;
    public float CenterCamTime;
    private float centerCamTimer;
    private Vector3 fromPos;
    private Quaternion fromRot, toRot;
    private AnimationCurve centerDist = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Transform Target;

    private void Start()
    {
        centerCamTimer = CenterCamTime;
        CenterCamSpeed = -CenterCamSpeed;
        centerCamDelay = 1 / CenterCamSpeed;
        Target = Player.transform;
        Follow(1);
        Point(1);
    }
    
    void Update()
    {
        Follow(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Point(Time.fixedDeltaTime);
    }

    private void Follow(float delta)
    {
        if (centerCamTimer <= 0)
        {
            if (centerCamTimer > centerCamDelay)
            {
                Vector3 behind = Player.transform.position - Player.transform.forward * MaxFollowDistance;
                transform.position = Vector3.Slerp(fromPos, behind, centerDist.Evaluate(centerCamTimer * CenterCamSpeed));
            }
            return;
        }

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

    private void Point(float delta)
    {

        Calculator.LookAt(Target);

        float speed = Player.GetLookAhead().magnitude;
        if (speed == 0)
        {
            
            centerCamTimer -= delta;
            if (centerCamTimer < 0)
            {
                if (centerCamTimer > centerCamDelay)
                {
                    Camera.rotation = Quaternion.Slerp(fromRot, Calculator.rotation, centerDist.Evaluate(centerCamTimer * CenterCamSpeed));
                }
                else
                {
                    Camera.LookAt(Target);
                    return;
                }
            }
        }
        else
        {

            centerCamTimer = CenterCamTime;
            fromPos = transform.position;
            fromRot = Camera.rotation;
            //print("speed is not 0");
        }

        

        Vector3 toTarget = Target.position - Camera.transform.position;
        float offAngle = Mathf.Abs(Vector3.Angle(toTarget, Camera.transform.forward));
        if (offAngle > OffAngle)
        {
            
            Camera.rotation = Quaternion.Slerp(Camera.rotation, Calculator.rotation, delta*followSpeed*(offAngle-OffAngle));
        }
        else
        {
            Camera.rotation = Quaternion.Slerp(Camera.rotation, Calculator.rotation, delta);
        }
    }
}
