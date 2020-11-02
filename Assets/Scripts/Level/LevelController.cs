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

        runBFS = V;

        GenerateLevel(CurrentLevel.Length, CurrentLevel.Width);
        AddStartTiles(CurrentLevel.Read());

        CleanControllers();
        InitControllers();
        runBFS = UpdateControllers;

        nullTile = Instantiate(TileTemplate);
        nullTile.name = "nullTile";

    }

    public void InitWinCondition(CheckWin win)
    {
        win.Init(map, this);
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

    private void AddStartTiles(IEnumerable<TileHolder.TileConfig> tiles)
    {
        foreach (TileHolder.TileConfig tile in tiles)
        {
            if (!inBounds(tile.pos.x, tile.pos.y))
            {
                Debug.LogWarning("not placing tile at " + tile.pos.x + " " + tile.pos.y);
                continue;
            }
            if(tile.TileTemplate!=null)
                map[tile.pos.x][tile.pos.y].Add(Inventory.Multi(Inventory.CloneTile(tile.TileTemplate, true)));
            map[tile.pos.x][tile.pos.y].AddAction(tile.Action);
        }
    }

    private bool inBounds(int x, int y)
    {
        if (x <= 0 || x >= length + 1)
            return false;
        if (y <= 0 || y >= width + 1)
            return false;
        return true;
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

    public void UpdateControllers(Tile t)
    {
        foreach (WorkerBFS goal in WorkerGoals)
            goal.UpdatePaths(map, t.x, t.y);
        foreach (BomberSpawner b in bombers)
            b.UpdateTile(t.x, t.y);
    }

    public void CleanControllers()
    {
        WorkerGoals = Clean(WorkerGoals);
        bombers = Clean(bombers);
    }

    public static T[] Clean<T>(T [] l)
    {
        List<T> b = new List<T>();
        foreach (T g in l)
            if (g != null)
                b.Add(g);
        T[] r = new T[b.Count];
        int i = 0;
        foreach (T g in b)
            r[i++] = g;
        return r;
    }


    public void InitControllers()
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
