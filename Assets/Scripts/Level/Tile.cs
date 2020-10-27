using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject floor;
    public GameObject pillar;
    public GameObject pullPoint;
    public bool addon = false;

    private int _s = 0;
    public int StackSize { get
        {
            return _s;
        }
        protected set
        {
            //print("set stack at " + x + " " + y + value);
            _s = value;

        }
    }

    private int tileHeight = 1, x, y;

    private List<Tile> additions = new List<Tile>();

    public void SetPos(int x, int y)
    {
        this.transform.position = LevelController.PhysicalLocation(x, y);
        this.x = x;
        this.y = y;
    }

    public void UpdateHeight()
    {
        Vector3 pos = new Vector3(pillar.transform.position.x, StackSize * tileHeight, pillar.transform.position.z);
        floor.transform.localScale = new Vector3(10, tileHeight * StackSize, 10);
        floor.transform.position = new Vector3(floor.transform.position.x, StackSize/2 * tileHeight, floor.transform.position.z);

        pillar.transform.position = pos;

        foreach (Tile t in additions)
        {
            t.transform.position = pos;
        }
    }

    public void Add(Tile tile)
    {
        if (!tile.addon)
        {
            if (StackSize == 0)
            {
                floor.gameObject.SetActive(true);
                pillar.gameObject.SetActive(true);
            }
            StackSize++;
            //print("adding stack size for " + x + " " + y+ "  stack size ="+StackSize);
        }
        GameObject g = Instantiate(tile.gameObject);
        g.transform.position = LevelController.PhysicalLocation(x, y);
        additions.Add(g.GetComponent<Tile>());

        UpdateHeight();
    }
}
