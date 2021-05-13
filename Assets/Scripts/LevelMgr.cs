using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr : MonoBehaviour
{
    public bool VREnabled;

    public GameObject oculus;

    public Canvas canvas;

    public GameObject debriefCanvas;

    private void Start()
    {
        VREnabled = SettingsMgr.vrEnabled;
        oculus.SetActive(VREnabled);
        canvas.renderMode = VREnabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        debriefCanvas.SetActive(true);
    }

    public void CloseDebrief()
    {
        debriefCanvas.SetActive(false);
    }
}
