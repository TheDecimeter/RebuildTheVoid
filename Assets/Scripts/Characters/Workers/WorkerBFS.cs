using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBFS : MonoBehaviour
{
    public Vector2Int Goal1, Goal2;
    public int MaxDist;

    private int [][] to1, to2;
    
    public void Set(Tile [][] map)
    {
        to1 = new int[map.Length][];
        for (int i = 0; i < map.Length; ++i)
            to1[i] = new int[map[0].Length];
        to2 = new int[map.Length][];
        for (int i = 0; i < map.Length; ++i)
            to2[i] = new int[map[0].Length];
    }

    public void UpdatePaths(Tile [][] map)
    {
        BFS(to1, Goal1.x, Goal1.y, Goal2.x, Goal2.y, map);
        BFS(to2, Goal2.x, Goal2.y, Goal1.x, Goal1.y, map);
    }

    public void BFS(int[][] m, int fX, int fY, int tX, int tY, Tile[][]map)
    {
        resetMap(m);
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(new Node(fX, fY, 1));

        while (q.Count > 0)
        {
            Node c = q.Dequeue();

            m[c.x][c.y] = c.c;
            if (c.x == tX && c.y == tY)
            {
                print("Worker BFS made path");
                return;
            }
            if (c.c > MaxDist)
            {
                print("Worker BFS unable to make path, too far");
                return;
            }


            Node n;
            if (tryGetNode(m, c, 0, 1, map, out n))
                q.Enqueue(n);
            if (tryGetNode(m, c, 0, -1, map, out n))
                q.Enqueue(n);
            if (tryGetNode(m, c, -1, 0, map, out n))
                q.Enqueue(n);
            if (tryGetNode(m, c, 1, 0, map, out n))
                q.Enqueue(n);
        }
        print("Worker BFS couldn't make path, none found");
    }

    private void resetMap(int [][] m)
    {
        int length = m.Length, width = m[0].Length;
        for (int i = 0; i < length; ++i)
            for (int j = 0; j < width; ++j)
                m[i][j] = 0;
    }

    private static bool tryGetNode(int[][] m, Node n, int ox, int oy, Tile[][] map, out Node o)
    {
        if (m[n.x + ox][n.y + oy] != 0)
        {
            o = null;
            return false;
        }

        if(isValid(map[n.x][n.y], map[n.x + ox][n.y + oy]))
        {
            o = new Node(n.x + ox, n.y + oy, n.c + 1);
            return true;
        }
        o = null;
        return false;
    }

    private static bool isValid(Tile from, Tile to)
    {
        if (to == null || to.StackSize == 0)
            return false;
        return WithinOneHeight(from, to);
    }

    private static bool WithinOneHeight(Tile from, Tile to)
    {
        return from.StackSize - 1 <= to.StackSize && to.StackSize <= from.StackSize + 1;
    }


    class Node
    {
        public readonly int c, x, y;
        public Node(int x, int y, int c)
        {
            this.c = c;
            this.x = x;
            this.y = y;
        }
    }
}
