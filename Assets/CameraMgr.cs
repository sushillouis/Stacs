using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMgr : MonoBehaviour
{
    public static CameraMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public GameObject CameraRigRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetKeyUp(KeyCode.Backslash) ) {
            NextCamera();
        }
    }
    public int entityIndex = 0; //This is RTS camera

    public void NextCamera()
    {
        entityIndex = (entityIndex >= EntityMgr.inst.entities.Count - 1 ? 0 : entityIndex + 1);
        SwitchView(EntityMgr.inst.entities[entityIndex]);
    }

    public void SwitchView(StacsEntity ent) {
        CameraRigRoot.transform.SetParent(ent.cameraRig.transform, false);
        foreach (Transform t in ent.cameraRig.GetComponentsInChildren<Transform>()) {
            //Debug.Log(t.gameObject.name);
            if (t.gameObject.name.Contains("CameraRig")) continue;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }
    }




}
