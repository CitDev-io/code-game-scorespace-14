using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SHOP_ShopKeep : MonoBehaviour
{
    GameController_DDOL _gc;
    [SerializeField] Button mb;
    [SerializeField] Button helm;
    [SerializeField] TextMeshProUGUI helmPrice;
    [SerializeField] Button chest;
    [SerializeField] TextMeshProUGUI chestPrice;
    [SerializeField] Button belt;
    [SerializeField] TextMeshProUGUI beltPrice;


    void Start()
    {
        _gc = FindObjectOfType<GameController_DDOL>();
    }

    private void FixedUpdate()
    {
        int price = getGoingRate();

        mb.interactable = _gc.coins >= 200;

        helm.interactable = _gc.coins >= price && !_gc.hasHelmet;
        chest.interactable = _gc.coins >= price && !_gc.hasChestplate;
        belt.interactable = _gc.coins >= price && !_gc.hasBelt;

        helmPrice.text = _gc.hasHelmet ? "OWNED" : "" + price;
        chestPrice.text = _gc.hasChestplate ? "OWNED" : "" + price;
        beltPrice.text = _gc.hasBelt ? "OWNED" : "" + price;
    }

    int getGoingRate()
    {
        int owned = 0;
        if (_gc.hasHelmet) owned += 1;
        if (_gc.hasChestplate) owned += 1;
        if (_gc.hasBelt) owned += 1;

        int price = 0;
        switch (owned)
        {
            case 0: price = 500; break;
            case 1: price = 1000; break;
            default: price = 1500; break;

        }
        return price;
    }

    public void AttemptToPurchaseMysteryBox()
    {
        if (_gc.coins < 100) return;

        _gc.CoinBalanceChange(-100);
        _gc.ChangeScene("OpenMysteryBox");
    }

    public void AttemptToPurchaseHelmet()
    {
        int price = getGoingRate();
        if (_gc.coins < price || _gc.hasHelmet) return;

        _gc.hasHelmet = true;
        _gc.CoinBalanceChange(-price);
    }

    public void AttemptToPurchaseChestplate()
    {
        int price = getGoingRate();
        if (_gc.coins < price || _gc.hasChestplate) return;

        _gc.ObtainChestplate();
        _gc.CoinBalanceChange(-price);
    }

    public void AttemptToPurchaseBelt()
    {
        int price = getGoingRate();
        if (_gc.coins < price || _gc.hasBelt) return;

        _gc.hasBelt = true;
        _gc.CoinBalanceChange(-price);
    }
}
