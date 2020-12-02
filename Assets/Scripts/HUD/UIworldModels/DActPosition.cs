using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DActPosition : DemoAction
{
    public AnimationCurve Dist = AnimationCurve.Linear(0, 0, 1, 1);
    public Transform goal;
    private Vector3 resetPos, startPos;

    public override void Do(float delta)
    {
        if (delta == 0)
        {
            print("reset start pos "+gameObject.name+" "+ transform.localPosition);
            startPos = transform.localPosition;
        }
        else if (delta == 1)
        {
            print("reset endPos " + gameObject.name + " " + transform.localPosition);
        }
        transform.localPosition = Vector3.Lerp(startPos, goal.localPosition, Dist.Evaluate(delta));
    }

    public override void ResetAction()
    {
        transform.localPosition = resetPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        print("reset position " + transform.localPosition);
        resetPos = transform.localPosition;
    }
    
}
