using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Tile TileTemplate;
    public Tile Embankment;
    public Tile Wall;

    public LevelConfig CurrentLevel;

    public float tileSize = 10;

    public WorkerBFS[] WorkerGoals;

    public BomberSpawner[] bombers;

    private static float tileSizeStat;
    private static Tile nullTile;
    private static int length, width;

    private Tile[][] map;

    private static float ox, oy;

    private delegate void UpdateMap(Tile t);
    UpdateMap runBFS;

    // Start is called before the first frame update
    void Awake()
    {
        ox = transform.position.x;
        oy = transform.position.z;
        tileSizeStat = tileSize;
        //GenerateLevel(7, 5);
        //AddEmbankement();

        runBFS = V;

        GenerateLevel(CurrentLevel.Length, CurrentLevel.Width);
        AddStartTiles(CurrentLevel.Tiles);

        InitNPCPaths();
        runBFS = SetNPCPaths;

        nullTile = Instantiate(TileTemplate);
        nullTile.name = "nullTile";

    }

    private void GenerateLevel(int length, int width)
    {
        ResetMap(length, width);
    }

    private void ResetMap(int length, int width)
    {
        map = new Tile[length + 2][];
        for (int i = 0; i < length + 2; ++i)
            map[i] = new Tile[width + 2];

        for (int x = 1; x <= length; ++x)
            for (int y = 1; y <= width; ++y)
            {
                GameObject g = Instantiate(TileTemplate.gameObject);
                g.name = "Tile (" + x + "," + y + ")";
                Tile t = g.GetComponent<Tile>();
                t.Level = this;
                map[x][y] = t;
                t.SetPos(x, y);
            }
        LevelController.length = length;
        LevelController.width = width;
    }

    private void AddStartTiles(LevelConfig.TileConfig[] tiles)
    {
        foreach (LevelConfig.TileConfig tile in tiles)
        {
            map[tile.x][tile.y].Add(Inventory.Multi(Inventory.CloneTile(tile.TileTemplate, true)));
            map[tile.x][tile.y].AddAction(tile.Action);
        }
    }

    public bool LegalSpot(Tile t)
    {
        int x, y;
        MapLocation(t.gameObject, out x, out y);
        if (x == 0 || x == length + 1)
            return false;
        if (y == 0 || y == width + 1)
            return false;
        return !t.Static;
    }

    private void AddEmbankement()
    {
        for (int i = 1; i <= width; ++i)
        {
            map[1][i].Add(Inventory.Multi(Inventory.CloneTile(Embankment, true)));
        }
        for (int i = 1; i <= width; ++i)
        {
            map[length][i].Add(Inventory.Multi(Inventory.CloneTile(Embankment, true)));
        }
    }

    //private void UpdateTile(Tile tile, int x, int y)
    //{
    //    if (!(x >= 0 && x < length && y >= 0 && y < width))
    //        return;

    //    print("Level Controller add tile " + x + " " + y);
    //}

    public static Vector3 PhysicalLocation(int x, int y)
    {
        return new Vector3((x) * tileSizeStat + tileSizeStat * .5f + ox, 0, (y) * tileSizeStat + tileSizeStat * .5f + oy);
    }

    public static void MapLocation(GameObject g, out int x, out int y)
    {
        Vector3 pos = g.transform.position;
        x = (int)((pos.x - ox) / tileSizeStat);
        y = (int)((pos.z - oy) / tileSizeStat);
    }

    public Tile MapTile(GameObject g)
    {
        int x, y;
        MapLocation(g, out x, out y);
        if (!(x >= 1 && x <= length && y >= 1 && y <= width))
            return nullTile;

        return map[x][y];
    }
    public Tile MapTile(int x, int y)
    {
        if (!(x >= 1 && x <= length && y >= 1 && y <= width))
            return nullTile;
        return map[x][y];
    }

    public void V(Tile t) {}

    public void SetNPCPaths(Tile t)
    {
        foreach (WorkerBFS goal in WorkerGoals)
            goal.UpdatePaths(map, t.x, t.y);
        foreach (BomberSpawner b in bombers)
            b.UpdateTile(t.x, t.y);
    }
    public void InitNPCPaths()
    {
        foreach (WorkerBFS goal in WorkerGoals)
            goal.Set(map, this);
        foreach (BomberSpawner b in bombers)
            b.Init(map, this);
    }

    public void TileChanged(Tile t)
    {
        runBFS(t);
    }
}
