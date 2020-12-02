using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DemoAction : MonoBehaviour
{
    public abstract void Do(float delta);
    public abstract void ResetAction();
}
