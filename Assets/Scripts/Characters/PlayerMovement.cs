using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : Character
{
    public LevelController Level;
    public float Acceleration;
    public float LaunchPower;
    public Grapple GrappleObject;
    public Inventory Inventory;
    public Camera Camera;
    public GameObject DeathMessage;

    public TextMeshProUGUI ActionHUD;

    public static bool stopInput = false;


    private bool buttonPressed = false, actionPerformed = false, Launching = false, launched = false, dead=false;
    private Vector3 pillar, pullPoint;

    private float tileWidth = 4.5f;
    private float rotationRadius = 1f;
    private float rotationDirection = 0;
    private float holdTimer = 0, lastAngle=0, deltaAngle, deltaAngleAction;
    private const float thrustTime = .1f, decayTime = .5f;

    private Vector3 lastDirToPillar;

    private Tile currentTile,previousTile, previousEmbankment;

    private int tileX, tileZ;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        stopInput = false;
        rb = GetComponent<Rigidbody>();
        currentTile = Level.MapTile(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Tile t = Level.MapTile(gameObject);
        t.OnTouchUpdate(this);
        ThrustCheck();
        ActionCheck(t);
        PlaceTileIntoVoid(t);
        if (!BounceCheck(t))
            JumpCheck(t);

        FallenCheck();
    }

    private void FallenCheck()
    {
        if (dead)
            return;
        if (transform.position.y < -5)
        {
            if (Solid(currentTile))
                Warp(5, currentTile.PullPoint.transform.position);
            else if (Solid(previousTile))
                Warp(5, previousTile.PullPoint.transform.position);
            else
                Warp(5, previousEmbankment.PullPoint.transform.position);
        }
    }

    private void Warp(float height, Vector3 loc)
    {
        transform.position = new Vector3(loc.x, loc.y + height, loc.z);
    }

    private void ThrustCheck()
    {
        if (stopInput)
            return;

        if (Input.anyKey)
        {
            if (!buttonPressed)
            {
                buttonPressed = true;
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
            if (buttonPressed)
            {
                if (holdTimer < thrustTime)
                    print("press");
                holdTimer = 0;
                deltaAngle = 0;
                deltaAngleAction = 0;
                buttonPressed = false;
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
                        //print("player performed action");
                        actionPerformed = true;
                        if(!t.Action(this)){
                            if (Inventory.IsEmpty())
                                GetTile(t);
                            else
                                PlaceTileOnTop(t);
                        }
                    }
                }
                else
                    rb.AddForce(toPillar.normalized * Acceleration / 2, ForceMode.Acceleration);
                
            }


            
        }
    }

    public void UpdateActionMessage(string msg)
    {
        if (msg == null)
        {
            SetInteractionActionMessage();
        }
        else
            ActionHUD.text = msg;

    }

    private void SetInteractionActionMessage()
    {
        if (currentTile.Static)
        {
            //print("action = nothing");
            ActionHUD.text = "Nothing";
        }
        else
        {
            if (Inventory.IsEmpty())
            {
                //print("action = Get Tile");
                ActionHUD.text = "Get Tile";
            }
            else
            {
                //print("action = Stack Tile");
                ActionHUD.text = "Stack Tile";
            }
        }
    }


    private bool JumpCheck(Tile t)
    {
        float rbThresh = .01f;

        if (launched)
        {
            if (rb.velocity.y > -rbThresh)
                return false;

            launched = false;
        }

        //if (buttonPressed)
        //    return false;

        float tileCenter = tileWidth * 0.65f;
        Vector3 dis = transform.position - currentTile.transform.position;
        float launch = LaunchPower*1.5f;
        if (dis.x > tileCenter)
        {
            if (StepUp(Level.MapTile(tileX + 1, tileZ)))
            {
                if (rb.velocity.x > rbThresh)
                {
                    //LaunchToward(Level.MapTile(tileX + 1, tileZ));
                    //return true;
                    
                    velocityChange(new Vector3(rb.velocity.x, launch, rb.velocity.z));
                    launched = true;
                    return true;
                }
            }
        }
        else if (dis.x < -tileCenter)
        {
            if (StepUp(Level.MapTile(tileX - 1, tileZ)))
            {
                if (rb.velocity.x < -rbThresh)
                {
                    //LaunchToward(Level.MapTile(tileX - 1, tileZ));
                    //return true;
                    velocityChange(new Vector3(rb.velocity.x, launch, rb.velocity.z));
                    launched = true;
                    return true;
                }
            }
        }
        else if (dis.z > tileCenter)
        {
            if (StepUp(Level.MapTile(tileX, tileZ + 1)))
            {
                if (rb.velocity.z > rbThresh)
                {
                    //LaunchToward(Level.MapTile(tileX, tileZ + 1));
                    //return true;
                    velocityChange(new Vector3(rb.velocity.x, launch, rb.velocity.z));
                    launched = true;
                    return true;
                }
            }
        }
        else if (dis.z < -tileCenter)
        {
            if (StepUp(Level.MapTile(tileX, tileZ - 1)))
            {
                if (rb.velocity.z < -rbThresh)
                {
                    //LaunchToward(Level.MapTile(tileX, tileZ - 1));
                    //return true;
                    velocityChange(new Vector3(rb.velocity.x, launch, rb.velocity.z));
                    launched = true;
                    return true;
                }
            }
        }
        return false;
    }

    private void velocityChange(Vector3 vel)
    {
        rb.velocity = vel;
    }


    private bool BounceCheck(Tile t)
    {

        if (t != currentTile)
        {
            //LevelController.MapLocation(gameObject, out x1, out z1);
            //print(" moved to Tile " + x1 + " " + y1+" s:"+t.StackSize);

            if (!Safe(t))
            {
                int x = 1, z = 1;
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

                if( x == -1 || z == -1)
                {
                    //rb.velocity = new Vector3(rb.velocity.x * x, 0, rb.velocity.z * z);
                    StartCoroutine(TempPullAnimate(.1f));
                    return true;
                }
                return false;
                
            }
            else
            {
                currentTile.OnTouchLeft(this);
                previousTile = currentTile;
                currentTile = t;
                currentTile.OnTouchBegin(this);
                if (!buttonPressed)
                    pullPoint = t.PullPoint.transform.position;
                Launching = false;
                if (t.Static)
                    previousEmbankment = t;
                LevelController.MapLocation(gameObject, out this.tileX, out tileZ);
            }
        }

        return false;
    }

    private IEnumerator TempPullAnimate(float seconds)
    {
        float pooptimer = 0;
        while (pooptimer < seconds)
        {
            GrappleObject.PointAt(pullPoint);
            pooptimer += Time.deltaTime;
            yield return null;
        }
        GrappleObject.Retract();
    }

    private void GetTile(Tile t)
    {
        Inventory.TryAddItem(t.GetTopLayer());
        if (!Solid(t))
            LaunchHomewards();
        SetInteractionActionMessage();
    }
    private void PlaceTileOnTop(Tile t)
    {
        IEnumerable<Tile> tiles;
        if (Inventory.TryGetItem(out tiles))
            t.Add(tiles);
        SetInteractionActionMessage();
    }

    private void LaunchHomewards()
    {
        if (Solid(previousTile))
            LaunchToward(previousTile);
        else
            LaunchToward(previousEmbankment);
        
    }
    private void LaunchToward(Tile t)
    {
        pillar = t.Pillar.transform.position;
        pullPoint = t.PullPoint.transform.position;

        Vector3 dir = pullPoint - this.transform.position;
        dir.Normalize();
        rb.velocity = new Vector3(dir.x, 2, dir.z)*LaunchPower;
        Launching = true;
        StartCoroutine(TempPullAnimate(.3f));
    }

    private bool PlaceTileIntoVoid(Tile t)
    {
        if (buttonPressed)
            return false;
        if (t == currentTile || (t.StackSize >= currentTile.StackSize - 1 && t.StackSize != 0) || !Level.LegalSpot(t))
            return false;

        if (Inventory.TileIsAddon())
            return false;

        IEnumerable<Tile> item;
        if (!Inventory.TryGetItem(out item))
            return false;

        t.Add(item);

        SetInteractionActionMessage();
        return true;
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

    private bool Solid(Tile t)
    {
        return t.StackSize != 0;
    }

    private bool Safe(Tile t)
    {
        //return t.StackSize != 0;
        if (t.StackSize == 0)
            return false;
        if (currentTile == null || currentTile.StackSize == 0)
            return true;
        if (t.StackSize + 1 == currentTile.StackSize)
            return true;

        return WithinOneHeight(t);
    }

    private bool WithinOneHeight(Tile t)
    {
        return (currentTile.StackSize - 1 <= t.StackSize && t.StackSize <= currentTile.StackSize + 1);
    }

    private bool MoreThanAStepDown(Tile t)
    {
        return currentTile.StackSize - t.StackSize > 1;
    }

    private bool StepUp(Tile t)
    {
        //return t.StackSize > currentTile.StackSize;
        if (transform.position.y > t.PullPoint.transform.position.y)
            return false;
        return t.StackSize - currentTile.StackSize == 1;
    }

    public void Kill()
    {
        dead = true;
        DeathMessage.SetActive(true);
    }
}
