//Elizabeth
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuMgr : MonoBehaviour
{
    public static MenuMgr instance;


    //used in SetMusicVolume
    public AudioMixer musicMixer;
    //used in SetNoiseVolume
    public AudioMixer noiseMixer; 
    //Used to store screen size
    Resolution[] screenSize;
    //used to refrence the UI for the screen size dropdown
    public Dropdown screenDropdown;
    //For the load button
    public Button loadButton;
    public GameObject loadBridgeMenu;
    //This determines if load, delete, or edit was clicked
    public int whichButtonPressed = 0;
    //Text
    public GameObject deleteTitle;
    public GameObject loadTitle;
    public GameObject editTitle;
    //Panels
    public GameObject BridgeSelectionPanel;
    public GameObject TestBridgeSelectionPanel;
    public GameObject PracticeBridgeSelectionPanel;
    public GameObject BridgeCreationSelectionPanel;
    public GameObject SimulationPanel;
    public GameObject TitlePanel;
    public GameObject UnrLogo;
    public string existingBridgeString = "existingBridge";


    //Buttons
    public Button SandboxButton;
    public Button PracticeButton;
    public Button TestButton;

    private void Awake()
    {
        //Sets the screen resolution on load
        screenSize = Screen.resolutions;
        if (instance == null)
        {
            instance = this;
        }
        PlayerPrefs.DeleteAll();
        for(int i = 0; i < 3; i++)
            Destroy(GameObject.Find("Bridge"));
    }

    //To change the screen size
    private void Start()
    {

        //Clears current size options
        screenDropdown.ClearOptions();
        //Turn the screenSize array into a string - Needs to be a string to be used in AddOptions
        List<string> options = new List<string>();

        int currentSize = 0;
        //Goes through array
        for (int i = 0; i < screenSize.Length; i++)
        {
            //creates the string that the user will see, ie 1600 x 900
            string option = screenSize[i].width + " x " + screenSize[i].height;
            options.Add(option);
            //compares the sizes
            if (screenSize[i].width == Screen.currentResolution.width && screenSize[i].height == Screen.currentResolution.height)
            {
                //if they are the same, then it's stored
                currentSize = i;
            }
        }
        //add size options to the dropdown
        screenDropdown.AddOptions(options);
        //updates the current size in unity
        screenDropdown.value = currentSize;
        screenDropdown.RefreshShownValue();

    }

    /// <summary>
    /// Opens up the file explorer, allows user to select bridge, loads bridge into sandbox scene -> Noah
    /// </summary>
    public void SetSandboxBridge()
    {
        var existingBridge = EditorUtility.OpenFilePanel("Select Existing Bridge", "", "txt");
        if (existingBridge.Length != 0)
        {
            PlayerPrefs.SetString(existingBridgeString, existingBridge.ToString());
            PlayerPrefs.SetString("nextScene", "SandboxMode");
            SceneManager.LoadScene("Bridge Creator");
        }
    }

    /// <summary>
    /// Connects to button, loads the scene
    /// </summary>
    /// <param name="sceneName"></param>
    public void LaunchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Quits out of program
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    //---Settings---
    //Audio section:
    //Window->Audio->AudioMixer
    //Music Volume (Music (Settings))
    /// <summary>
    /// Sets volume to music when connected to the audiomixer
    /// </summary>
    /// <param name="musicVol"></param>
    public void SetMusicVolume(float musicVol)
    {
        Debug.Log(musicVol);
        musicMixer.SetFloat("musicVolume", musicVol);
    }
    //Noise Volume (Noise(Settings))
    /// <summary>
    /// Sets volume to noise when connected to the audiomixer
    /// </summary>
    public void SetNoiseVolume(float noiseVol)
    {
        Debug.Log(noiseVol);
        noiseMixer.SetFloat("noiseVolume", noiseVol);
    }

    //Display Section:
    //Graphics  -- updates the graphics (Takes a while to load the change for some reason)
    //Edit->Project Settings...->Quality
    public void SetGraphics(int qualityNum)
    {
        QualitySettings.SetQualityLevel(qualityNum);
    }
    //Screen Size -- updates the screen size
    public void SetScreenSize(int sizeNum)
    {

        Resolution size = screenSize[sizeNum];
        Screen.SetResolution(size.width, size.height, Screen.fullScreen); 
    }
    //Fullscreen -- toggles fullscreen
    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
    //Load Button functionality
    public void LoadButtonFunction()
    {
        loadBridgeMenu.SetActive(true);
        loadTitle.SetActive(true);
        deleteTitle.SetActive(false);
        editTitle.SetActive(false);
        whichButtonPressed = 1; 
    }
    //Delete Button functionality
    public void DeleteButtonFunction()
    {
        loadBridgeMenu.SetActive(true);
        deleteTitle.SetActive(true);
        loadTitle.SetActive(false);
        editTitle.SetActive(false);
        whichButtonPressed = 2;
    }
    //Edit Button functionality
    public void EditButtonFunction()
    {
        loadBridgeMenu.SetActive(true);
        loadTitle.SetActive(false);
        editTitle.SetActive(true);
        deleteTitle.SetActive(false);
        whichButtonPressed = 3;
    }

    //All buttons that bring you to bridge selection
    public void ToBridgeSelectionPanel()
    {
        BridgeSelectionPanel.SetActive(true);
        SimulationPanel.SetActive(false);
        TitlePanel.SetActive(false);
        UnrLogo.SetActive(false);
    }
    //All buttons that bring you to bridge selection
    public void ToTESTBridgeSelectionPanel()
    {
        TestBridgeSelectionPanel.SetActive(true);
        SimulationPanel.SetActive(false);
        TitlePanel.SetActive(false);
        UnrLogo.SetActive(false);
    }
    //All buttons that bring you to bridge selection
    public void ToPracticeBridgeSelectionPanel()
    {
        PracticeBridgeSelectionPanel.SetActive(true);
        SimulationPanel.SetActive(false);
        TitlePanel.SetActive(false);
        UnrLogo.SetActive(false);
    }
    //All buttons that bring you to bridge selection
    public void ToBridgeCreatorSelectionPanel()
    {
        BridgeCreationSelectionPanel.SetActive(true);
        SimulationPanel.SetActive(false);
        TitlePanel.SetActive(false);
        UnrLogo.SetActive(false);
    }

    /// <summary>
    /// Opens the bridge to be edited in bridge creator ->Noah
    /// </summary>
    public void ExploreFiles()
    {
        var existingBridge = EditorUtility.OpenFilePanel("Select Existing Bridge", "", "txt");
        if(existingBridge.Length != 0)
        {
            PlayerPrefs.SetString(existingBridgeString, existingBridge.ToString());
            SceneManager.LoadScene(1);
        }
    }


}