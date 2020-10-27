using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public GameObject rope;

    public void PointAt(Vector3 pillar)
    {
        rope.SetActive(true);
        transform.LookAt(pillar);
        Vector3 dist = transform.position - pillar;
        rope.transform.localScale = new Vector3(.1f, dist.magnitude/2, .1f);

        rope.transform.localPosition = new Vector3(0, 0, dist.magnitude / 2);
    }
    public void Retract()
    {
        rope.SetActive(false);
    }
}
