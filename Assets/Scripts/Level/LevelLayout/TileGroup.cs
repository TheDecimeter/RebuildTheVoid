using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup : MonoBehaviour
{
    [System.Serializable]
    public class TileConfig : System.Object
    {
        public int x;
        public int y;
        public Tile TileTemplate;
        public TileAction Action;

        public void X(int x)
        {
            this.x = x;
        }
        public void Y(int y)
        {
            this.y = y;
        }
    }

    public int maxX;
    public int maxY;
    public TileConfig [] Tiles;
}
