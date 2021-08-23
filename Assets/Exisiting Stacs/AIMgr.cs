using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector3 offset;
    public Vector3 offsetXZ;
    public Vector3 startMousePos;

    public ECommandType commandType = ECommandType.None;
    public StacsEntity targetEntity;
    public LineRenderer offsetLine = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            isOffsetting = true;
            SetStartPosAndTarget();
            offsetLine = LineMgr.inst.CreateCommandOffsetLine(startPos, startPos, startPos);
            if (targetEntity == null) {
                commandType = ECommandType.Move;
                offsetLine.loop = true;
            } else {
                if (Input.GetKey(KeyCode.LeftControl))
                    commandType = ECommandType.Intercept;
                else
                    commandType = ECommandType.Follow;
            }
        }

    


        if (Input.GetMouseButtonUp(1)) {
            isOffsetting = false;
            SetEndPos();
            SetOffset(startPos, Input.mousePosition);
            HandleCommand(startPos, endPos, offset, targetEntity);
            LineMgr.inst.DestroyLR(offsetLine);
        }

        if (isOffsetting) {
            SetOffset(startPos, Input.mousePosition);
            DrawOffset();
        }
    }
    public void OldCommand()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue)) {
            //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
            Vector3 pos = hit.point;
            pos.y = 0;
            Debug.Log(pos);
            targetEntity = FindClosestEntInRadius(pos, rClickRadiusSq);
            if (targetEntity == null) {
                HandleMove(SelectionMgr.inst.selectedEntities, pos);
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
    }

    public void SetStartPosAndTarget()
    {
        startMousePos = Input.mousePosition;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue)) {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
            startPos = hit.point;
            targetEntity = FindClosestEntInRadius(startPos, rClickRadiusSq);
        } else {
            startPos = Vector3.down;
            targetEntity = null;
        }

    }

    public void SetEndPos()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue)) {
            //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
            endPos = hit.point;
        } else {
            endPos = Vector3.down;
        }
    }

    public float altitudeFactor = 0.1f;//should be a function screen height
    public float xzFactor = 0.05f; // function of screen height and width
    public void SetOffset(Vector3 start, Vector3 mousePos)
    {
        offset = Vector3.zero;  //startPos;// Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offsetXZ = Vector3.zero;
        Vector3 diff = mousePos - startMousePos;
        offset.y = diff.y * altitudeFactor;

        offsetXZ.x = diff.x;
        offsetXZ.z = diff.y;
        offsetXZ *= xzFactor;
    }

    public void HandleCommand(Vector3 start, Vector3 end, Vector3 offset, StacsEntity targetEntity) {
        if (start == Vector3.down || end == Vector3.down)
            Debug.Log("Error in start position: " + startPos + " or end position: " + endPos);
        if(targetEntity == null) {
            //Debug.Log("Endposition = " + (start + offset).ToString());
            Debug.Log("Move:" + startPos + " ofsett:" + offset);
            HandleMove(SelectionMgr.inst.selectedEntities, start + offset);
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


    public void HandleMove(List<StacsEntity> entities, Vector3 point)
    {
        foreach (StacsEntity entity in entities) {
            Move m = new Move(entity, point);
            UnitAI uai = entity.GetComponent<UnitAI>();
            AddOrSet(m, uai);
        }
    }

    public void HandleMove(StacsEntity entities, Vector3 point)
    {
        
            Move m = new Move(entities, point);
            UnitAI uai = entities.GetComponent<UnitAI>();
            AddOrSet(m, uai);
        
    }

    void AddOrSet(Command c, UnitAI uai)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            uai.AddCommand(c);
        else
            uai.SetCommand(c);
    }

    public void HandleClear(List<StacsEntity> entities)
    {
        foreach(StacsEntity entity in entities)
        {
            UnitAI uai = entity.GetComponent<UnitAI>();
            uai.StopAndRemoveAllCommands();
        }
      
    }

    public void HandleClear(StacsEntity entities)
    {
  
            UnitAI uai = entities.GetComponent<UnitAI>();
            uai.StopAndRemoveAllCommands();
        

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
}
