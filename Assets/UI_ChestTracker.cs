using UnityEngine;
using UnityEngine.UI;

public class UI_ChestTracker : MonoBehaviour
{
    GameController_DDOL _gc;
    Image _i;
    [SerializeField] Sprite on;
    [SerializeField] Sprite off;

    void Start()
    {
        _gc = GameObject.FindObjectOfType<GameController_DDOL>();
        _i = GetComponent<Image>();

        _i.sprite = _gc.hasChestplate ? on : off;
    }

}
