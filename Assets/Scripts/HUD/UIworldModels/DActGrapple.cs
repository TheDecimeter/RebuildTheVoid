using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DActGrapple : DemoAction
{
    public Grapple laser;
    public Transform [] targets;
    public float ShootTime, ShootCycle;
    private Transform target;

    public override void Do(float delta)
    {
        float timer = delta % ShootCycle;
        if (delta == 0)
            AckwireTarget();
        else if (delta == 1)
            StopFiring();
        else
            UpdateShot(timer);

    }

    private void UpdateShot(float timer)
    {
        if (timer > ShootTime)
        {
            AckwireTarget();
        }
        else
        {
            laser.PointAt(target.position);
        }
    }
    private void StopFiring()
    {
        ResetAction();
    }

    private void AckwireTarget()
    {
        target = targets[Random.Range(0, targets.Length)];
        laser.Retract();
    }

    public override void ResetAction()
    {
        laser.Retract();
    }

   
}
