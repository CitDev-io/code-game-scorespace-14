using UnityEngine;
using TMPro;

public class UI_LevelProgressTicker : MonoBehaviour
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
        _txt.text = _rc.TotalKills() + " / " + _rc.NextLevelAt;
    }
}
