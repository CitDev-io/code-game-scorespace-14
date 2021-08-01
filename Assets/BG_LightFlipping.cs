using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_LightFlipping : MonoBehaviour
{
    [SerializeField] Sprite lightOn;
    [SerializeField] Sprite lightOff;
    SpriteRenderer _sr;

    void Start()
    {
        StartCoroutine("RollForLight");
        _sr = GetComponent<SpriteRenderer>();
    }

    IEnumerator RollForLight()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(4f, 18f));
            Sprite random = Random.Range(0, 2) == 0 ? lightOn : lightOff;

            _sr.sprite = random;
        }
    }
}
