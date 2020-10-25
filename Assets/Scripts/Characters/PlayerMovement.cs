using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Acceleration;
    private bool pressed = false, wasSafe=true;
    private Vector3 pillar;

    private float tileWidth = 4.5f;
    private float rotationRadius = 1f;
    private float rotationDirection = 0;

    private Tile lastTile;

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
        if (Input.anyKey)
        {
            if (!pressed)
            {
                pressed = true;
                Tile t = LevelController.MapTile(gameObject);
                if (Safe(t))
                    pillar = t.pillar.transform.position;

            }
            Vector3 dir = GetHeading(pillar);

            rb.AddForce(dir.normalized * Acceleration, ForceMode.Acceleration);
        }
        else
            pressed = false;

        BounceCheck();
    }

    private void BounceCheck()
    {
        Tile t = LevelController.MapTile(gameObject);

        if (t != lastTile)
        {
            //LevelController.MapLocation(gameObject, out x1, out z1);
            //print(" moved to Tile " + x1 + " " + y1+" s:"+t.StackSize);

            if (!Safe(t))
            {
                print("course correction");

                int x=1, z=1;
                Vector3 dis = transform.position - lastTile.transform.position;
                if (dis.x > tileWidth)
                {
                    if (!Safe(LevelController.MapTile(tileX + 1, tileZ)))
                    {
                        if (rb.velocity.x > 0)
                            x = -1;
                    }
                }
                else if (dis.x < tileWidth)
                {
                    if (!Safe(LevelController.MapTile(tileX - 1, tileZ)))
                    {
                        if (rb.velocity.x < 0)
                            x = -1;
                    }
                }

                if (dis.z > tileWidth)
                {
                    if (!Safe(LevelController.MapTile(tileX, tileZ + 1)))
                    {
                        if (rb.velocity.z > 0)
                            z = -1;
                    }
                }
                else if (dis.z < tileWidth)
                {
                    if (!Safe(LevelController.MapTile(tileX, tileZ - 1)))
                    {
                        if (rb.velocity.z < 0)
                            z = -1;
                    }
                }

                rb.velocity = new Vector3(rb.velocity.x * x, 0, rb.velocity.z * z);


                    //dis = dis.normalized * tileWidth;
                    //Vector3 inBoundsX = Vector3.Project(dis, Vector3.right);
                    //Vector3 inBoundsZ = Vector3.Project(dis, Vector3.forward);
                    //Vector3 inBoundsPlaneCoord = lastTile.transform.position + inBoundsX + inBoundsZ;
                    //transform.position = new Vector3(inBoundsPlaneCoord.x,transform.position.y,inBoundsPlaneCoord.z);
                
            }
            else
            {
                wasSafe = true;
                lastTile = t;
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
