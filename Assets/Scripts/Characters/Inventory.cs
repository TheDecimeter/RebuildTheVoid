using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public TextMeshProUGUI MoneyDisplay;
    public TextMeshProUGUI InventoryDisplay;
    
    private static Tile inventorySlot;
    private static int money;

    private static TextMeshProUGUI dMoney;
    private static TextMeshProUGUI dInventory;

    private void Start()
    {
        dMoney = MoneyDisplay;
        dInventory = InventoryDisplay;
        if (dInventory == null)
            print("dInventory is null");
        else
            print("dInventory isn't null");
        inventorySlot = null;
        money = 6;

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
        return inventorySlot.Cost;
    }

    public static bool TryAddItem(Tile item)
    {
        if (inventorySlot == null)
        {
            print("inventory adding " + item.Name);
            if (dInventory == null)
                print("dInventory is null");
            inventorySlot = item;
            dInventory.text = item.Name;
            return true;
        }
        else
            return false;
    }

    public static bool TryGetItem(out Tile item)
    {
        if (inventorySlot != null)
        {
            item = inventorySlot;
            dInventory.text = "No Item";
            inventorySlot = null;
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
        if (!TryAddItem(item))
            return false;
        return TryGetMoney(item.Cost);
    }

    public static bool BuyPlayersItem(out Tile item)
    {
        if (!TryGetItem(out item))
            return false;
        AddMoney(item.Cost);
        return true;
    }


    private static void SetMoneyHud()
    {
        dMoney.text = "$" + Inventory.money;
    }
}
