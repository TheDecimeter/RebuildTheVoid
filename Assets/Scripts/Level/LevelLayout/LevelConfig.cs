using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    public int Length;
    public int Width;

    [SerializeField]
    private AreaConfig[] Areas;

    public IEnumerable<TileHolder.TileConfig> Read()
    {
        foreach (AreaConfig a in Areas)
            if(a!=null)
            foreach (TileHolder.TileConfig t in a.Read())
                yield return t;
    }
}
