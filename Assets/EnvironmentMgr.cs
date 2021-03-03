using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentMgr : MonoBehaviour
{
    public List<GameObject> environments;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject g in environments)
        {
            g.SetActive((g == environments[SettingsMgr.bridge]) ? true : false);
        }
    }
}
