using UnityEngine;
using TMPro;

public class UI_XPTicker : MonoBehaviour
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
        _txt.text = "x" + (_rc.KillRequirement - _rc.Kills);
    }
}
