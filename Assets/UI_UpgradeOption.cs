using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_UpgradeOption : MonoBehaviour
{
    CharacterUpgrade Upgrade;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Description;
    GameController_DDOL _gc;
    RoundController _rc;
    [SerializeField] Image Icon;

    private void Start()
    {
        _gc = FindObjectOfType<GameController_DDOL>();
        _rc = FindObjectOfType<RoundController>();
    }

    public void SetupButtonWithValues(CharacterUpgrade upgrade)
    {
        Upgrade = upgrade;
        Title.text = upgrade.title;
        Description.text = upgrade.description;
        Icon.sprite = Resources.Load<Sprite>("Images/" + upgrade.iconName);
    }

    public void OnClick()
    {
        _gc.RequestCharacterUpgrade(Upgrade);
        _rc.UpgradeSelected();
    }
}
