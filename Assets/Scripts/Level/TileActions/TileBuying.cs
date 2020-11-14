using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuying : TileAction
{
    private const int askPlayerToSellState = 0, askPlayerToBuyState = 1;
    public string ToutMessage = "";
    public string SalesMessage = "";
    public string BuyMessage = "";
    public string OnSoldItemToYou = "";
    public string OnBoughtItemFromYou = "";
    public float PurchaseMessageDelay = 5;

    public Tile TileToSell;
    private Tile realTileToSell;

    private Tile homeTile;

    private int playerState = askPlayerToSellState;

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
    

    public override bool Action(PlayerMovement player)
    {
        if (Inventory.SellPlayerAnItem(realTileToSell))
        {
            SellPlayerTheTile(player);
            realTileToSell = Inventory.CloneTile(TileToSell);
        }
        else
        {
            BuyPlayersTile(player);
        }
        return true;
    }

    private void SellPlayerTheTile(PlayerMovement player)
    {
        //string m = BuyMessage.Replace("@c", "" + Inventory.InventoryValue());
        //print("selling player the tile "+m);
        //homeTile.Text.Show(m); //orbit to sell
        AskPlayerToSell(player);
        homeTile.Text.Show(OnSoldItemToYou, PurchaseMessageDelay);
    }

    private void BuyPlayersTile(PlayerMovement player)
    {
        IEnumerable<Tile> item;
        if (Inventory.BuyPlayersItem(out item))
        {
            //homeTile.Text.Show(SalesMessage); //orbit to buy
            AskPlayerToBuy(player);
            homeTile.Text.Show(OnBoughtItemFromYou, PurchaseMessageDelay);
            foreach(Tile t in item)
                Destroy(t.gameObject);
        }
    }


    public override void OnTouchBegin(PlayerMovement player)
    {
        if (Inventory.IsEmpty())
        {
            AskPlayerToBuy(player);
        }
        else
        {
            AskPlayerToSell(player);
        }
    }

    private void AskPlayerToBuy(PlayerMovement player)
    {
        homeTile.Text.Show(SalesMessage); //orbit to buy
        player.UpdateActionMessage("Buy");
        playerState = askPlayerToBuyState;
    }
    private void AskPlayerToSell(PlayerMovement player)
    {
        string m = BuyMessage.Replace("@c", "" + Inventory.InventoryValue());
        homeTile.Text.Show(m); // orbit to sell
        player.UpdateActionMessage("Sell");
        playerState = askPlayerToSellState;
    }

    public override void OnTouchUpdate(PlayerMovement player)
    {
        if (Inventory.IsEmpty())
        {
            if (playerState == askPlayerToSellState)
            {
                AskPlayerToBuy(player);
            }
        }
        else
        {
            if (playerState == askPlayerToBuyState)
            {
                AskPlayerToSell(player);
            }
        }
    }

    public override void OnTouchLeft(PlayerMovement player)
    {
        //print("left, showing tout message " + ToutMessage);
        homeTile.Text.Show(ToutMessage);
    }
}
