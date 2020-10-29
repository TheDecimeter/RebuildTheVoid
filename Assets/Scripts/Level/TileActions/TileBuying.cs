using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuying : TileAction
{
    public string ToutMessage = "";
    public string SalesMessage = "";
    public string BuyMessage = "";
    public string OnSoldItemToYou = "";
    public string OnBoughtItemFromYou = "";
    public float PurchaseMessageDelay = 5;

    public Tile TileToSell;

    private Tile homeTile;


    public override void Init(Tile tile)
    {
        ToutMessage = ToutMessage.Replace("\\n", "\n");

        SalesMessage = SalesMessage.Replace("\\n", "\n");
        SalesMessage = SalesMessage.Replace("@c", ""+TileToSell.Cost);

        BuyMessage = BuyMessage.Replace("\\n", "\n");
        OnSoldItemToYou = OnSoldItemToYou.Replace("\\n", "\n");
        OnBoughtItemFromYou = OnBoughtItemFromYou.Replace("\\n", "\n");

        this.homeTile = tile;
        this.homeTile.Text.Show(ToutMessage);
    }

    public override void Action(PlayerMovement player)
    {
        if (!Inventory.TrySellItem(TileToSell))
        {
            Tile item;
            if(Inventory.TryBuyItem(out item)){
                homeTile.Text.Show(SalesMessage);
                homeTile.Text.Show(OnBoughtItemFromYou, PurchaseMessageDelay);
            }
        }
        else
        {
            BuyMessage = BuyMessage.Replace("@c", "" + Inventory.InventoryValue());
            homeTile.Text.Show(BuyMessage);
            homeTile.Text.Show(OnSoldItemToYou, PurchaseMessageDelay);
        }
        
    }

    public override void Copy(TileAction original)
    {
        if (original is TileBuying)
        {
            TileBuying o = (TileBuying)original;
            this.TileToSell = o.TileToSell;
        }
    }


    public override void OnTouchBegin(PlayerMovement player)
    {
        if (Inventory.IsEmpty())
        {
            homeTile.Text.Show(SalesMessage);
        }
        else
        {
            BuyMessage = BuyMessage.Replace("@c", "" + Inventory.InventoryValue());
            homeTile.Text.Show(BuyMessage);
        }
    }
    public override void OnTouchUpdate(PlayerMovement player)
    {
    }

    public override void OnTouchLeft(PlayerMovement player)
    {
        print("left, showing tout message " + ToutMessage);
        homeTile.Text.Show(ToutMessage);
    }
}
