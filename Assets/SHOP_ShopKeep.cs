using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SHOP_ShopKeep : MonoBehaviour
{
    GameController_DDOL _gc;
    [SerializeField] Button mb;
    [SerializeField] Button helm;
    [SerializeField] Button chest;
    [SerializeField] Button belt;

    void Start()
    {
        _gc = FindObjectOfType<GameController_DDOL>();
    }

    private void FixedUpdate()
    {
        mb.interactable = _gc.coins >= 200;

        helm.interactable = _gc.coins >= 1000 && !_gc.hasHelmet;
        chest.interactable = _gc.coins >= 1000 && !_gc.hasChestplate;
        belt.interactable = _gc.coins >= 1000 && !_gc.hasBelt;
    }

    public void AttemptToPurchaseMysteryBox()
    {
        if (_gc.coins < 200) return;

        Debug.Log("BOUGHT MYSTERY BOX, rip its empty");
        _gc.CoinBalanceChange(-200);
    }

    public void AttemptToPurchaseHelmet()
    {
        if (_gc.coins < 1000 || _gc.hasHelmet) return;

        _gc.hasHelmet = true;
        _gc.CoinBalanceChange(-1000);
    }

    public void AttemptToPurchaseChestplate()
    {
        if (_gc.coins < 1000 || _gc.hasChestplate) return;

        _gc.ObtainChestplate();
        _gc.CoinBalanceChange(-1000);
    }

    public void AttemptToPurchaseBelt()
    {
        if (_gc.coins < 1000 || _gc.hasBelt) return;

        _gc.hasBelt = true;
        _gc.CoinBalanceChange(-1000);
    }
}
