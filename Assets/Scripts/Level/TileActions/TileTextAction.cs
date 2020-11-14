using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTextAction : TileAction
{
    public string StartMessage = "";
    public string ActionMessage = "";
    public float ShowFor = 3;
    private Tile tile;


    public override void Init(Tile tile)
    {
        StartMessage = StartMessage.Replace("\\n", "\n");
        ActionMessage = ActionMessage.Replace("\\n", "\n");
        this.tile = tile;
        this.tile.Text.Show(StartMessage);
    }

    public override bool Action(PlayerMovement player)
    {
        //print("Tile Text Action triggered");
        tile.Text.Show(ActionMessage,ShowFor);
        return true;
    }

    public override void OnTouchBegin(PlayerMovement player)
    {
        player.UpdateActionMessage("Info");
    }
    public override void OnTouchUpdate(PlayerMovement player)
    {
    }

    public override void OnTouchLeft(PlayerMovement player)
    {
    }
}
