using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Leader : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _score;
    [SerializeField] TextMeshProUGUI _index;

    private void Awake()
    {
        _index.gameObject.SetActive(false);
    }

    public void SetLeader(string name, string score)
    {
        _index.gameObject.SetActive(true);
        _name.text = name;
        _score.text = score;
    }
}
