using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_LevelSticker : MonoBehaviour
{
    void Start()
    {
        GameController_DDOL _gc = GameObject.FindObjectOfType<GameController_DDOL>();
        GetComponent<TextMeshProUGUI>().text = "Wave " + _gc.round;
    }
}
