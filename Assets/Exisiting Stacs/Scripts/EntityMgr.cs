using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr : MonoBehaviour
{
    public static EntityMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public List<StacsEntity> entities;
    private StacsEntity selected;
    private int selectedIndex = 0;

    bool selectPressed = false;

    private void Start()
    {
        selected = entities[selectedIndex];
        //selected.Select();
    }

    // Update is called once per frame
    void Update()
    {/*
        if (selectPressed == false && Input.GetAxis("Select") != 0)
        {
            selected.isSelected = false;
            if(Input.GetAxis("Select") > 0)
            {
                selectedIndex = (selectedIndex < entities.Count - 1) ? selectedIndex + 1 : 0;
            }
            else
            {
                selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : entities.Count - 1;

            }
            selected = entities[selectedIndex];
            selected.isSelected = true; 
            //set camera
            selectPressed = true;
            Debug.Log("Pressed");
        }
        else if (Input.GetAxis("Select") == 0) 
        {
            selectPressed = false;
        }
        */
    }
}
