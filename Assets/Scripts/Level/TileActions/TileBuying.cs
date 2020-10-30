﻿using System.Collections;
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
    private Tile realTileToSell;

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

        realTileToSell = Inventory.CloneTile(TileToSell);
    }
    

    public override void Action(PlayerMovement player)
    {
        if (Inventory.SellPlayerAnItem(realTileToSell))
        {
            SellPlayerTheTile();
            realTileToSell = Inventory.CloneTile(TileToSell);
        }
        else
        {
            BuyPlayersTile();
        }
        
    }

    private void SellPlayerTheTile()
    {
        BuyMessage = BuyMessage.Replace("@c", "" + Inventory.InventoryValue());
        homeTile.Text.Show(BuyMessage);
        homeTile.Text.Show(OnSoldItemToYou, PurchaseMessageDelay);
    }

    private void BuyPlayersTile()
    {
        Tile item;
        if (Inventory.BuyPlayersItem(out item))
        {
            homeTile.Text.Show(SalesMessage);
            homeTile.Text.Show(OnBoughtItemFromYou, PurchaseMessageDelay);
            Destroy(item.gameObject);
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
        //print("left, showing tout message " + ToutMessage);
        homeTile.Text.Show(ToutMessage);
    }
}
