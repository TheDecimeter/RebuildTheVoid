using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoAction : TileAction
{
    public string ToutMessage = "";
    public string IntroMessage = "";
    public InfoActionHUD InfoMessage;
    private Tile tile;


    public override void Init(Tile tile)
    {
        ToutMessage = ToutMessage.Replace("\\n", "\n");
        IntroMessage = IntroMessage.Replace("\\n", "\n");
        this.tile = tile;
        this.tile.Text.Show(ToutMessage);
    }

    public override bool Action(PlayerMovement player)
    {
        InfoMessage.Activate(true);
        return true;
    }

    public override void OnTouchBegin(PlayerMovement player)
    {
        this.tile.Text.Show(IntroMessage);
        player.UpdateActionMessage("Info");
    }
    public override void OnTouchUpdate(PlayerMovement player)
    {

    }

    public override void OnTouchLeft(PlayerMovement player)
    {
        this.tile.Text.Show(ToutMessage);
    }
}
