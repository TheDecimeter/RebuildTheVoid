using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberSpawner : MonoBehaviour
{
    public Bomber BomberTemplate;

    public Vector2Int AttackArea;
    public int AttackRange;
    public int MaxAttacks;
    public float attackFrequency;

    private int currentAttacks = 0;
    private Tile[][] map;

    private int maxX, minX, maxY, minY;

    private bool attacking = false;

    private List<Tile> Tiles;

    private HashSet<Bomber> bombers = new HashSet<Bomber>();

    public void Init(Tile[][] map, LevelController level)
    {
        this.map = map;

        setBounds();

        getTiles();
        if (Tiles.Count > 0)
            StartAttack();
    }

    public void UpdateTile(int x, int y)
    {
        if (inRange(x, y))
        {
            getTiles();
            if (Tiles.Count > 0)
                StartAttack();
            else
            {
                attacking = false;
                StopAllCoroutines();
                DestroyBombers();
            }
        }
    }

    private void DestroyBombers()
    {
        foreach (Bomber b in bombers)
            Destroy(b.gameObject);
        bombers = new HashSet<Bomber>();
    }

    public void RemoveBomber(Bomber bomber)
    {
        bombers.Remove(bomber);
        if(bomber!=null)
            Destroy(bomber.gameObject);
    }

    private void setBounds()
    {
        minX = Mathf.Max(AttackArea.x - AttackRange, 1);
        minY = Mathf.Max(AttackArea.y - AttackRange, 1);

        maxX = Mathf.Min(AttackArea.x + AttackRange, map.Length - 2);
        maxY = Mathf.Min(AttackArea.y + AttackRange, map[0].Length - 2);
    }

    private void getTiles()
    {
        //print("Get tiles " + minX + " " + maxX + "   " + minY + " " + maxY);
        Tiles = new List<Tile>();

        for(int i=minX; i<=maxX; ++i)
        {
            for(int j=minY; j<=maxY; ++j)
            {
                if (ValidTile(map[i][j]))
                    Tiles.Add(map[i][j]);
            }
        }

        //print("got tiles " + Tiles.Count);
    }

    private bool ValidTile(Tile t)
    {
        if (t == null || t.Static)
            return false;
        return t.StackSize > 0;
    }

    private bool inRange(int x, int y)
    {
        return !(x > maxX || x < minX || y > maxY || y < minY);
    }


    private void StartAttack()
    {
        if (attacking)
            return;
        StartCoroutine(StartAttackCoroutine());
    }

    private IEnumerator StartAttackCoroutine()
    {
        attacking = true;
        while (true)
        {
            if (bombers.Count < MaxAttacks)
                LaunchBomber();
            yield return new WaitForSeconds(attackFrequency);
        }
    }

    private void LaunchBomber()
    {
        Tile target = Tiles[Random.Range(0,Tiles.Count-1)];
        //print("launching bomber at " + target.gameObject.name);

        GameObject g = Instantiate(BomberTemplate.gameObject);
        Bomber b = g.GetComponent<Bomber>();
        b.Init(target, this);
        bombers.Add(b);
    }

    private static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
