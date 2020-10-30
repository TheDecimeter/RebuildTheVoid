using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string Name;
    public int Cost;
    public GameObject Floor;
    public GameObject Pillar;
    public GameObject PullPoint;
    public TileText Text;
    public bool Addon = false;
    public bool Static=false;


    private TileAction action;

    private int _s = 0;
    public int StackSize {
        get
        {
            return _s;
        }
        protected set
        {
            //print("set stack at " + x + " " + y + value);
            _s = value;

        }
    }

    private int tileHeight = 1, x, y;

    private List<Tile> additions = new List<Tile>();

    public void SetPos(int x, int y)
    {
        this.transform.position = LevelController.PhysicalLocation(x, y);
        this.x = x;
        this.y = y;
    }

    public void UpdateHeight()
    {
        Vector3 pos = new Vector3(Pillar.transform.position.x, StackSize * tileHeight, Pillar.transform.position.z);
        Floor.transform.localScale = new Vector3(10, tileHeight * StackSize, 10);
        Floor.transform.position = new Vector3(Floor.transform.position.x, StackSize/2 * tileHeight, Floor.transform.position.z);

        Pillar.transform.position = pos;

        foreach (Tile t in additions)
        {
            t.transform.position = pos;
        }
    }

    public void OnTouchUpdate(PlayerMovement player)
    {
        Text.LookAt(player.Camera.transform);
    }
    public void OnTouchLeft(PlayerMovement player)
    {
        if (action != null)
            action.OnTouchLeft(player);
    }
    public void OnTouchBegin(PlayerMovement player)
    {
        if (action != null)
            action.OnTouchBegin(player);
    }

    public void Add(Tile tile)
    {
        if (Static)
        {
            print("Tile, can't add to static tile");
            return;
        }

        if (tile.Addon)
        {
            if (StackSize == 0)
            {
                Floor.gameObject.SetActive(true);
                Pillar.gameObject.SetActive(true);
            }
            StackSize++;
            print("adding stack size for " + x + " " + y+ "  stack size ="+StackSize);
        }
        GameObject g = Instantiate(tile.gameObject);
        g.transform.position = LevelController.PhysicalLocation(x, y);
        additions.Add(g.GetComponent<Tile>());

        UpdateHeight();
        Static = tile.Static;
    }

    public void AddAction(TileAction action)
    {
        if (action == null)
            return;
        print("adding action " + x + " " + y);

        GameObject g = Instantiate(action.gameObject);
        g.name = "Action for Tile(" + x + "," + y + ")";

        //this.action = gameObject.AddComponent<TileAction>();
        //this.action.Copy(action);
        this.action = g.GetComponent<TileAction>();
        this.action.Init(this);
    }
    public void Action(PlayerMovement player)
    {
        if(action==null)
        {
            print("action fired, but tile has no action");
            return;
        }

        action.Action(player);

    }
}
