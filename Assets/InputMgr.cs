using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : MonoBehaviour
{
    public bool increaseDS = false;
    public bool decreaseDS = false;

    public bool increaseDH = false;
    public bool decreaseDH = false;

    public void IncreaseDS()
    {
        increaseDS = true;
    }

    public void DecreaseDS()
    {
        decreaseDS = true;
    }

    public void IncreaseDH()
    {
        increaseDH = true;
    }

    public void DecreaseDH()
    {
        decreaseDH = true;
    }
}
