using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StacsPanel : MonoBehaviour
{

    public Vector3 EditPosition;
    public Vector3 ShowPosition;

    private void Awake()
    {
        EditPosition = transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        //EditPosition = transform.localPosition;
    }

    public bool show = false;
    public bool isValid
    {
        get { return show; }
        set
        {
            show = value;
            if (show)
                transform.localPosition = Vector3.zero;
            else
                transform.localPosition = EditPosition;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }



}
