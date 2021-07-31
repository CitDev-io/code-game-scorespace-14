using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_DDOL : MonoBehaviour
{
    public int round = 1;
    public int totalKills = 0;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Reset()
    {
        round = 1;
        totalKills = 0;
    }

    public void OnMonsterKilled()
    {
        totalKills += 1;
    }
}
