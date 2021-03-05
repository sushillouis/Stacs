using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuMgr : MonoBehaviour
{
    public Vector3 offScreenPos;
    public RectTransform mainMenuPanel;
    public RectTransform vrSelectionPanel;

    public Button vrButton;
    public Button pcButton;

    public Text title;
    public Text countdownText;

    private int mode = 0;
    private int counterFontSize = 0;
    public float secondsBeforeLaunch;
    private GameObject eventSystem;

    public void Awake()
    {
        mainMenuPanel.position += offScreenPos;



        title.text = "Launching PC mode in:";

        counterFontSize = countdownText.fontSize;
    }

    private void Start()
    {
        DeselectAll();
        pcButton.Select();
        pcButton.OnSelect(null);
    }

    public void LaunchSelected()
    {
        if (mode == 0)
        {
            ShowMenu();
        } else if (mode == 1)
        {
            ShowMenu();
            EnableVR();
        }
    }

    private void DeselectAll()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectPC()
    {
        mode = 0;
        title.text = "Launching PC mode in:";
/*        pcButton.Select();
        pcButton.OnSelect(null);*/
    }


    public void SelectVR()
    {
        mode = 1;
        title.text = "Launching VR mode in:";
/*        vrButton.Select();
        vrButton.OnSelect(null);*/
    }

    public void ShowMenu()
    {
        mainMenuPanel.position -= offScreenPos;
        vrSelectionPanel.position += offScreenPos;
    }

    public void EnableVR()
    {
        SettingsMgr.vrEnabled = true;
    }

    public void LaunchScene(int bridge)
    {
        SettingsMgr.bridge = bridge;
        SceneManager.LoadScene("SteelTrussBridge");
    }

    private void Update()
    {
        if (secondsBeforeLaunch > 0)
        {
            secondsBeforeLaunch -= Time.deltaTime;
            countdownText.fontSize =  Mathf.CeilToInt((1f - (secondsBeforeLaunch - Mathf.Floor(secondsBeforeLaunch))) * (float) counterFontSize); // adjustst the size of the font 0 - 100% based on the time inbetween seconds
            countdownText.text = Mathf.CeilToInt(secondsBeforeLaunch).ToString();

            if (secondsBeforeLaunch <= 0)
            {
                LaunchSelected();
            }
        } 
    }



}