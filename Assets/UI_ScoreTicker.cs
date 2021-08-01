using UnityEngine;
using TMPro;

public class UI_ScoreTicker : MonoBehaviour
{
    GameController_DDOL _gc;
    TextMeshProUGUI _txt;

    private void Start()
    {
        _gc = GameObject.FindObjectOfType<GameController_DDOL>();
        _txt = GetComponent<TextMeshProUGUI>();
    }
    private void OnGUI()
    {
        _txt.text = _gc.score + "";
    }
}
