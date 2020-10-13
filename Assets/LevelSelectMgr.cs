using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum EMenuState
{
    None = 0,
    MainMenu,
    ModeMenu
}

public class LevelSelectMgr : MonoBehaviour
{
    public static LevelSelectMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public StacsPanel MainMenuPanel;
    public StacsPanel ModeMenuPanel;

    public Button LevelButton_0;
    public Button ModeButton_0;

    // Start is called before the first frame update
    void Start()
    {
        State = EMenuState.MainMenu;
    }

    public EMenuState priorState;
    public EMenuState _state = EMenuState.None;

    //[System.Serializable]
    public EMenuState State
    {
        get { return _state; }
        set
        {
            priorState = _state;
            _state = value;

            MainMenuPanel.isValid = (_state == EMenuState.MainMenu);
            ModeMenuPanel.isValid = (_state == EMenuState.ModeMenu);

            //Game Controller UI/Playing switch and Navigation
            switch (_state) {
                case EMenuState.MainMenu:
                    EventSystem.current.firstSelectedGameObject = LevelButton_0.gameObject;
                    break;
                case EMenuState.ModeMenu:
                    EventSystem.current.firstSelectedGameObject = null;
                    ModeButton_0.Select();
                    break;
                default:
                    EventSystem.current.firstSelectedGameObject = null;
                    break;
            }


        }
    }

    public void HandleModeMenu()
    {
        State = EMenuState.ModeMenu;
    }

    public void HandleMenuBack()
    {
        State = priorState;
    }

    //--------------------------------------------------------------------------------------------
}
