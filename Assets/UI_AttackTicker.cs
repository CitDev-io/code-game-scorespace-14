using UnityEngine;
using TMPro;

public class UI_AttackTicker : MonoBehaviour
{
    GameController_DDOL _gc;
    TextMeshProUGUI _txt;

    private void Start()
    {
        _gc = FindObjectOfType<GameController_DDOL>();
        _txt = GetComponent<TextMeshProUGUI>();
    }
    private void OnGUI()
    {
        CharacterUpgrade up = _gc.GetUpgradeValues();
        _txt.text = up.SwordInstanceMin + " to " + up.SwordInstanceMax;
    }
}
