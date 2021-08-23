﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public StacsEntity entity;
    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponentInParent<StacsEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
            entity.selectionCircle.SetActive(entity.isSelected);
    }

    private void OnMouseDown()
    {
        //if (Input.GetMouseButtonDown(0)) {
        SelectionMgr.inst.SelectEntity(entity);
        //}
    }

}
