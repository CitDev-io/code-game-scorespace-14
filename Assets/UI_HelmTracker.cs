using UnityEngine;
using UnityEngine.UI;

public class UI_HelmTracker : MonoBehaviour
{
    GameController_DDOL _gc;
    RoundController _rc;
    Image _i;
    [SerializeField] Sprite on;
    [SerializeField] Sprite off;

    [SerializeField] Sprite borderOn;
    [SerializeField] Sprite borderOff;
    [SerializeField] Image border;

    void Start()
    {
        _rc = GameObject.FindObjectOfType<RoundController>();
        _gc = GameObject.FindObjectOfType<GameController_DDOL>();
        _i = GetComponent<Image>();

        _i.sprite = _gc.hasHelmet ? on : off;
    }

    private void OnGUI()
    {
        border.sprite = _rc.CanCastHelm ? borderOn : borderOff;
    }

    public void OnMouseDown()
    {
        _rc.AttemptToCastHelm();
    }

}
