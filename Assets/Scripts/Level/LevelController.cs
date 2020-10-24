using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Tile movableTileTemplate;
    public Tile embankmentTileTemplate;

    Tile[][] map;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void GenerateLevel(int length, int width)
    {
        map = new Tile[length][];
        for (int i = 0; i < length; ++i)
            map[i] = new Tile[width];
    }

    private void updateTile(Tile tile, int x, int y)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
