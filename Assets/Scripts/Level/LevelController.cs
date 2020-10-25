using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Tile TileTemplate;
    public Tile Embankment;

    public float tileSize = 10;
    private static float tileSizeStat;

    Tile[][] map;

    // Start is called before the first frame update
    void Start()
    {
        tileSizeStat = tileSize;
        GenerateLevel(7, 5);
        AddEmbankement();
    }

    private void GenerateLevel(int length, int width)
    {
        ResetMap(length, width);
    }

    private void ResetMap(int length, int width)
    {
        map = new Tile[length][];
        for (int i = 0; i < length; ++i)
            map[i] = new Tile[width];

        for (int x = 0; x < length; ++x)
            for (int y = 0; y < width; ++y)
            {
                GameObject g = Instantiate(TileTemplate.gameObject);
                Tile t= g.GetComponent<Tile>();
                map[x][y] = t;
                t.SetPos(x, y);
            }
    }

    private void AddEmbankement()
    {
        for (int i = 0; i < map[0].Length; ++i)
        {
            map[0][i].Add(Embankment);
        }
        for (int i = 0; i < map[0].Length; ++i)
        {
            map[map.Length-1][i].Add(Embankment);
        }
    }

    private void UpdateTile(Tile tile, int x, int y)
    {
        if (!(x >= 0 && x < map.Length && y >= 0 && y < map[0].Length))
            return;

        print("Level Controller add tile " + x + " " + y);

    }

    public static Vector3 PhysicalLocation(int x, int y)
    {
        return new Vector3(x * tileSizeStat, 0, y * tileSizeStat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
