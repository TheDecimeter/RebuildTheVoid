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

    private Stack<Tile> additions = new Stack<Tile>();

    public void SetPos(int x, int y)
    {
        this.transform.position = LevelController.PhysicalLocation(x, y);
        this.x = x;
        this.y = y;
    }

    public void UpdateHeight()
    {
        float h = StackSize * tileHeight;
        //Vector3 pos = new Vector3(Pillar.transform.position.x, h, Pillar.transform.position.z);
        Floor.transform.localScale = new Vector3(10, h, 10);
        Floor.transform.position = new Vector3(Floor.transform.position.x, h, Floor.transform.position.z);

        Pillar.transform.position = new Vector3(Pillar.transform.position.x, h, Pillar.transform.position.z);

        //foreach (Tile t in additions)
        //{
        //    t.transform.position = pos;
        //}
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
            Debug.LogError("Tile.cs, can't add to static tile");
            return;
        }

        if (!tile.Addon || StackSize==0)
        {
            if (StackSize == 0)
            {
                Floor.gameObject.SetActive(true);
                Pillar.gameObject.SetActive(true);
            }
            StackSize++;
            print("adding stack size for " + x + " " + y+ "  stack size ="+StackSize);
        }

        //GameObject g = Instantiate(tile.gameObject);
        //g.name = "addon for (" + x + "," + y + ")";
        //g.transform.position = LevelController.PhysicalLocation(x, y);
        //g.transform.SetParent(Pillar.transform);

        tile.transform.position= LevelController.PhysicalLocation(x, y);
        tile.transform.SetParent(Pillar.transform);
        additions.Push(tile);

        UpdateHeight();
        Static = tile.Static;
    }

    public void AddAction(TileAction action)
    {
        if (action == null)
            return;
        if (this.action != null)
            Destroy(this.action.gameObject);

        print("adding action " + x + " " + y);

        GameObject g = Instantiate(action.gameObject);
        g.name = "Action for Tile(" + x + "," + y + ")";
        
        this.action = g.GetComponent<TileAction>();
        this.action.Init(this);
    }
    public bool Action(PlayerMovement player)
    {
        if (action == null)
            return false;
        action.Action(player);
        return true;

        //if (Inventory.IsEmpty())
        //{
        //    Inventory.TryAddItem(WrapTopLayer());
        //    //see if player needs to be kicked back
        //}
    }

    ~Tile()
    {
        while (additions.Count > 0)
            Destroy(additions.Pop());
    }
}
