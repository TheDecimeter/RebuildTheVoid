using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaConfig : TileHolder
{
    public enum ReadOrder { flipXY, reverseX, reverseY, rotate90, rotate180, rotate270 }

    [System.Serializable]
    public struct Area
    {
        public int OffsetX;
        public int OffsetY;
        public ReadOrder[] ReadOrder;
        public TileHolder [] Groups;
    }

    private Vector2Int max=Vector2Int.zero;

    [SerializeField]
    private Area area;

    private delegate void manipulation(List<TileHolder.TileConfig> l, Area a, TileHolder g);


    private void Start()
    {
        //foreach (TileHolder group in area.Groups)
        //{
        //    List<TileHolder.TileConfig> l = new List<TileHolder.TileConfig>();
        //    foreach (TileHolder.TileConfig t in group.Read())
        //        l.Add(new TileHolder.TileConfig(t));

        //    foreach (ReadOrder ro in area.ReadOrder)
        //        GetManipulation(ro)(l, area, group);

        //    ApplyOffset(l, area);
        //}
    }

    /// <summary>
    /// Read the areas out with correct x and y
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<TileHolder.TileConfig> Read()
    {
        foreach (TileHolder group in area.Groups)
        {
            List<TileHolder.TileConfig> l = new List<TileHolder.TileConfig>();
            foreach (TileHolder.TileConfig t in group.Read())
                l.Add(new TileHolder.TileConfig(t));

            foreach (ReadOrder ro in area.ReadOrder)
                GetManipulation(ro)(l, area, group);

            ApplyOffset(l, area);

            foreach (TileHolder.TileConfig t in l)
                yield return t;
        }
    }

    //private TileHolder.TileConfig New(TileHolder.TileConfig t)
    //{
    //    TileHolder.TileConfig r = new TileHolder.TileConfig();
    //    r.pos = new Vector2Int(t.pos.x, t.pos.y);
    //    r.TileTemplate = t.TileTemplate;
    //    r.Action = t.Action;
    //    return r;
    //}

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

    private void Error(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        Debug.LogError("trying to use an area manipulation which isn't written");
    }
    private void FlipXY(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        int tmp = g.Max().x;
        Vector2Int flipmax = new Vector2Int(g.Max().y, g.Max().y);
        g.Max(flipmax);

        foreach (TileHolder.TileConfig t in l)
        {
            tmp = t.pos.x;
            t.X(t.pos.y);
            t.Y(tmp);
        }
    }
    private void ReverseX(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        foreach (TileHolder.TileConfig t in l)
        {
            t.X(g.Max().x-t.pos.x);
        }
    }
    private void ReverseY(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        foreach (TileHolder.TileConfig t in l)
        {
            t.Y(g.Max().y - t.pos.y);
        }
    }
    private void Rotate90(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        ReverseX(l, a, g);
        FlipXY(l, a, g);
    }
    private void Rotate270(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        FlipXY(l, a, g);
        ReverseX(l, a, g);
    }
    private void Rotate180(List<TileHolder.TileConfig> l, Area a, TileHolder g)
    {
        ReverseX(l, a, g);
        ReverseY(l, a, g);
    }
    private void ApplyOffset(List<TileHolder.TileConfig> l, Area a)
    {
        foreach (TileHolder.TileConfig t in l)
        {
            //int preX = t.pos.x;
            t.Y(t.pos.y + a.OffsetY);
            t.X(t.pos.x + a.OffsetX);
            //print("reading for " + gameObject.name + " " + preX + "+" + area.OffsetX + "=" + t.pos.x);
        }
    }

    public override Vector2Int Max()
    {
        int miny= int.MaxValue, minx = int.MaxValue, maxx=int.MinValue, maxy=int.MinValue;
        if (max == Vector2Int.zero)
        {
            foreach (TileHolder group in area.Groups)
            {
                foreach (TileHolder.TileConfig t in group.Read())
                {
                    if (t.pos.x < minx)
                        minx = t.pos.x;
                    if (t.pos.x > maxx)
                        maxx = t.pos.x;

                    if (t.pos.y < miny)
                        miny = t.pos.y;
                    if (t.pos.y > maxy)
                        maxy = t.pos.y;
                }

                max = new Vector2Int(maxx - minx, maxy - miny);
            }
        }
        return max;
    }

    public override void Max(Vector2Int max)
    {
        this.max = max;
    }
}
