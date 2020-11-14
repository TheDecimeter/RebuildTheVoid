using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoAction : TileAction
{
    public string ToutMessage = "";
    public string IntroMessage = "";
    public Vector2 faceDir = Vector2.zero;
    public InfoActionHUD InfoMessage;
    private Tile tile;


    public override void Init(Tile tile)
    {
        if (faceDir != Vector2.zero)
            faceDir.Normalize();
        ToutMessage = ToutMessage.Replace("\\n", "\n");
        IntroMessage = IntroMessage.Replace("\\n", "\n");
        this.tile = tile;
        this.tile.Text.Show(ToutMessage);
    }

    public override bool Action(PlayerMovement player)
    {
        if (faceDir != Vector2.zero)
        {
            Vector3 f= new Vector3(faceDir.x, 0, faceDir.y);
            player.SetDirection(f);
        }
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
