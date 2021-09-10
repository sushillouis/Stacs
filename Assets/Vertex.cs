using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vertex : MonoBehaviour
{
    public Text text;

    public void UpdateInfo(string name)
    {
        gameObject.name = name;
        text.text = gameObject.name.Replace("v", "");
    }
}