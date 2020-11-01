using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public InfoActionHUD message;
    public Collider trigger;

    private const int layer = 1 << 9;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layer)
            message.Activate(true);
        Physics.IgnoreCollision(trigger, other);
    }
}
