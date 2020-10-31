using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBFS : MonoBehaviour
{
    public Vector2Int Goal1, Goal2;
    public int MaxDist;
    public LevelController Level { get; protected set; }

    private int [][] to1, to2;
    
    public void Set(Tile [][] map, LevelController level)
    {
        this.Level = level;

        to1 = new int[map.Length][];
        for (int i = 0; i < map.Length; ++i)
            to1[i] = new int[map[0].Length];
        to2 = new int[map.Length][];
        for (int i = 0; i < map.Length; ++i)
            to2[i] = new int[map[0].Length];
    }

    public void UpdatePaths(Tile [][] map)
    {
        BFS(to1, Goal1.x, Goal1.y, Goal2.x, Goal2.y, map, MaxDist);
        BFS(to2, Goal2.x, Goal2.y, Goal1.x, Goal1.y, map, MaxDist);
    }

    public void UpdatePathsCoroutine(Tile [][] map)
    {
        StopAllCoroutines();
        //StartCoroutine(BFS(to1, Goal1.x, Goal1.y, Goal2.x, Goal2.y, map, MaxDist));
        //StartCoroutine(BFS(to2, Goal2.x, Goal2.y, Goal1.x, Goal1.y, map, MaxDist));
    }

    public int DistTo(Vector2Int goal, int x, int y)
    {
        if (goal == Goal1) return to2[x][y];
        else return to1[x][y];
    }

    public bool TryGetMove(Vector2Int goal, int fx, int fy, out int gx, out int gy)
    {
        if (goal == Goal1) return TryGetMove(to1, fx, fy, out gx, out gy);
        else return TryGetMove(to2, fx, fy, out gx, out gy);
    }

    private bool TryGetMove(int[][] m, int fx, int fy, out int gx, out int gy)
    {
        gx = 0;gy = 0;
        int c = m[fx][fy];

        print("getting move " + fx + " " + fy + " c:" + c);
        printMap(m, 0, 10, 0, 8);

        if (m[fx + 1][fy] == c - 1)
        {
            gx = 1;
            return true;
        }
        if (m[fx - 1][fy] == c - 1)
        {
            gx = -1;
            return true;
        }
        if (m[fx][fy + 1] == c - 1)
        {
            gy = 1;
            return true;
        }
        if (m[fx][fy - 1] == c - 1)
        {
            gy = -1;
            return true;
        }
        return false;
    }

    public static void BFS(int[][] m, int fX, int fY, int tX, int tY, Tile[][]map, int MaxDist)
    {
        resetMap(m);
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(new Node(fX, fY, 1));
        bool noPath = true;
        while (q.Count > 0)
        {
            Node c = q.Dequeue();

            m[c.x][c.y] = c.c;
            if (c.x == tX && c.y == tY)
            {
                print("Worker BFS made path");
                printMap(m, 0, 10, 0, 8);
                noPath = false;
                break;
            }
            if (c.c > MaxDist)
            {
                print("Worker BFS unable to make path, too far");
                noPath = false;
                break;
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

            //yield return null;
        }

        if(noPath)
            print("Worker BFS couldn't make path, none found");
    }

    private static void resetMap(int [][] m)
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

    private static void printMap(int[][] m, int x1, int x2, int y1, int y2)
    {
        string s="";
        for(int i=y1; i<y2; ++i)
        {
            for(int j=x1; j<x2; ++j)
            {
                s += m[j][i] + " ";
            }
            s += "\n";
        }
        print("Map from " + x1 + " to " + x2 + " and " + y1 + "to" + y2 + "\n" + s);
    }
}
