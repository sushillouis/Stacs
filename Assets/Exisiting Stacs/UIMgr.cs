using System.Collections;
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
    Sandbox,
    Practice,
    Test
}

public enum TestState
{
    EasyTest1,
    EasyTest2,
    MediumTest,
    HardTest,
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

    public Text GradeText;
    public Text DataFeedText;

    //public GameObject myCanvas;

    public Button gameMenuButton;
    public Button briefingPanelOkButton;
    public Button menuHelpButton;
    public Button helpDoneButton;


    public GameObject hintPanel;
    public GameObject resultsPagePanel;
    public GameObject resultsPagePanelFailed;

    
    public TestState mTest;
    public EGameType mTestType;

    // Start is called before the first frame update
    void Start()
    {
        State = EGameState.Briefing;
        CameraViewPanels.Clear();
    }
    public bool show = false;
    // Update is called once per frame
    void Update()
    {
        //ProtoPanel.isValid = show;
        UpdateSelectedEntity();
        CheckForUINavigation();



    }

    public LineGraph Graph;
    public void UpdateDataFeed(StacsEntity ent)
    {
        Graph.ConnectToEntity(ent);
    }

    void CheckForUINavigation()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button7))
        {
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
        if (SelectionMgr.inst != null)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {

                EntityTypeText.text = SelectionMgr.inst.selectedEntity.entityType.ToString();
                EntityNameText.text = SelectionMgr.inst.selectedEntity.name;
                EntityBatteryText.text = SelectionMgr.inst.selectedEntity.batteryState.ToString("F1") + "%";
                EntitySpeedText.text = SelectionMgr.inst.selectedEntity.speed.ToString("F1") + "m/s";
                EntityDesiredSpeedText.text = SelectionMgr.inst.selectedEntity.desiredSpeed.ToString("F1") + "m/s";
                EntityHeadingText.text = SelectionMgr.inst.selectedEntity.heading.ToString("F1") + "deg";
                EntityDesiredHeadingText.text = SelectionMgr.inst.selectedEntity.desiredHeading.ToString("F1") + "deg";
                EntityAltitudeText.text = SelectionMgr.inst.selectedEntity.altitude.ToString("F1") + "m";
                EntityDesiredAltitudeText.text = SelectionMgr.inst.selectedEntity.desiredAltitude.ToString("F1") + "m";
                if (SelectionMgr.inst.selectedEntity.entityType == EntityType.ClimbingRobot)
                    DataFeedText.text = "Data: " + SelectionMgr.inst.selectedEntity.name;
            }
            
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

            //If I was using the HelpPanel and MenuPanel, I would put &&  HelpPanel != null && MenuPanel != null
            if (BriefingPanel != null)
            {
                BriefingPanel.isValid = (_state == EGameState.Briefing);

                //I personally did not use these states -> Since I did not know how
                //HelpPanel.isValid = (_state == EGameState.ShowHelp);
                //MenuPanel.isValid = (_state == EGameState.GameMenu);
            

                //Game Controller UI/Playing switch and Navigation
                switch (_state)
                {
                    case EGameState.Briefing:
                        EventSystem.current.firstSelectedGameObject = briefingPanelOkButton.gameObject;
                        break;
                    case EGameState.Monitoring:
                        EventSystem.current.firstSelectedGameObject = null;
                        briefingPanelOkButton.Select();
                        break;
                   /* case EGameState.GameMenu:
                        EventSystem.current.firstSelectedGameObject = menuHelpButton.gameObject;
                        menuHelpButton.Select();
                        break;
                    case EGameState.ShowHelp:
                        EventSystem.current.firstSelectedGameObject = helpDoneButton.gameObject;
                        helpDoneButton.Select();
                        break;
                    */
                    default:
                        EventSystem.current.firstSelectedGameObject = null;
                        break;
                }

            }
        }
    }

    //on briefing panel Ok button click
    public void StartGame()
    {
        if (mTest == TestState.EasyTest1)
        {
            TestEvalution.inst.easyTest1();
        }
        if (mTest == TestState.MediumTest)
        {
            TestEvalution.inst.mediumTest();
        }
        if (mTest == TestState.HardTest)
        {
            TestEvalution.inst.hardTest();
        }
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
        Debug.Log("Quit");
        Application.Quit();
    }

    public void HandleHelpDone()
    {
        State = EGameState.Monitoring;
    }
    //--------------------------------------------------------------------------------------------
    public RectTransform CameraViewPanelPrefab;
    public RectTransform CameraViewPanelsParent;
    public List<RectTransform> CameraViewPanels;

    public void MakeCameraView(RenderTexture tex)
    {
        RectTransform cameraViewPanel = Instantiate(CameraViewPanelPrefab, CameraViewPanelsParent);
        RawImage ri = cameraViewPanel.GetComponentInChildren<RawImage>();
        if (ri != null)
        {
            ri.texture = tex;
            CameraViewPanels.Add(cameraViewPanel);
        }

    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// This gives functionality to the buttons in Bridge Creator  -> Elizabeth
    /// </summary>
    /// <summary>
    /// This gives functionality to the buttons in Bridge Creator -Turn on mirror button
    /// </summary>
    public void MirrorButtonOn()
    {
        Debug.Log("on");
        BridgeCreator.instance.EnableMirroring();
    }

    /// <summary>
    /// This gives functionality to the buttons in Bridge Creator -Turn off mirror button
    /// </summary>
    public void MirrorButtonOff()
    {
        Debug.Log("off");
        BridgeCreator.instance.DisableMirroring();
    }

    /// <summary>
    /// This gives functionality to the buttons in Bridge Creator -Turn on mirror selected button
    /// </summary>
    public void MirrorSelectedButton()
    {
        Debug.Log("Selected");
        BridgeCreator.instance.MirrorSelectedObjects();
    }

    /// <summary>
    /// This gives functionality to the buttons in Bridge Creator -Deselected object
    /// </summary>
    public void DeselectButton()
    {
        BCSelectionMgr.instance.DisableBoxSelection();
        BCSelectionMgr.instance.DeselectAll();
    }

    /// <summary>
    ///  -Restarts the bridge creator scene
    /// </summary>
    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// Connects resart to button to discard bridge 
    /// </summary>
    public void DiscardBridge()
    {
        RestartScene();
    }

    ///     /// <summary>
    /// Pause game
    /// </summary>
    public void PauseGame()
    {

        Debug.Log("pause");
        Time.timeScale = 0;
    }

    ///     /// <summary>
    /// Resume Game
    /// </summary>
    public void ResumeGame()
    {
        Debug.Log("resume");
        Time.timeScale = 1;
    }

    ///     /// <summary>
    /// Deleted the bridge
    /// </summary>
    public void DeleteBridge()
    {
        Destroy(Bridge.instance);
    }
}
