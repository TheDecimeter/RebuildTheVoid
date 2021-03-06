﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehavior : Character
{
    public WorkerBFS Goals;
    public Vector2Int StartMapPos;

    private Vector2Int currentGoal;
    Next next;
    private Vector3 heading, startPos;
    private float timer = 0, unitsPerSec = 10, totalDistance=0;
    private bool reachedFarGoal;
    private Tile CurrentTile;

    private void Start()
    {
        transform.position = LevelController.PhysicalLocation(StartMapPos.x, StartMapPos.y);
        reachedFarGoal = false;
        currentGoal = Goals.Goal1;

        CurrentTile= Goals.Level.MapTile(gameObject);
        CurrentTile.AddCharacter(this);
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
        CheckIfReachedGoal(x, y);

        if (Goals.TryGetMove(currentGoal, x, y, out gx, out gy))
        {
            //Vector3 nextPos = LevelController.PhysicalLocation(x + gx, y + gy) + GetOffset();
            Vector3 nextPos = Goals.Level.MapTile(x + gx, y + gy).PullPoint.transform.position + GetOffset();
            Vector3 course = nextPos-transform.position;
            //course = new Vector3(course.x, 0, course.z);
            heading = course.normalized;
            timer = 0;
            startPos = transform.position;
            totalDistance = course.magnitude;
            //print("on " + x + " " + y + " plotted next move to " + (x+gx) + " " + (y+gy));
        }
        else
        {
            //print("failed to get map, trying remapping");
            Goals.TryMapToMe(currentGoal, x, y);
        }
    }

    private bool CheckIfReachedGoal(int x, int y)
    {
        if (x == currentGoal.x && y == currentGoal.y)
        {
            if (currentGoal == Goals.Goal2)
            {
                if (reachedFarGoal)
                {
                    Reward();
                    currentGoal = Goals.Goal1;

                    //print("reached goal reward");
                }
            }
            else
            {
                reachedFarGoal = true;
                currentGoal = Goals.Goal2;
                //print("reached half goal");
            }
            return true;
        }
        return false;
    }

    private void Reward()
    {
        //print("NPC reached goal");
        reachedFarGoal = false;
        Inventory.AddMoney(1);
    }

    private void UpdateStep()
    {
        timer += Time.deltaTime;
        float distanceTraveled = timer * unitsPerSec;

        if (distanceTraveled >= totalDistance)
        {
            SetNextStep();
        }
        else
        {
            transform.position = startPos + heading * distanceTraveled;
            UpdateTile();
        }

    }

    private void UpdateTile()
    {
        Tile t = Goals.Level.MapTile(gameObject);
        if (t != CurrentTile)
        {
            CurrentTile.RemoveCharacter(this);
            CurrentTile = t;
            CurrentTile.AddCharacter(this);
        }
    }

    private Vector3 GetOffset()
    {
        return new Vector3(Random.Range(-4, 4), 1, Random.Range(-4, 4));
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
