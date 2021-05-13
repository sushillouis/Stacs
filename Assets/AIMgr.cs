using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ECommandType
{
    Move = 0,
    Follow,
    Intercept,
    WaypointMove,
    None
}

public class AIMgr : MonoBehaviour
{
    public static AIMgr inst;
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 9;// LayerMask.GetMask("Water");
    }

    public bool isPotentialFieldsMovement = false;
    public float potentialDistanceThreshold = 1000;
    public float attractionCoefficient = 500;
    public float attractiveExponent = -1;
    public float repulsiveCoefficient = 60000;
    public float repulsiveExponent = -2.0f;


    public RaycastHit hit;
    public int layerMask;

    public bool isOffsetting = false;
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 normal;
    public Vector3 offset;
    public Vector3 offsetXZ;
    public Vector3 startMousePos;

    public ECommandType commandType = ECommandType.None;
    public StacsEntity targetEntity;
    public LineRenderer offsetLine = null;

    private bool chainCommand = false;

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(1)) {

            StartOffsetting();
        }

        if (Input.GetMouseButtonUp(1)) {
            StopOffsetting();
        }
        */

        if (isOffsetting) {
            SetOffset(startPos, Input.mousePosition);
            DrawOffset();
        }
    }

    public void GiveCommand(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            isOffsetting = true;
            SetStartPosAndTarget();
            offsetLine = LineMgr.inst.CreateCommandOffsetLine(startPos, startPos, startPos);
            if (targetEntity == null)
            {
                commandType = ECommandType.Move;
                offsetLine.loop = true;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    commandType = ECommandType.Intercept;
                else
                    commandType = ECommandType.Follow;
            }
        }
        else if(context.canceled)
        {
            isOffsetting = false;
            SetEndPos();
            SetOffset(startPos, Input.mousePosition);
            HandleCommand(startPos, normal, endPos, offset, targetEntity);
            LineMgr.inst.DestroyLR(offsetLine);
        }
    }

    /*
    public void OldCommand()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue)) {
            //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
            Vector3 pos = hit.point;
            pos.y = 0;
            Debug.Log(pos);
            targetEntity = FindClosestEntInRadius(pos, rClickRadiusSq);
            if (targetEntity == null) {
                HandleMove(SelectionMgr.inst.selectedEntities, pos, normal);
                commandType = ECommandType.Move;
            } else {
                if (Input.GetKey(KeyCode.LeftControl))
                    commandType = ECommandType.Intercept;
                //HandleIntercept(SelectionMgr.inst.selectedEntities, ent);
                else
                    commandType = ECommandType.Follow;
                //HandleFollow(SelectionMgr.inst.selectedEntities, ent);
            }
        } else {
            //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * 1000, Color.white, 2);
        }
    }*/

    public void SetStartPosAndTarget()
    {
        if (SettingsMgr.vrEnabled)
        {
            ControlMgr.inst.rightHand.DoRaycast(out hit);
            if(hit.point != Vector3.zero)
            {
                startPos = hit.point;
                normal = hit.normal;
                targetEntity = FindClosestEntInRadius(startPos, rClickRadiusSq);
            }
            else
            {
                startPos = Vector3.down;
                targetEntity = null;
            }
        }
        else
        {
            startMousePos = Input.mousePosition;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
                startPos = hit.point;
                normal = hit.normal;
                targetEntity = FindClosestEntInRadius(startPos, rClickRadiusSq);
            }
            else
            {
                startPos = Vector3.down;
                targetEntity = null;
            }
        }
    }

    public void SetEndPos()
    {
        if(SettingsMgr.vrEnabled)
        {
            ControlMgr.inst.rightHand.DoRaycast(out hit);
            if(hit.point != Vector3.zero)
            {
                endPos = hit.point;
            }
            else
            {
                endPos = Vector3.down;
            }
        }
        else
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue))
            {
                //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
                endPos = hit.point;
            }
            else
            {
                endPos = Vector3.down;
            }
        }
    }

    public float altitudeFactor = 0.1f;//should be a function screen height
    public float xzFactor = 0.05f; // function of screen height and width
    public void SetOffset(Vector3 start, Vector3 mousePos)
    {
        offset = Vector3.zero;  //startPos;// Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offsetXZ = Vector3.zero;
        if(!SettingsMgr.vrEnabled)
        {
            Vector3 diff = mousePos - startMousePos;
            offset.y = diff.y * altitudeFactor;

            offsetXZ.x = diff.x;
            offsetXZ.z = diff.y;
            offsetXZ *= xzFactor;
        }
        else
        {
            Transform hand = ControlMgr.inst.rightHand.transform;
            Vector3 diff = startPos - ControlMgr.inst.rightHand.transform.position;
            Vector3 norm = Vector3.ProjectOnPlane(diff, Vector3.up);
            Plane target = new Plane(norm, startPos);
            Ray ray = new Ray(hand.position, hand.forward);
            target.Raycast(ray, out float dist);
            offset.y = (hand.position + (hand.forward * dist)).y - startPos.y;
        }
    }

    public void HandleCommand(Vector3 start, Vector3 norm, Vector3 end, Vector3 offset, StacsEntity targetEntity) {
        if (start == Vector3.down || end == Vector3.down)
            Debug.Log("Error in start position: " + startPos + " or end position: " + endPos);
        if(targetEntity == null) {
            //Debug.Log("Endposition = " + (start + offset).ToString());
            HandleMove(SelectionMgr.inst.selectedEntities, start, offset, normal);
        } else {
            if (Input.GetKey(KeyCode.LeftControl))
                HandleIntercept(SelectionMgr.inst.selectedEntities, targetEntity);
            else
                HandleFollow(SelectionMgr.inst.selectedEntities, targetEntity, offsetXZ);
        }

    }

    public void DrawOffset()
    {
        switch (commandType) {
            case ECommandType.Move:
                offsetLine.SetPosition(0, SelectionMgr.inst.selectedEntity.transform.position);
                offsetLine.SetPosition(1, startPos);
                offsetLine.SetPosition(2, startPos + offset);
                break;
            case ECommandType.Follow:
                offsetLine.SetPosition(0, SelectionMgr.inst.selectedEntity.transform.position);
                offsetLine.SetPosition(1, targetEntity.transform.position);
                offsetLine.SetPosition(2, targetEntity.transform.position + offsetXZ);
                break;
            default:
                break;
        }
    }


    public void HandleMove(List<StacsEntity> entities, Vector3 start, Vector3 offset, Vector3 norm)
    {
        foreach (StacsEntity entity in entities) {
            if(entity.GetComponent<ClimbingPhysics>() != null)
            {
                TrussMove tm = new TrussMove(entity, new Waypoint(start, norm));
                UnitAI uai = entity.GetComponent<UnitAI>();
                AddOrSet(tm, uai);
            }
            else
            {
                Move m = new Move(entity, start + offset);
                UnitAI uai = entity.GetComponent<UnitAI>();
                AddOrSet(m, uai);
            }
        }
    }

    void AddOrSet(Command c, UnitAI uai)
    {
        if (chainCommand)
            uai.AddCommand(c);
        else
            uai.SetCommand(c);
    }

    public void SetChainCommand(InputAction.CallbackContext context)
    {
        if (context.started)
            chainCommand = true;
        else if (context.canceled)
            chainCommand = false;
    }

    public void HandleFollow(List<StacsEntity> entities, StacsEntity ent, Vector3 offset)
    {
        foreach (StacsEntity entity in SelectionMgr.inst.selectedEntities) {
            Follow f = new Follow(entity, ent, offset);
            UnitAI uai = entity.GetComponent<UnitAI>();
            AddOrSet(f, uai);
        }
    }

    void HandleIntercept(List<StacsEntity> entities, StacsEntity ent)
    {
        foreach (StacsEntity entity in SelectionMgr.inst.selectedEntities) {
            Intercept intercept = new Intercept(entity, ent);
            UnitAI uai = entity.GetComponent<UnitAI>();
            AddOrSet(intercept, uai);
        }

    }

    public float rClickRadiusSq = 100;
    public StacsEntity FindClosestEntInRadius(Vector3 point, float rsq)
    {
        StacsEntity minEnt = null;
        float min = float.MaxValue;
        foreach (StacsEntity ent in EntityMgr.inst.entities) {
            if (ent.entityType == EntityType.Camera) continue;
            float distanceSq = (ent.transform.position - point).sqrMagnitude;
            if (distanceSq < rsq) {
                if (distanceSq < min) {
                    minEnt = ent;
                    min = distanceSq;
                }
            }
        }
        return minEnt;
    }

    
    void SetupWaypoints()
    {
        foreach(StacsEntity ent in EntityMgr.inst.entities)
        {
            if(ent.GetComponent<UnitAI>() != null)
            {
                foreach(Transform t in ent.GetComponent<UnitAI>().waypoints)
                {
                    if(ent.GetComponent<ClimbingPhysics>() != null)
                    {
                        TrussMove tm = new TrussMove(ent, new Waypoint(t));
                        UnitAI uai = ent.GetComponent<UnitAI>();
                        uai.AddCommand(tm);
                    }
                    else
                    {
                        Move m = new Move(ent, t.position);
                        UnitAI uai = ent.GetComponent<UnitAI>();
                        uai.AddCommand(m);
                    }
                }
                ent.GetComponent<UnitAI>().waypoints.Clear();
            }
        }
    }
}
