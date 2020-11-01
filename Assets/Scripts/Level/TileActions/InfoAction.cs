using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoAction : TileAction
{
    public string StartMessage = "";
    public InfoActionHUD InfoMessage;
    public float ShowFor = 3;
    private Tile tile;


    public override void Init(Tile tile)
    {
        StartMessage = StartMessage.Replace("\\n", "\n");
        this.tile = tile;
        this.tile.Text.Show(StartMessage);
    }

    public override void Action(PlayerMovement player)
    {
        InfoMessage.Activate(true);
    }

    public override void OnTouchBegin(PlayerMovement player)
    {
    }
    public override void OnTouchUpdate(PlayerMovement player)
    {
    }

    public override void OnTouchLeft(PlayerMovement player)
    {
    }
}
