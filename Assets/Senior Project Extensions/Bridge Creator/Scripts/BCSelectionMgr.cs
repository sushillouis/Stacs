using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// establishes methods for selecting single and multiple objects in the bridge creator scene
/// </summary>
public class BCSelectionMgr : MonoBehaviour
{
    public static BCSelectionMgr instance;

    public float boxBorderThickness = 2;
    public Color boxBorderColor;
    public Color boxFillColor;

    public bool boxSelecting;

    private Vector2 startPos;
    private Vector2 endPos;
    public List<Transform> selectedObjects;
    /// <summary>
    /// called once upon application initialization
    /// </summary>
    private void Awake()
    {
        //establishes BCSelection manager as a referenceable singleton
        if (instance == null)
        {
            instance = this;
            selectedObjects = new List<Transform>();
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }
    /// <summary>
    /// determines if a given GameObject is within bounds for selection
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns> only returns true if the GameObject is within bounds </returns>
    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        var camera = Camera.main;
        var viewportBounds = Box.GetViewportBounds(camera, startPos, Input.mousePosition);
        if(gameObject.transform.position.z == -4)
        {
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }
        return false;
        
    }
    /// <summary>
    /// determines if a vector3 point in the scene is within selection bounds
    /// </summary>
    /// <param name="p"></param>
    /// <returns> only returns true if the point is within selection bounds </returns>
    public bool IsWithinSelectionBoundsVec(Vector3 p)
    {
        var camera = Camera.main;
        var viewportBounds = Box.GetViewportBounds(camera, startPos, Input.mousePosition);
        if(p.z == -4)
        {
            return viewportBounds.Contains(camera.WorldToViewportPoint(p));
        }
        return false;
    }
    /// <summary>
    /// sets box selection methods to enabled
    /// </summary>
    public void EnableBoxSelection()
    {
            startPos = Input.mousePosition;
            boxSelecting = true;
    }
    /// <summary>
    /// disables box selection methods
    /// </summary>
    public void DisableBoxSelection()
    {
        boxSelecting = false;
        bool somethingSelected = false;
        foreach (var pair in Bridge.instance.vertices)
        {
            if (IsWithinSelectionBounds(pair.Value))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }

        foreach (var pair in Bridge.instance.edges)
        {
            if (IsWithinSelectionBounds(pair.Value.gameObject))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }

        if (!somethingSelected)
        {
            DeselectAll();
        }
    }
    /// <summary>
    /// disables higlighting of box selection
    /// </summary>
    public void DisableHighLight()
    {
        boxSelecting = false;
        bool somethingSelected = false;
        foreach (var pair in Bridge.instance.vertices)
        {
            if (IsWithinSelectionBounds(pair.Value))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }

        foreach (var pair in Bridge.instance.edges)
        {
            if (IsWithinSelectionBounds(pair.Value.gameObject))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }
    }

    /// <summary>
    /// updates a selection container based on selected objects
    /// </summary>
    /// <param name="trans"></param>
    /// <returns> whether adjusted objects are selected or not selected </returns>
    public bool AdjustSelectedObjects(Transform trans)
    {
        if (selectedObjects.Contains(trans))
        {
            selectedObjects.Remove(trans);
            Renderer rend = trans.GetComponent<Renderer>();
            rend.material.color = Color.white;
            return false;
        }
        else
        {
            selectedObjects.Add(trans);
            Renderer rend = trans.GetComponent<Renderer>();
            rend.material.color = Color.green;
            return true;
        }
    }
    /// <summary>
    /// updates the list of selected objects based upon the slection or box selection
    /// </summary>
    /// <param name="list"></param>
    public void AdjustListObjects(List<Transform> list)
    {
        foreach(Transform trans in list)
        {
     
                selectedObjects.Add(trans);
                Renderer rend = trans.GetComponent<Renderer>();
                rend.material.color = Color.green;
           
            
        }
      
    }
    /// <summary>
    /// allows all selected objects to be unselected
    /// </summary>
    public void DeselectAll()
    {

        Renderer rend;
        for (; selectedObjects.Count > 0;)
        {
            rend = selectedObjects[0].GetComponent<Renderer>();
            rend.material.color = Color.white;
            if (selectedObjects[0].transform.tag == "Truss")
            {
                print(selectedObjects[0].transform.position);
            }
            selectedObjects.RemoveAt(0);

        }
        selectedObjects.Clear();

    }
    /// <summary>
    /// method for adjusting whether an object is part of a group selection or whether to deselect all 
    /// </summary>
    public void UnitSelect()
    {
        RaycastHit hit = BridgeCreator.instance.RaycastFromMouse();
        bool somethingSelected = false;
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.layer == 9)
            {
                somethingSelected = AdjustSelectedObjects(hit.transform);
            } 
        }

        if (!somethingSelected)
        {
            DeselectAll();
        }
    }

    /// <summary>
    /// creates a visible selection box for the user
    /// </summary>
    void OnGUI()
    {
        if (boxSelecting)
        {
            // Create a rect from both mouse positions
            var rect = Box.GetScreenRect(startPos, Input.mousePosition);
            Box.DrawScreenRect(rect, boxFillColor);
            Box.DrawScreenRectBorder(rect, boxBorderThickness, boxBorderColor);
        }

    }
}
