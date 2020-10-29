using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LevelController Level;
    public float Acceleration;
    public Grapple GrappleObject;
    public Inventory Inventory;
    public Camera Camera;


    private bool pressed = false, actionPerformed = false, wasSafe=true;
    private Vector3 pillar, pullPoint;

    private float tileWidth = 4.5f;
    private float rotationRadius = 1f;
    private float rotationDirection = 0;
    private float holdTimer = 0, lastAngle=0, deltaAngle, deltaAngleAction;
    private const float thrustTime = .1f, decayTime = .5f;

    private Vector3 lastDirToPillar;

    private Tile currentTile,previousTile;

    private int tileX, tileZ;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Tile t = Level.MapTile(gameObject);
        t.OnTouchUpdate(this);
        ThrustCheck();
        ActionCheck(t);
        BounceCheck(t);
    }

    private void ThrustCheck()
    {
        if (Input.anyKey)
        {
            if (!pressed)
            {
                pressed = true;
                Tile t = Level.MapTile(gameObject);
                if (Safe(t))
                {
                    pillar = t.Pillar.transform.position;
                    pullPoint = t.PullPoint.transform.position;
                    deltaAngle = 0;
                    deltaAngleAction = 0;
                    lastDirToPillar = pillar - transform.position;
                }

            }
            Vector3 dir = GetHeading(pullPoint);

            holdTimer += Time.deltaTime;
            if (holdTimer >= thrustTime && !actionPerformed)
            {
                rb.AddForce(dir.normalized * Acceleration, ForceMode.Acceleration);
                GrappleObject.PointAt(pullPoint);
            }
        }
        else
        {
            if (pressed)
            {
                if (holdTimer < thrustTime)
                    print("press");
                holdTimer = 0;
                deltaAngle = 0;
                deltaAngleAction = 0;
                pressed = false;
                actionPerformed = false;
                GrappleObject.Retract();
            }
        }
    }

    private void ActionCheck(Tile t)
    {
        if (!actionPerformed && holdTimer >= decayTime)
        {

            Vector3 toPillar= pullPoint - transform.position;
            float angle= Vector3.Angle(lastDirToPillar, toPillar);
            deltaAngle += angle;
            lastDirToPillar = toPillar;

            if (deltaAngle > 480)
            {
                
                rb.AddForce(rb.velocity * -Acceleration/4, ForceMode.Acceleration);
                if (toPillar.magnitude <= 1)
                {
                    rb.AddForce(toPillar * Acceleration, ForceMode.Acceleration);
                    deltaAngleAction += angle;
                    if (deltaAngleAction > 360)
                    {
                        print("player performed action");
                        actionPerformed = true;
                        t.Action(this);
                    }
                }
                else
                    rb.AddForce(toPillar.normalized * Acceleration / 2, ForceMode.Acceleration);
                
            }


            
        }
    }

    private void BounceCheck(Tile t)
    {
        
        if (t != currentTile)
        {
            //LevelController.MapLocation(gameObject, out x1, out z1);
            //print(" moved to Tile " + x1 + " " + y1+" s:"+t.StackSize);

            if (!Safe(t))
            {

                int x=1, z=1;
                Vector3 dis = transform.position - currentTile.transform.position;
                if (dis.x > tileWidth)
                {
                    if (!Safe(Level.MapTile(tileX + 1, tileZ)))
                    {
                        if (rb.velocity.x > 0)
                            x = -1;
                    }
                }
                else if (dis.x < -tileWidth)
                {
                    if (!Safe(Level.MapTile(tileX - 1, tileZ)))
                    {
                        if (rb.velocity.x < 0)
                            x = -1;
                    }
                }

                if (dis.z > tileWidth)
                {
                    if (!Safe(Level.MapTile(tileX, tileZ + 1)))
                    {
                        if (rb.velocity.z > 0)
                            z = -1;
                    }
                }
                else if (dis.z < -tileWidth)
                {
                    if (!Safe(Level.MapTile(tileX, tileZ - 1)))
                    {
                        if (rb.velocity.z < 0)
                            z = -1;
                    }
                }

                rb.velocity = new Vector3(rb.velocity.x * x, 0, rb.velocity.z * z);
            }
            else
            {
                wasSafe = true;
                if(currentTile!=null)
                    currentTile.OnTouchLeft(this);
                t.OnTouchBegin(this);
                previousTile = currentTile;
                currentTile = t;
                LevelController.MapLocation(gameObject, out this.tileX, out tileZ);
            }
        }

        return;
    }

    private Vector3 GetHeading(Vector3 pillarPos)
    {
        Vector3 sideOfPillar;
        //if (rotationDirection != 0)
        //{
        //    Vector3 toPillar = pillarPos - transform.position;
        //    sideOfPillar = new Vector3(toPillar.z, 0, -toPillar.x);
        //    sideOfPillar = sideOfPillar.normalized * .2f * rotationDirection;
        //}
       //else //if(rb.velocity.magnitude<.001)
        {
            Vector3 toPillar = pillarPos - transform.position;
            sideOfPillar = new Vector3(toPillar.z, 0, -toPillar.x);
            sideOfPillar = sideOfPillar.normalized * rotationRadius;
            rotationDirection = 1;
        }
        //else
        //{
        //    Vector3 toPillar = pillarPos - transform.position;
        //    sideOfPillar = new Vector3(toPillar.z, 0, -toPillar.x);

        //    float headingLeftOrRightOfPillar = Mathf.Sign(Vector3.Cross(toPillar, rb.velocity).y);
        //    rotationDirection = headingLeftOrRightOfPillar;

        //    sideOfPillar = sideOfPillar.normalized * .2f * rotationDirection*rotationDirection;

        //}

        Vector3 offsetTargetPos = new Vector3(pillarPos.x + sideOfPillar.x, 0, pillarPos.z + sideOfPillar.z);
        Vector3 heading = offsetTargetPos - transform.position;
        heading.Normalize();
        return new Vector3(heading.x, 0, heading.z);
    }

    private bool Safe(Tile t)
    {
        return t.StackSize > 0;
    }
}
