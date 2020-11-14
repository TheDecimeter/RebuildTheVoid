using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoOnTouchAction : TileAction
{
    public string ToutMessage = "";
    public InfoActionHUD InfoMessage;

    private Tile tile;
    private bool shown = false;


    public override void Init(Tile tile)
    {
        ToutMessage = ToutMessage.Replace("\\n", "\n");
        this.tile = tile;
        this.tile.Text.Show(ToutMessage);
    }

    public override bool Action(PlayerMovement player)
    {
        return false;
    }

    public override void OnTouchBegin(PlayerMovement player)
    {
        player.UpdateActionMessage(null);
        if (shown)
            return;
        shown = true;
        InfoMessage.Activate(true);
    }
    public override void OnTouchUpdate(PlayerMovement player)
    {

    }

    public override void OnTouchLeft(PlayerMovement player)
    {
        this.tile.Text.Show(ToutMessage);
    }
}
