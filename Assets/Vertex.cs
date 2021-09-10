using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vertex : MonoBehaviour
{
    // public
    public Text text;
    public vector3 coordinate; // this is actually the transform position

    // private
    private bool selected;

    public void UpdateInfo(string name)
    {
        gameObject.name = name;
        text.text = gameObject.name.Replace("v", "");
    }


}