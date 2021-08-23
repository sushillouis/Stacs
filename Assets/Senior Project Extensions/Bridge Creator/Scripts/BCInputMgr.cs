using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class BCInputMgr : MonoBehaviour
{
    public static BCInputMgr instance;
    public bool boxState = false; //false to indicate normal left click true to indicate box slection
    public bool endMove = false;
    public GameObject SaveBridgePanel;

    private void Awake()
    {
        // this keeps instance a singlton
        if (instance == null)
        {
            instance = this;
        } else if (instance != null)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
                if (Input.GetMouseButtonDown(0) && !boxState) // Left mouse button
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {

                        BCSelectionMgr.instance.UnitSelect();
                    }
                    else
                    {
                        BridgeCreator.instance.AttemptAddVertexAndConnectEdge();
                    }
                }

                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) == false) // fires every frame
                {

                    BridgeCreator.instance.AttemptMoveSelected();
                    endMove = true;
                    boxState = false;
                }

            }


            if (Input.GetMouseButtonUp(0) && endMove)
            {
                BridgeCreator.instance.EndMovingSelected();
                endMove = false;
                //BridgeCreator.instance.AdjustDraggedVertex(hit);
            }


            if (Input.GetMouseButtonDown(1)) // Right mouse button
            {
                BridgeCreator.instance.AttemptRemoveEdgeOrVertex();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                boxState = true;

            }
            if (Input.GetMouseButtonDown(0) && boxState && Input.GetKey(KeyCode.LeftControl) == false)
            {

                BCSelectionMgr.instance.EnableBoxSelection();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                //leftClickState = false;
                BCSelectionMgr.instance.DisableHighLight();
                //  BCSelectionMgr.instance.DisableBoxSelection();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                BridgeCreator.instance.EnableMirroring();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                BridgeCreator.instance.DisableMirroring();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                List<Transform> newObjects = new List<Transform>(BridgeCreator.instance.CopySelectedObjects());
                BCSelectionMgr.instance.DeselectAll();
                BCSelectionMgr.instance.AdjustListObjects(newObjects);


            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                BridgeCreator.instance.MirrorSelectedObjects();
                BCSelectionMgr.instance.DeselectAll();

            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BCSelectionMgr.instance.DisableBoxSelection();
                BCSelectionMgr.instance.DeselectAll();

            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                BridgeCreator.instance.DeleteSelectedObjects();
                BCSelectionMgr.instance.DeselectAll();
                print("Counter:" + BCSelectionMgr.instance.selectedObjects.Count);

            }
    }

    /// <summary>
    /// checks if the mouse is over a UI object so we can aproperiately handle the input
    /// </summary>
    /// <returns>if the mouse is over a UI object</returns>
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
