using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Tile TileTemplate;
    public Tile Embankment;
    public Tile Wall;

    public float tileSize = 10;
    private static float tileSizeStat;
    private static Tile nullTile;
    private static int length, width;

    private static Tile[][] map;

    // Start is called before the first frame update
    void Start()
    {
        tileSizeStat = tileSize;
        GenerateLevel(7, 5);
        AddEmbankement();

        nullTile = Instantiate(TileTemplate);
        nullTile.name = "nullTile";
    }

    private void GenerateLevel(int length, int width)
    {
        ResetMap(length, width);
    }

    private void ResetMap(int length, int width)
    {
        map = new Tile[length+2][];
        for (int i = 0; i < length+2; ++i)
            map[i] = new Tile[width+2];

        for (int x = 1; x <= length; ++x)
            for (int y = 1; y <= width; ++y)
            {
                GameObject g = Instantiate(TileTemplate.gameObject);
                g.name = "Tile (" + x + "," + y + ")";
                Tile t= g.GetComponent<Tile>();
                map[x][y] = t;
                t.SetPos(x, y);
            }
        LevelController.length = length;
        LevelController.width = width;
    }

    private void AddEmbankement()
    {
        for (int i = 1; i <= width; ++i)
        {
            map[1][i].Add(Embankment);
        }
        for (int i = 1; i <= width; ++i)
        {
            map[length][i].Add(Embankment);
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
        return new Vector3(x * tileSizeStat+ tileSizeStat*.5f, 0, y * tileSizeStat + tileSizeStat *.5f);
    }

    public static void MapLocation(GameObject g, out int x, out int y)
    {
        Vector3 pos = g.transform.position;
        x = (int)((pos.x) / tileSizeStat);
        y = (int)((pos.z) / tileSizeStat);
    }

    public static Tile MapTile(GameObject g)
    {
        int x, y;
        MapLocation(g, out x, out y);
        if (!(x >= 1 && x <= length && y >= 1 && y <= width))
            return nullTile;
        //print("providing pillar " + x + " " + y);
        return map[x][y];
    }
    public static Tile MapTile(int x, int y)
    {
        if (!(x >= 1 && x <= length && y >= 1 && y <= width))
            return nullTile;
        //print("providing pillar " + x + " " + y);
        return map[x][y];
    }
}
