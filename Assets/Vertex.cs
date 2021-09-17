using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vertex : MonoBehaviour
{
    // public
    public Text text;

    // private
    private bool selected;

    public void UpdateInfo(string name)
    {
        gameObject.name = name;
        text.text = gameObject.name.Replace("v", "");
    }


}