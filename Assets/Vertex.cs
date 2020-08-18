using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vertex : MonoBehaviour
{
    public Text text;

    void Awake()
    {
        text.text = gameObject.name;
    }
}
