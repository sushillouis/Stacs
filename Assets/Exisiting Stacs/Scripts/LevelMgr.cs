using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr : MonoBehaviour
{
    public GameObject debriefCanvas;

    private void Start()
    {
        debriefCanvas.SetActive(true);
    }

    public void CloseDebrief()
    {
        debriefCanvas.SetActive(false);
    }
}
