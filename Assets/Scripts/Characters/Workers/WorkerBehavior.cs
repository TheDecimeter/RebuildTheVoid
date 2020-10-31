using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehavior : MonoBehaviour
{
    public WorkerBFS Goals;

    private Vector2Int currentGoal;
    Next next;
    private Vector3 heading, startPos;
    private float timer = 0, unitsPerSec = 10, distance=0;
    private bool reachedFarGoal;

    private void Start()
    {
        reachedFarGoal = false;
        currentGoal = Goals.Goal1;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateStep();
    }

    private void SetNextStep()
    {
        int x, y, gx, gy;
        LevelController.MapLocation(gameObject, out x, out y);
        ReachedGoal(x, y);

        if (Goals.TryGetMove(currentGoal, x, y, out gx, out gy))
        {
            Vector3 nextPos = LevelController.PhysicalLocation(x + gx, y + gy) + GetOffset();
            Vector3 course = transform.position - nextPos;
            course = new Vector3(course.x, 0, course.z);
            heading = course.normalized;
            timer = 0;
            startPos = transform.position;
            distance = course.magnitude;
            print("on " + x + " " + y + " plotted next move to " + (x+gx) + " " + (y+gy));
        }
        else
            print("failed to get move");
    }

    private bool ReachedGoal(int x, int y)
    {
        if (x == currentGoal.x && y == currentGoal.y)
        {
            if (currentGoal == Goals.Goal2)
            {
                if (reachedFarGoal)
                {
                    Reward();
                    currentGoal = Goals.Goal1;

                    print("reached goal reward");
                }
            }
            else
            {
                reachedFarGoal = true;
                currentGoal = Goals.Goal2;
                print("reached half goal");
            }
            return true;
        }
        return false;
    }

    private void Reward()
    {
        print("NPC reached goal");
        reachedFarGoal = false;
        Inventory.AddMoney(1);
    }

    private void UpdateStep()
    {
        timer += Time.deltaTime;
        float traveled = timer * unitsPerSec;

        if (traveled >= distance)
        {
            SetNextStep();
        }
        else
        {
            transform.position = startPos - traveled * heading;
        }

    }

    private Vector3 GetOffset()
    {
        return new Vector3(Random.Range(-4, 4),0, Random.Range(-4, 4));
    }

    class Next
    {
        int x, y;
        public Next(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
