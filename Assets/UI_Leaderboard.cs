using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UI_Leaderboard : MonoBehaviour
{
    [SerializeField] List<UI_Leader> LeadersUI = new List<UI_Leader>();

    void Start()
    {
        StartCoroutine("GetLeaderboard");
    }

    IEnumerator GetLeaderboard()
    {
        string uri = "infamy.dev/highscore/json?id=kerboblin-crunch";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    InfamyResult res = JsonUtility.FromJson<InfamyResult>("{\"result\":" + webRequest.downloadHandler.text + "}");

                    int i = 0;
                    foreach (LeaderboardEntry entry in res.result) {
                        if (LeadersUI.Count > i)
                        {
                            LeadersUI[i].SetLeader(entry.name, entry.score + "");
                        }
                        i += 1;
                    }
                    break;
            }
        }
    }
}
