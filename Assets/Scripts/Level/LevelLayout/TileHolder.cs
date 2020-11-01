using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileHolder : MonoBehaviour
{
    [System.Serializable]
    public class TileConfig : System.Object
    {
        public Vector2Int pos;
        public Tile TileTemplate;
        public TileAction Action;

        public void X(int x)
        {
            pos.x = x;
        }
        public void Y(int y)
        {
            pos.y = y;
        }
    }

    public abstract IEnumerable<TileConfig> Read();
    public abstract Vector2Int Max();
    public abstract void Max(Vector2Int max);
}
