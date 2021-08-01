using UnityEngine;
using TMPro;

public class UI_ArmorTicker : MonoBehaviour
{
    RoundController _rc;
    TextMeshProUGUI _txt;

    private void Awake()
    {
        _rc = GameObject.FindObjectOfType<RoundController>();
        _txt = GetComponent<TextMeshProUGUI>();
    }
    private void OnGUI()
    {
        int shieldMax = _rc.StatSheet().ShieldMax + (int)(_rc.StatSheet().ShieldMax * (_rc.StatSheet().armorMaxPercentModifier / 100));
        _txt.text = _rc.Armor + " / " + shieldMax;
    }
}
