using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_DDOL : MonoBehaviour
{
    public int round = 1;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
