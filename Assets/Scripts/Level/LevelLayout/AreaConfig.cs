using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaConfig : MonoBehaviour
{
    public enum ReadOrder { flipXY, reverseX, reverseY, rotate90, rotate180, rotate270 }

    [System.Serializable]
    public struct Area
    {
        public int OffsetX;
        public int OffsetY;
        public ReadOrder[] ReadOrder;
        public TileGroup Group;
    }

    [SerializeField]
    private Area area;

    private delegate void manipulation(List<TileGroup.TileConfig> l, Area a, TileGroup g);

    /// <summary>
    /// Read the areas out with correct x and y
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TileGroup.TileConfig> Read()
    {
        List<TileGroup.TileConfig> l = new List<TileGroup.TileConfig>();
        foreach (TileGroup.TileConfig t in area.Group.Tiles)
            l.Add(t);

        foreach (ReadOrder ro in area.ReadOrder)
            GetManipulation(ro)(l, area, area.Group);

        ApplyOffset(l, area);


        return l;
    }

    private manipulation GetManipulation(ReadOrder order)
    {
        switch (order)
        {
            case ReadOrder.flipXY:
                return FlipXY;
            case ReadOrder.reverseX:
                return ReverseX;
            case ReadOrder.reverseY:
                return ReverseY;
            case ReadOrder.rotate180:
                return Rotate180;
            case ReadOrder.rotate90:
                return Rotate90;
            case ReadOrder.rotate270:
                return Rotate270;
            default:
                return Error;
        }
    }

    private void Error(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        Debug.LogError("trying to use an area manipulation which isn't written");
    }
    private void FlipXY(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        int tmp = g.maxX;
        g.maxX = g.maxY;
        g.maxY = g.maxX;

        foreach (TileGroup.TileConfig t in l)
        {
            tmp = t.x;
            t.X(t.y);
            t.Y(tmp);
        }
    }
    private void ReverseX(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        foreach (TileGroup.TileConfig t in l)
        {
            t.X(g.maxX-t.x);
        }
    }
    private void ReverseY(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        foreach (TileGroup.TileConfig t in l)
        {
            t.Y(g.maxY - t.y);
        }
    }
    private void Rotate90(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        ReverseX(l, a, g);
        FlipXY(l, a, g);
    }
    private void Rotate270(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        FlipXY(l, a, g);
        ReverseX(l, a, g);
    }
    private void Rotate180(List<TileGroup.TileConfig> l, Area a, TileGroup g)
    {
        ReverseX(l, a, g);
        ReverseY(l, a, g);
    }
    private void ApplyOffset(List<TileGroup.TileConfig> l, Area a)
    {
        foreach (TileGroup.TileConfig t in l)
        {
            t.Y(t.y + a.OffsetY);
            t.X(t.x + a.OffsetY);
        }


        foreach (TileGroup.TileConfig t in l)
            print("loc check " + t.y + " " + t.x);
    }
}
