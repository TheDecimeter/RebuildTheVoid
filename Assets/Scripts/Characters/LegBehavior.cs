using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegBehavior : MonoBehaviour
{
    public PingPong[] movingParts;
    public Spin[] spinningParts;

    [System.Serializable]
    public class PingPong
    {
        public Transform obj;
        public Transform start;
        public Transform goal;
        public float minTime;
        public float maxTime;
        public float margin;

        private float i, t, upBnd, lwBnd;
        public void Init()
        {
            i = 0;
            minTime = 1 / minTime;
            maxTime = 1 / maxTime;
            set(1);
        }

        private void set(float dir)
        {
            t = Random.Range(minTime, maxTime) * dir;
            upBnd = 1 - Random.Range(0, margin);
            lwBnd = Random.Range(0, margin);
        }

        public void Update(float dt)
        {
            i += dt * t;

            if (i >= upBnd)
            {
                set(-1);
                i = upBnd;
            }
            else if (i <= lwBnd)
            {
                set(1);
                i = lwBnd;
            }

            //yes, non local rotation, it's cuter and more chaotic that way
            obj.localRotation = Quaternion.Slerp(start.rotation, goal.rotation, i);
        }
    }

    [System.Serializable]
    public struct Spin
    {
        public Transform obj;
        public Vector3 rotationVector;
        public float minTime;
        public float maxTime;
        
        public void Init()
        {
            minTime = 1 / minTime;
            maxTime = 1 / maxTime;
        }

        public void Update(float dt)
        {
            obj.Rotate(rotationVector * dt * Random.Range(minTime, maxTime));
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        float dt = Time.deltaTime;
        foreach (Spin s in spinningParts)
            s.Init();
        foreach (PingPong p in movingParts)
            p.Init();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        foreach (Spin s in spinningParts)
            s.Update(dt);
        foreach (PingPong p in movingParts)
            p.Update(dt);
    }

    public void SpinUp(float seconds)
    {

    }
}
