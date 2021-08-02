using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using UnityEngine.UI;

public class SHOP_OpenBox : MonoBehaviour
{
    [SerializeField] SkeletonGraphic chestSkeleton;
    [SerializeField] TextMeshProUGUI rewardName;
    [SerializeField] TextMeshProUGUI rewardDesc;
    [SerializeField] Image rewardIcon;

    [Header("Sprites")]
    [SerializeField] Sprite spr_Coins;
    [SerializeField] Sprite spr_Belt;
    [SerializeField] Sprite spr_Chest;
    [SerializeField] Sprite spr_Helm;
   

    GameController_DDOL _gc;
    string _text;
    string _desc;
    Sprite _sprite;

    private void Start()
    {
        _gc = FindObjectOfType<GameController_DDOL>();
        rewardIcon.gameObject.SetActive(false);
        rewardName.gameObject.SetActive(false);
        rewardDesc.gameObject.SetActive(false);
    }
    bool isOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Open();
        }
    }

    public void Open()
    {
        if (isOpen)
        {
            GetComponent<ChangeScene>().SwapToScene("Store");
            return;
        }

        StartCoroutine("DoOpen");
    }

    void AwardRandomCoins()
    {
        int coinCt = Random.Range(15, 56);
        _text = coinCt + " Coins";
        _desc = "Better luck next time";
        _sprite = spr_Coins;
        _gc.PlaySound("Receive_Coins_MysteryBox");
        _gc.CoinBalanceChange(coinCt);
    }

    void AwardRandomUpgrade()
    {
        _gc.PlaySound("Receive_Item_Upgrade_MysteryBox");
        int randomId = Random.Range(1, 9);
        while (randomId == 5)
        {
            randomId = Random.Range(1, 9);
        }
 
        CharacterUpgrade rando = Resources.Load<CharacterUpgrade>("CharacterUpgrade/" + randomId);
        _text = rando.title;
        _desc = rando.description;
        _sprite = Resources.Load<Sprite>("Images/" + rando.iconName);
        _gc.RequestCharacterUpgrade(rando);
    }

    void AwardRandomItem()
    {
        if (_gc.hasBelt && _gc.hasChestplate && _gc.hasHelmet)
        {
            AwardRandomUpgrade();
            return;
        }

        _desc = "A rare find!";
        bool foundOne = false;
        while (!foundOne)
        {
            int random = Random.Range(1, 4);
            switch (random)
            {
                case 1:
                    if (!_gc.hasBelt)
                    {
                        // give em the belt
                        _text = "Belt of Thievery";
                        _sprite = spr_Belt;
                        _gc.hasBelt = true;
                        foundOne = true;
                    }
                    break;
                case 2:
                    if (!_gc.hasHelmet)
                    {
                        // give em the chest
                        _text = "Helm of Conscription";
                        _sprite = spr_Helm;
                        _gc.hasHelmet = true;
                        foundOne = true;
                    }
                    break;
                default:
                    if (!_gc.hasChestplate)
                    {
                        // give em the helmet
                        _text = "Chestplate of Warding";
                        _sprite = spr_Chest;
                        _gc.ObtainChestplate();
                        foundOne = true;
                    }
                    break;
            }
        }
        _gc.PlaySound("Receive_Item_Upgrade_MysteryBox");
    }

    IEnumerator DoOpen()
    {
        isOpen = true;
        _gc.PlaySound("Open_MysteryBox");
        chestSkeleton.AnimationState.SetAnimation(0, "open", false);

        yield return new WaitForSeconds(0.65f);

        int reward = Random.Range(0, 101);

        if (reward <= 75)
        {
            AwardRandomCoins();
        }
        else if (reward <= 97)
        {
            AwardRandomUpgrade();
        }
        else
        {
            AwardRandomItem();
        }
        rewardIcon.sprite = _sprite;
        rewardDesc.text = _desc;
        rewardName.text = _text;
        rewardIcon.gameObject.SetActive(true);
        rewardName.gameObject.SetActive(true);
        rewardDesc.gameObject.SetActive(true);
    }
}
