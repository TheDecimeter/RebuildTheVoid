using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    public float loopTime = 1;
    public int repitions = -1;
    public DemoManager[] reset;
    public Action[] actions;
    private float delta = 0;
    private int repitionCount = 0;
    private HashSet<Action> inProgress = new HashSet<Action>();

    [System.Serializable]
    public struct Action{
        public float start, end;
        public DemoAction action;
    }
    
    void Update()
    {
        if (repitionCount == repitions)
            return;
        delta += Time.deltaTime;
        if (delta >= loopTime)
        {
            repitionCount++;
            ResetActions();
        }
        else
            UpdateActions(delta);
    }

    public void ResetAll()
    {
        ResetActions();
        repitionCount = 0;
    }

    private void ResetActions()
    {
        delta = 0;
        foreach (Action a in actions)
            a.action.ResetAction();
        foreach (DemoManager d in reset)
            d.ResetAll();
        inProgress = new HashSet<Action>();
    }

    private void UpdateActions(float delta)
    {
        foreach (Action a in actions)
        {
            if (InTime(a, delta))
            {
                if (inProgress.Contains(a))
                {
                    a.action.Do(Delta(a, delta));
                }
                else
                {
                    a.action.Do(0);
                    inProgress.Add(a);
                }
            }
            else
            {
                if (inProgress.Contains(a))
                {
                    inProgress.Remove(a);
                    a.action.Do(1);
                }
            }
        }
    }

    

    private bool InTime(Action a, float delta)
    {
        return delta >= a.start && delta < a.end;
    }

    private float Delta(Action a, float delta)
    {
        return (delta - a.start) / (a.end - a.start);
    }
}
