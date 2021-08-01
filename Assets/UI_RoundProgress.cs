using UnityEngine;
using TMPro;

public class UI_RoundProgress : MonoBehaviour
{
    [SerializeField] GameObject text;
    [SerializeField] GameObject check;

    RoundController _rc;

    private void Awake()
    {
        _rc = GameObject.FindObjectOfType<RoundController>();
    }

    private void Start()
    {
        _rc.OnRoundEnd += OnRoundEnd;
        text.SetActive(true);
        check.SetActive(false);
    }

    private void OnDestroy()
    {
        _rc.OnRoundEnd -= OnRoundEnd;
    }

    void OnRoundEnd()
    {
        text.SetActive(false);
        check.SetActive(true);
    }
}