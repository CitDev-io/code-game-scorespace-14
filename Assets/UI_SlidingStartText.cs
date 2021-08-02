using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SlidingStartText : MonoBehaviour
{
    [SerializeField] public GameObject startText;
    Vector3 _destinationPosition;
    float lerpSpeed = 3f;

    void Awake() {
        _destinationPosition = startText.GetComponent<RectTransform>().anchoredPosition;
    }

    public void GoGoStartText(string message1, string message2) {
        StartCoroutine(TextFlyBy(message1, message2));
    }

    void Update() {
        startText.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startText.GetComponent<RectTransform>().anchoredPosition, _destinationPosition, Time.deltaTime * lerpSpeed);
    }

    IEnumerator TextFlyBy(string message1, string message2) {
        startText.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1024f, 156f, 0f);
        _destinationPosition = new Vector3(-1024f, 156f, 0f);
        startText.GetComponent<TextMeshProUGUI>().text = message1;
        lerpSpeed = 3f;
        yield return new WaitForSeconds(0.25f);

        _destinationPosition = new Vector3(0f, 156f, 0f);
        yield return new WaitForSeconds(2.55f);
        _destinationPosition = new Vector3(1500f, 156f, 0f);
        startText.GetComponent<TextMeshProUGUI>().text = message2;

        yield return new WaitForSeconds(0.1f);
        lerpSpeed = 3f;
    }

    private void Start()
    {
        GoGoStartText("WAVE START","WAVE START");
    }
}
