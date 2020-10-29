using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    public int Length;
    public int Width;

    [System.Serializable]
    public struct TileConfig
    {
        public int x;
        public int y;
        public Tile TileTemplate;
        public TileAction Action;
    }
    public TileConfig [] Tiles;
}
