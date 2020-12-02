using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DActActive : DemoAction
{
    public bool defaultOn = false;

    public override void Do(float delta)
    {
        if (delta == 0)
            gameObject.SetActive(!defaultOn);
        else if (delta == 1)
            gameObject.SetActive(defaultOn);
    }

    public override void ResetAction()
    {
        gameObject.SetActive(defaultOn);
    }

    private void Start()
    {
        gameObject.SetActive(defaultOn);
    }
}
