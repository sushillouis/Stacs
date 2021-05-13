using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuMgr : MonoBehaviour
{
    public Canvas mainCanvas;
    public Vector3 offScreenPos;
    public Vector3 vrCanvasPos;
    public Vector3 vrCanvasRotation;
    public Vector3 vrCanvasScale;
    public RectTransform mainMenuPanel;
    public RectTransform vrSelectionPanel;

    public Button vrButton;
    public Button pcButton;
    public Button defaultMenuButton;

    public Text title;
    public Text countdownText;

    private int mode = 1;
    private int counterFontSize = 0;
    public float secondsBeforeLaunch;
    private GameObject eventSystem;
    private bool autoLaunch;

    public void Awake()
    {
        autoLaunch = true;
        /*
        if (Time.realtimeSinceStartup > 5.0f)
        {
            autoLaunch = false;
            vrSelectionPanel.position += offScreenPos;
            if (SettingsMgr.vrEnabled)
                SetupVRCanvas();
        }

        else
        {*/
            mainMenuPanel.position += offScreenPos;
        //}

        title.text = "Launching VR mode in:";

        counterFontSize = countdownText.fontSize;
    }

    private void Start()
    {
        DeselectAll();
        vrButton.Select();
        vrButton.OnSelect(null);
    }

    public void LaunchSelected()
    {
        if (mode == 0)
        {
            ShowMenu();
        } else if (mode == 1)
        {
            ShowMenu();
            SetupVRCanvas();
            EnableVR();
        }
        defaultMenuButton.Select();
        defaultMenuButton.OnSelect(null);
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

    public void SetupVRCanvas()
    {
        mainCanvas.renderMode = RenderMode.WorldSpace;
        mainCanvas.transform.position = vrCanvasPos;
        mainCanvas.transform.eulerAngles = vrCanvasRotation;
        mainCanvas.transform.localScale = vrCanvasScale;
    }

    public void EnableVR()
    {
        SettingsMgr.vrEnabled = true;
    }

    public void LaunchScene(int environment)
    {
        SettingsMgr.environment = environment;
        SceneManager.LoadScene("SteelTrussBridge");
    }

    private void Update()
    {
        if(autoLaunch)
        {
            if (secondsBeforeLaunch > 0)
            {
                secondsBeforeLaunch -= Time.deltaTime;
                countdownText.fontSize = Mathf.CeilToInt((1f - (secondsBeforeLaunch - Mathf.Floor(secondsBeforeLaunch))) * (float)counterFontSize); // adjustst the size of the font 0 - 100% based on the time inbetween seconds
                countdownText.text = Mathf.CeilToInt(secondsBeforeLaunch).ToString();

                if (secondsBeforeLaunch <= 0)
                {
                    LaunchSelected();
                }
            }
        }
    }
}