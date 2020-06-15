using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EGameState
{
    None = 0,
    Menu,
    ShowHelp,
    Briefing,
    Monitoring,
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

    public GameObject EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        State = EGameState.Briefing;
    }
    public bool show = false;
    // Update is called once per frame
    void Update()
    {
        //ProtoPanel.isValid = show;
        UpdateSelectedEntity();
    }

    [ContextMenu("UpdateProto")]
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
    private EGameState _state;
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
            MenuPanel.isValid = (_state == EGameState.Menu);

            if (BriefingPanel.isValid) {

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
        State = EGameState.Menu;
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
