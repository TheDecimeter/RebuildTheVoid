using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWin : TileAction
{
    public InfoActionHUD ActionIfWin;
    public InfoActionHUD ActionIfNotWin;

    public Vector2Int from, to;

    Tile [][] map;
    int[][] m;

    bool inited=false;

    public void Init(Tile[][] map, LevelController Level)
    {
        this.map = map;
        m = new int[map.Length][];
        for (int i = 0; i < map.Length; ++i)
            m[i] = new int[map[0].Length];
    }

    public override void Init(Tile tile)
    {
        if (inited)
            return;
        tile.Level.InitWinCondition(this);
    }

    public override bool Action(PlayerMovement player)
    {
        return false;
    }

    public override void OnTouchBegin(PlayerMovement player)
    {
        if(CompletedPath())
            ActionIfWin.Activate(true);
        else
            ActionIfNotWin.Activate(true);
    }
    public override void OnTouchUpdate(PlayerMovement player)
    {

    }

    public override void OnTouchLeft(PlayerMovement player)
    {

    }

    public bool CompletedPath()
    {
        return WorkerBFS.BFS(m, from.x, from.y, to.x, to.y, map, m.Length * m.Length);
    }
}