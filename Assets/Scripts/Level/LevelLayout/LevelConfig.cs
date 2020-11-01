using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    public int Length;
    public int Width;

    [SerializeField]
    private AreaConfig[] Areas;

    public IEnumerable<TileGroup.TileConfig> Read()
    {
        foreach (AreaConfig a in Areas)
            foreach (TileGroup.TileConfig t in a.Read())
                yield return t;
    }
}
