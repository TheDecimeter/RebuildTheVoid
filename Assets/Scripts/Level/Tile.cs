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

    private Stack<Tile> Tiles = new Stack<Tile>();
    private List<Tile> Addons = new List<Tile>();

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
        //Floor.transform.localScale = new Vector3(10, h, 10);
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

    public void Add(IEnumerable<Tile> tiles)
    {
        if (Static)
            return;

        foreach (Tile t in tiles)
        {
            if (t.Addon)
            {
                t.gameObject.SetActive(true);
                AddAddon(t);
            }
            else
                AddSolidTile(t);
        }
    }

    //private void Add(Tile tile)
    //{
    //    if (Static)
    //    {
    //        return;
    //    }

    //    if (!tile.Addon)
    //    {
    //        if (StackSize == 0)
    //        {
    //            Floor.gameObject.SetActive(true);
    //            Pillar.gameObject.SetActive(true);
    //        }
    //        StackSize++;
    //    }

    //    tile.transform.position= LevelController.PhysicalLocation(x, y);
    //    tile.transform.SetParent(Pillar.transform);
    //    Tiles.Push(tile);

    //    UpdateHeight();
    //    Static = tile.Static;
    //}

    private void AddAddon(Tile tile)
    {
        tile.transform.position = LevelController.PhysicalLocation(x, y);
        tile.transform.SetParent(Pillar.transform);
        Addons.Add(tile);
    }
    private void AddSolidTile(Tile tile)
    {
        if (StackSize == 0)
        {
            Floor.gameObject.SetActive(true);
            Pillar.gameObject.SetActive(true);
        }
        StackSize++;
        
        tile.transform.position = LevelController.PhysicalLocation(x, y);
        tile.transform.SetParent(Pillar.transform);
        Tiles.Push(tile);

        UpdateHeight();
        Static = tile.Static;
    }

    public void AddAction(TileAction action)
    {
        if (action == null)
            return;
        if (this.action != null)
            Destroy(this.action.gameObject);

        print("setting action for " + x + " " + y);

        GameObject g = Instantiate(action.gameObject);
        
        this.action = g.GetComponent<TileAction>();
        this.action.Init(this);
    }
    public bool Action(PlayerMovement player)
    {
        if (action == null)
        {
            print("no action for " + x + " " + y);
            return false;
        }
        print("yes action for " + x + " " + y);
        action.Action(player);
        return true;
    }

    public IEnumerable<Tile> GetTopLayer()
    {
        if (Static)
            return new List<Tile>();
        if (Addons.Count > 0)
        {
            List<Tile> r = Addons;
            Addons = new List<Tile>();
            return r;
        }
        print("getting top of tile ss:" + StackSize + " T.c" + Tiles.Count);
        Tile t = Tiles.Pop();
        StackSize--;

        if (StackSize == 0)
        {
            Pillar.gameObject.SetActive(false);
            Floor.gameObject.SetActive(false);
        }
        else
            UpdateHeight();

        return Inventory.Multi(t);
    }

    ~Tile()
    {
        while (Tiles.Count > 0)
            Destroy(Tiles.Pop());
    }
}
