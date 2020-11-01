using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup : TileHolder
{


    public Vector2Int max;

    [SerializeField]
    private TileConfig [] Tiles;

    public override Vector2Int Max()
    {
        return max;
    }

    public override void Max(Vector2Int max)
    {
        this.max = max;
    }

    public override IEnumerable<TileConfig> Read()
    {
        foreach (TileConfig t in Tiles)
            yield return t;
    }
}
