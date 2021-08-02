using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SHOP_ShopKeep : MonoBehaviour
{
    GameController_DDOL _gc;
    [SerializeField] Button mb;
    [SerializeField] TextMeshProUGUI mbPrice;
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

    void OnGUI()
    {
        int price = getGoingRate();

        mb.interactable = _gc.coins >= _gc.mysteryBoxPrice;

        helm.interactable = _gc.coins >= price && !_gc.hasHelmet;
        chest.interactable = _gc.coins >= price && !_gc.hasChestplate;
        belt.interactable = _gc.coins >= price && !_gc.hasBelt;

        helmPrice.text = _gc.hasHelmet ? "OWNED" : "" + price;
        chestPrice.text = _gc.hasChestplate ? "OWNED" : "" + price;
        beltPrice.text = _gc.hasBelt ? "OWNED" : "" + price;
        mbPrice.text = _gc.mysteryBoxPrice + "";
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
            case 1: price = 2500; break;
            default: price = 12500; break;

        }
        return price;
    }

    public void AttemptToPurchaseMysteryBox()
    {
        if (_gc.coins < _gc.mysteryBoxPrice) return;
        _gc.CoinBalanceChange(-_gc.mysteryBoxPrice);
        _gc.mysteryBoxPrice = Mathf.FloorToInt(_gc.mysteryBoxPrice * 1.20f);
        _gc.ChangeScene("OpenMysteryBox");
    }

    public void AttemptToPurchaseHelmet()
    {
        int price = getGoingRate();
        if (_gc.coins < price || _gc.hasHelmet) return;
        _gc.PlaySound("Coin_Collect");
        _gc.hasHelmet = true;
        _gc.CoinBalanceChange(-price);
    }

    public void AttemptToPurchaseChestplate()
    {
        int price = getGoingRate();
        if (_gc.coins < price || _gc.hasChestplate) return;
        _gc.PlaySound("Coin_Collect");
        _gc.ObtainChestplate();
        _gc.CoinBalanceChange(-price);
    }

    public void AttemptToPurchaseBelt()
    {
        int price = getGoingRate();
        if (_gc.coins < price || _gc.hasBelt) return;
        _gc.PlaySound("Coin_Collect");
        _gc.hasBelt = true;
        _gc.CoinBalanceChange(-price);
    }
}
