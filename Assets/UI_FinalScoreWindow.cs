using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;


public class UI_FinalScoreWindow : MonoBehaviour
{
    GameController_DDOL _gc;
    [SerializeField] TextMeshProUGUI _txt;
    [SerializeField] TMP_InputField input;
    bool Submitted = false;

    private void Start()
    {
        _gc = FindObjectOfType<GameController_DDOL>();
    }
    private void OnGUI()
    {
        _txt.text = _gc.score + "";
    }

    public void OnLeavingScreen()
    {
        _gc.Reset();
    }

    public void OnSubmitButtonClicked()
    {
        if (Submitted) return;

        Submitted = true;
        StartCoroutine(SendScoreToLeaderboard(input.text, _gc.score));
    }

    IEnumerator SendScoreToLeaderboard(string Name, double Score)
    {
        string uri = "infamy.dev/highscore/add?id=kerboblin-crunch&name=" + Name + "&score=" + Score;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                // Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}
