using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Tile tileTemplate;

    public float tileSize = 10;

    Tile[][] map;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel(7, 5);
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
                GameObject g = Instantiate(tileTemplate.gameObject);
                SetPhysicalLocation(g.transform, x, y);
                map[x][y] = g.GetComponent<Tile>();
            }
    }

    //private void AddEmbankement()
    //{
    //    for(int i=0; i<map[0].Length; ++i)
    //    {
    //        UpdateTile
    //    }
            
    //}

    private void UpdateTile(Tile tile, int x, int y)
    {
        print("Level Controller add tile " + x + " " + y);
        if (x >= 0 && x < map.Length && y >= 0 && y < map[0].Length)
        {
            if (map[x][y] == null)
            {
                GameObject g = Instantiate(tile.gameObject);
                SetPhysicalLocation(g.transform, x, y);
                map[x][y] = g.GetComponent<Tile>();
            }

        }
    }
    private void SetPhysicalLocation(Transform t, int x, int y)
    {
        t.position = new Vector3(x * tileSize, 0, y * tileSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
