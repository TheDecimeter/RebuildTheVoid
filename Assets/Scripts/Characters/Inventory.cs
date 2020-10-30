using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI MoneyDisplay;
    public TextMeshProUGUI InventoryDisplay;
    
    private static IEnumerable<Tile> inventorySlot;
    private static int money, tileValue;
    private static bool addon;

    private static TextMeshProUGUI dMoney;
    private static TextMeshProUGUI dInventory;

    private void Start()
    {
        dMoney = MoneyDisplay;
        dInventory = InventoryDisplay;
        
        inventorySlot = null;

        money = 6;
        addon = false;
        tileValue = 0;

        SetMoneyHud();
    }

    public static bool IsEmpty()
    {
        return inventorySlot == null;
    }

    public static int InventoryValue()
    {
        if (inventorySlot == null)
            return 0;
        return tileValue;
    }
    public static bool TileIsAddon()
    {
        if (inventorySlot == null)
            return false;
        return addon;
    }

    public static bool TryAddItem(IEnumerable<Tile> item)
    {
        if (inventorySlot == null)
        {
            FillInventorySlot(item);
            return true;
        }
        else
            return false;
    }

    private static void FillInventorySlot(IEnumerable<Tile> tile)
    {
        inventorySlot = tile;
        tileValue = 0;
        addon = false;
        string n = "";
        foreach(Tile t in tile)
        {
            if (t.Addon)
                addon = true;
            tileValue += t.Cost;
            n += " " + t.Name;
            t.gameObject.SetActive(false);
        }
        if (tileValue != 0)
        {
            if (addon)
                dInventory.text = "Addon:" + n;
            else
                dInventory.text = "Tile:" + n;
        }
        else
        {
            dInventory.text = "No Item";
            addon = false;
            inventorySlot = null;
        }
    }

    public static bool TryGetItem(out IEnumerable<Tile> item)
    {
        if (inventorySlot != null)
        {
            item = inventorySlot;
            dInventory.text = "No Item";
            inventorySlot = null;
            tileValue = 0;
            addon = false;
            return true;
        }
        else
        {
            item = null;
            return false;
        }
    }

    public static bool TryGetMoney(int money)
    {
        if (money > Inventory.money)
            return false;
        Inventory.money -= money;
        //print("  removing " + money + " dollars, " + Inventory.money);
        SetMoneyHud();
        return true;
    }

    public static void AddMoney(int money)
    {
        int preInv = Inventory.money;
        Inventory.money += money;
        //print("adding " + money + " dollars to "+preInv+", " + Inventory.money);
        SetMoneyHud();
    }


    public static bool SellPlayerAnItem(Tile item)
    {
        if (money < item.Cost)
            return false;
        if (!TryAddItem(Multi(item)))
            return false;
        return TryGetMoney(item.Cost);
    }

    public static bool BuyPlayersItem(out IEnumerable<Tile> item)
    {
        if (!TryGetItem(out item))
            return false;
        AddMoney(TileCost(item));
        return true;
    }


    private static void SetMoneyHud()
    {
        dMoney.text = "$" + Inventory.money;
    }

    public static IEnumerable<Tile> Multi(Tile tile)
    {
        yield return tile;
    }
    public static Tile CloneTile(Tile tile)
    {
        GameObject g = Instantiate(tile.gameObject);
        return g.GetComponent<Tile>();
    }

    public static int TileCost(IEnumerable<Tile> tiles)
    {
        int r = 0;
        foreach (Tile t in tiles)
            r += t.Cost;
        return r;
    }
}
