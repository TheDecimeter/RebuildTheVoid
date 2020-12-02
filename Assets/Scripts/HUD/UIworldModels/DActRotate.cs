using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DActRotate : DemoAction
{
    public AnimationCurve Dist = AnimationCurve.Linear(0,0,1,1);
    public Transform goal;
    private Quaternion resetRot, startRot;

    public override void Do(float delta)
    {
        if (delta == 0)
            startRot = transform.rotation;
        transform.rotation = Quaternion.Slerp(startRot, goal.rotation, Dist.Evaluate(delta));
        //Lerp(Dist.Evaluate(delta));
    }

    public override void ResetAction()
    {
        transform.rotation = resetRot;
    }

    void Start()
    {
        resetRot = transform.rotation;
    }

    private void Lerp(float delta)
    {
        float x = Mathf.Lerp(startRot.x, goal.rotation.x, Dist.Evaluate(delta));
        float y = Mathf.Lerp(startRot.y, goal.rotation.y, Dist.Evaluate(delta));
        float z = Mathf.Lerp(startRot.z, goal.rotation.z, Dist.Evaluate(delta));
        float w = Mathf.Lerp(startRot.w, goal.rotation.w, Dist.Evaluate(delta));
        transform.rotation = new Quaternion(x, y, z, w);
    }
}
