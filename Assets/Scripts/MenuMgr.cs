using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuMgr : MonoBehaviour
{
    public Vector3 offScreenPos;
    public RectTransform mainMenuPanel;
    public RectTransform vrSelectionPanel;
    public Text countdownText;

    public void Start()
    {
        mainMenuPanel.position += offScreenPos;
        StartCoroutine("Countdown");
    }

    public void GoToMenu(bool vr)
    {
        StopAllCoroutines();
        SettingsMgr.vrEnabled = vr;
        mainMenuPanel.position -= offScreenPos;
        vrSelectionPanel.position += offScreenPos;
    }

    public void LaunchScene(int bridge)
    {
        SettingsMgr.bridge = bridge;
        SceneManager.LoadScene("SteelTrussBridge");
    }

    IEnumerator Countdown()
    {
        while(Time.time <= 5.0f)
        {
            countdownText.text = (5.0f - Time.time).ToString("F0");
            yield return null;
        }
        GoToMenu(true);
    }
}