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
    public Grapple AutoGrapple;
    public Inventory Inventory;
    public Camera Camera;
    public GameObject DeathMessage;

    public TextMeshProUGUI ActionHUD;

    public static bool stopInput = false;


    private bool buttonPressed = false, actionPerformed = false, Launching = false, launched = false, dead=false;
    private Vector3 pillar, pullPoint;

    private float tileWidth = 4.5f;
    private float rotationRadius = 1f;
    private float holdTimer = 0, lastAngle=0, deltaAngle, deltaAngleAction;
    private const float thrustTime = .1f, decayTime = .5f;

    private Vector3 lastDirToPillar, heading, lastPos;

    private Tile currentTile,previousTile, previousEmbankment;

    private int tileX, tileZ;

    Rigidbody rb;

    private delegate Vector3 Heading(Vector3 pullPoint);
    private Heading GetHeading;

    private void Awake()
    {
        GetHeading = GetHeadingClckwise;
        heading = Vector3.zero;
        stopInput = false;
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        currentTile = Level.MapTile(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        FaceForward();

        Tile t = Level.MapTile(gameObject);
        t.OnTouchUpdate(this);
        ThrustCheck();
        ActionCheck(t);
        PlaceTileIntoVoid(t);
        if (!BounceCheck(t))
            JumpCheck(t);

        FallenCheck();

    }

    private void FaceForward()
    {
        if (stopInput)
        {
            return;
        }

        if (rb.velocity != Vector3.zero)
        {
            Vector3 face = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (face != Vector3.zero && face.magnitude > .05f)
            {
                heading = face;
                if(buttonPressed)
                    transform.forward = face.normalized;
                return;
            }
        }
        heading = Vector3.zero;
    }

    public Vector3 GetLookAhead()
    {
        return heading;
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
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(loc.x+3, loc.y + height, loc.z+3);
    }

    public void SetDirection(Vector3 dir)
    {
        if (dir == Vector3.zero)
            return;

        heading = Vector3.zero;
        transform.forward = dir.normalized;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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

                    GetHeading = CalculateHeading(pullPoint);


                    rb.AddForce(GetHeading(pullPoint).normalized * Acceleration*4, ForceMode.Acceleration);
                }

            }
            Vector3 dir = GetHeading(pullPoint);

            holdTimer += Time.deltaTime;
            if (holdTimer >= thrustTime && !actionPerformed)
            {
                rb.AddForce(dir.normalized * Acceleration, ForceMode.Acceleration);
                GrappleObject.PointAt(pullPoint);
                AutoGrapple.Retract();
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
                if (deltaAngle> 480*1.5)//(toPillar.magnitude <= 1)
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
        if (dead)
            return false;

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

                rb.velocity = new Vector3(rb.velocity.x * x, rb.velocity.y, rb.velocity.z * z);

                if( x == -1 || z == -1)
                {
                    transform.position = lastPos;
                    //rb.velocity = new Vector3(rb.velocity.x * x, 0, rb.velocity.z * z);
                    StartCoroutine(TempPullAnimate(.1f));
                    return true;
                }
                else
                {
                    rb.velocity = new Vector3(-rb.velocity.x, rb.velocity.y, -rb.velocity.z);
                    print("not safe but didn't deflect");
                    transform.position = lastPos;
                    //rb.velocity = new Vector3(rb.velocity.x * x, 0, rb.velocity.z * z);
                    StartCoroutine(TempPullAnimate(.1f));
                }
                return false;
                
            }
            else
            {
                lastPos = transform.position;
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
        else if (Safe(t))
        {
            lastPos = transform.position;
        }

        return false;
    }

    private IEnumerator TempPullAnimate(float seconds)
    {
        float pooptimer = 0;
        while (pooptimer < seconds)
        {
            AutoGrapple.PointAt(pullPoint);
            pooptimer += Time.deltaTime;
            yield return null;
        }
        AutoGrapple.Retract();
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
        if (t.Static)
            return;
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

    /// <summary>
    /// Figure out which side of a pillar to rotate around based
    /// either on velocity, or character direction
    /// </summary>
    /// <param name="pillarPos"></param>
    private Heading CalculateHeading(Vector3 pillarPos)
    {
        Vector3 heading = rb.velocity;
        if (heading == Vector3.zero)
            heading = transform.forward;

        Vector3 toPillar = pillarPos - transform.position;

        if (Mathf.Sign(Vector3.Cross(toPillar, heading).y) < 0)
            return GetHeadingClckwise;
        else
            return GetHeadingCntrClckwise;
    }

    /// <summary>
    /// rotate around a pillar counter clockwise
    /// </summary>
    /// <param name="pillarPos"></param>
    /// <returns></returns>
    private Vector3 GetHeadingCntrClckwise(Vector3 pillarPos)
    {
        Vector3 sideOfPillar;
        Vector3 toPillar = pillarPos - transform.position;
        sideOfPillar = new Vector3(toPillar.z, 0, -toPillar.x);
        sideOfPillar = sideOfPillar.normalized * rotationRadius;
        

        Vector3 offsetTargetPos = new Vector3(pillarPos.x + sideOfPillar.x, 0, pillarPos.z + sideOfPillar.z);
        Vector3 heading = offsetTargetPos - transform.position;
        heading.Normalize();
        return new Vector3(heading.x, 0, heading.z);
    }

    /// <summary>
    /// rotate around a pillar clockwise
    /// </summary>
    /// <param name="pillarPos"></param>
    /// <returns></returns>
    private Vector3 GetHeadingClckwise(Vector3 pillarPos)
    {
        Vector3 sideOfPillar;
        Vector3 toPillar = pillarPos - transform.position;
        sideOfPillar = new Vector3(-toPillar.z, 0, toPillar.x);
        sideOfPillar = sideOfPillar.normalized * rotationRadius;
        

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

    public void Respawn()
    {
        dead = false;
        DeathMessage.SetActive(false);
        Warp(5, previousEmbankment.PullPoint.transform.position);
    }
}
