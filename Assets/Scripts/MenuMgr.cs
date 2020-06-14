using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMgr : MonoBehaviour
{
    public void LaunchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}