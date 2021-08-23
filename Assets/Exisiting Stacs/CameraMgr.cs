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

    public GameObject CameraRoot;

    // Start is called before the first frame update
    void Start()
    {
        cameraEntity = EntityMgr.inst.entities[entityIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button1) || Input.GetKeyUp(KeyCode.Backslash) ) {
            SelectNextCameraEntity();
        }
        //FollowEntity();
    }
    public int entityIndex = 0; //This is RTS camera
    public StacsEntity cameraEntity;
    public RenderTexture RenderTextureToCopyForEntityCams;

    public void SelectNextCameraEntity()
    {
        entityIndex = (entityIndex >= EntityMgr.inst.entities.Count - 1 ? 0 : entityIndex + 1);
        cameraEntity = EntityMgr.inst.entities[entityIndex];
        SwitchViewTo(EntityMgr.inst.entities[entityIndex]);
    }

    // Follow camera code - if needed
    public Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;
    public void FollowEntity()
    {
        CameraRoot.transform.position =
            Vector3.SmoothDamp(CameraRoot.transform.position,
                               cameraEntity.cameraRig.transform.position,
                               ref velocity, smoothTime);

        CameraRoot.transform.rotation =
            Quaternion.Lerp(CameraRoot.transform.rotation,
                               cameraEntity.cameraRig.transform.rotation, smoothTime);

    }

    // reparent camera code currently being used
    public void SwitchViewTo(StacsEntity ent) {
        CameraRoot.transform.SetParent(ent.cameraRig.transform, false);
        foreach (Transform t in ent.cameraRig.GetComponentsInChildren<Transform>()) {
            //Debug.Log(t.gameObject.name);
            if (t.gameObject.name.Contains("CameraRig")) continue;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }
    }




}
