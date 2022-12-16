using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool Started;

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        Application.targetFrameRate = (int)Screen.mainWindowDisplayInfo.refreshRate.value;
    }

    private void Update()
    {
        if (Started)
        {
            if (Time.timeScale <= 2)
                Time.timeScale += Time.deltaTime * Time.deltaTime / 2f;
        }

        Debug.Log(Time.timeScale);
    }
}
