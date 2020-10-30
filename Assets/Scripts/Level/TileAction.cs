using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileAction : MonoBehaviour
{
    public abstract void Init(Tile tile);
    public abstract void Action(PlayerMovement player);
    public abstract void OnTouchBegin(PlayerMovement player);
    public abstract void OnTouchUpdate(PlayerMovement player);
    public abstract void OnTouchLeft(PlayerMovement player);
}
