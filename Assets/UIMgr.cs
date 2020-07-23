﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum EGameState
{
    None = 0,
    GameMenu,
    ShowHelp,
    Briefing,
    Monitoring,
    UINavigating,
    AfterActionReview,
}


public enum EGameType
{
    None = 0,
    Practice,
    Quiz
}

public class UIMgr : MonoBehaviour
{
    public static UIMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public StacsPanel BriefingPanel;
    public StacsPanel MenuPanel;
    public StacsPanel HelpPanel;

    public RectTransform TopLevelMenuPanel;

    //Entity state left panel update
    public Text EntityTypeText; public Text EntityNameText;
    public Text EntityBatteryText;
    public Text EntitySpeedText; public Text EntityDesiredSpeedText;
    public Text EntityHeadingText; public Text EntityDesiredHeadingText;
    public Text EntityAltitudeText; public Text EntityDesiredAltitudeText;

    //public GameObject myCanvas;

    public Button gameMenuButton;
    public Button briefingPanelOkButton;
    public Button menuHelpButton;
    public Button helpDoneButton;

    public Transform viewPanelParent;
    public GameObject viewPanel;

    // Start is called before the first frame update
    void Start()
    {
        State = EGameState.Briefing;
        foreach(StacsEntity ent in EntityMgr.inst.entities)
        {
            GameObject panel = Instantiate(viewPanel, viewPanelParent);
            panel.transform.GetChild(0).GetComponent<RawImage>().texture = ent.renderTexture;
        }
    }
    public bool show = false;
    // Update is called once per frame
    void Update()
    {
        //ProtoPanel.isValid = show;
        UpdateSelectedEntity();
        CheckForUINavigation();
    }

    void CheckForUINavigation()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button7)) {
            State = EGameState.GameMenu;
        }

    }

    [ContextMenu("UpdateProto")] //For testing Stacs Panels
    public void UpdateProto()
    {
        BriefingPanel.isValid = show;
    }


    public void UpdateSelectedEntity()
    {
        if (SelectionMgr.inst.selectedEntity != null) {
            EntityTypeText.text = SelectionMgr.inst.selectedEntity.entityType.ToString();
            EntityNameText.text = SelectionMgr.inst.selectedEntity.name;
            EntityBatteryText.text = SelectionMgr.inst.selectedEntity.batteryState + "%";
            EntitySpeedText.text = SelectionMgr.inst.selectedEntity.speed.ToString("F1") + "m/s";
            EntityDesiredSpeedText.text = SelectionMgr.inst.selectedEntity.desiredSpeed.ToString("F1") + "m/s";
            EntityHeadingText.text = SelectionMgr.inst.selectedEntity.heading.ToString("F1") + "deg";
            EntityDesiredHeadingText.text = SelectionMgr.inst.selectedEntity.desiredHeading.ToString("F1") + "deg";
            EntityAltitudeText.text = SelectionMgr.inst.selectedEntity.altitude.ToString("F1") + "m";
            EntityDesiredAltitudeText.text = SelectionMgr.inst.selectedEntity.desiredAltitude.ToString("F1") + "m";
        }
    }

    public EGameState priorState;
    public EGameState _state = EGameState.None;
    //[System.Serializable]
    public EGameState State
    {
        get { return _state; }
        set
        {
            priorState = _state;
            _state = value;

            BriefingPanel.isValid = (_state == EGameState.Briefing);
            HelpPanel.isValid = (_state == EGameState.ShowHelp);
            MenuPanel.isValid = (_state == EGameState.GameMenu);

            //Game Controller UI/Playing switch and Navigation
            switch (_state) {
                case EGameState.Briefing:
                    EventSystem.current.firstSelectedGameObject = briefingPanelOkButton.gameObject;
                    break;
                case EGameState.Monitoring:
                    EventSystem.current.firstSelectedGameObject = null;
                    briefingPanelOkButton.Select();
                    break;
                case EGameState.GameMenu:
                    EventSystem.current.firstSelectedGameObject = menuHelpButton.gameObject;
                    menuHelpButton.Select();
                    break;
                case EGameState.ShowHelp:
                    EventSystem.current.firstSelectedGameObject = helpDoneButton.gameObject;
                    helpDoneButton.Select();
                    break;
                default:
                    EventSystem.current.firstSelectedGameObject = null;
                    break;
            }


        }
    }

    //on briefing panel Ok button click
    public void StartGame()
    {
        //Debug.Log("GameStarting");
        State = EGameState.Monitoring;
    }

    public void HandleMenuHelp()
    {
        State = EGameState.ShowHelp;
    }

    public void HandleMenu()
    {
        State = EGameState.GameMenu;
    }

    public void HandleMenuBack()
    {
        State = priorState;
    }

    public void HandleMenuQuitTask()
    {
        SceneManager.LoadScene(0);
    }

    public void HandleMenuQuitToOS()
    {
        Application.Quit();
    }

    public void HandleHelpDone()
    {
        State = EGameState.Monitoring;
    }
}
