using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject DDOLobj;

    private void Awake()
    {
        if (GameObject.FindObjectOfType<GameController_DDOL>() == null)
        {
            Instantiate(DDOLobj);
        }
    }
}
