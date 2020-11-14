using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareActivation : MonoBehaviour
{
    public GameObject[] share;

    private void OnEnable()
    {
        set(true);
    }

    private void OnDisable()
    {
        set(false);
    }

    private void set(bool active)
    {
        foreach (GameObject g in share)
            g.SetActive(active);
    }
}
