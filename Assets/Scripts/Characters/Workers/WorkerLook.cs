using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerLook : MonoBehaviour
{
    public Transform worker;

    private float minTime = 30;
    private float maxTime = 50;
    private float margin = .2f;

    private class Data
    {
        public float i, t, upBnd, lwBnd;
    }
    private Data x, y, z;
    private Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        x = new Data();
        y = new Data();
        z = new Data();
        Init();
        dir = new Vector3(Random.Range(0.1f, 1), Random.Range(0.1f, 1), Random.Range(0.1f, 1)).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        sUpdate(Time.deltaTime, x);
        sUpdate(Time.deltaTime, y);
        sUpdate(Time.deltaTime, z);
        worker.localScale = new Vector3(x.i, y.i, z.i);
    }


    public void Init()
    {
        minTime = 1 / minTime;
        maxTime = 1 / maxTime;
        set(1, x);
        set(1, y);
        set(1, z);
        x.i = x.upBnd;
        y.i = y.upBnd;
        z.i = z.upBnd;
    }

    private void set(float dir, Data d)
    {
        d.t = Random.Range(minTime, maxTime) * dir;
        d.upBnd = 1 - Random.Range(0, margin);
        d.lwBnd = Random.Range(.5f, margin);
    }

    private void sUpdate(float dt, Data d)
    {
        d.i += dt * d.t;

        if (d.i >= d.upBnd)
        {
            set(-1, d);
        }
        else if (d.i <= d.lwBnd)
        {
            set(1, d);
        }
        
    }
}
