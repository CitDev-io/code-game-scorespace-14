using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_FloatingDamageText : MonoBehaviour
{
    TextMeshProUGUI _txt;
    public float speed;

    void Start()
    {
        _txt = GetComponent<TextMeshProUGUI>();
        StartCoroutine("Go");
    }

    private void FixedUpdate()
    {
        _txt.color = new Color(_txt.color.r, _txt.color.g, _txt.color.b, _txt.color.a - 0.015f);
        transform.position = transform.position + (Vector3.up * speed);
    }

    IEnumerator Go() {

        yield return new WaitForSeconds(1.25f);
        Destroy(gameObject);
    }

}
