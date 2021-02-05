using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public StacsEntity entity; //public only for ease of debugging

    // Start is called before the first frame update
    void Start()
    {
        //StacsEntity component to work on
        entity = GetComponentInParent<StacsEntity>();
        //Create empty lists
        commands = new List<Command>();
        intercepts = new List<Intercept>();
        moves = new List<Move>();
        trussMoves = new List<TrussMove>();
    }

    //Keep lists of of all moves created
    public List<Move> moves;
    public List<TrussMove> trussMoves;
    public List<Command> commands;
    public List<Intercept> intercepts;

    //Hold waypoints assigned to this robot
    public List<Transform> waypoints;

    // Update is called once per frame
    void Update()
    {
        if (commands.Count > 0) {
            //If current command is done, kill it and start the next
            if (commands[0].IsDone())
            {
                StopAndRemoveCommand(0);
            }
            //Otherwise call tick on current command
            else
            {
                commands[0].Tick();
                commands[0].isRunning = true;
                DecorateAll();
            }
        }
    }

    //Used to place robot at route starting position.
    //Someday the robot will have to find the starting position on its own.
    public void Teleport(Waypoint waypoint)
    {
        GetComponent<StacsEntity>().position = transform.position = waypoint.position;
    }

    //Calls functions to kill current command
    void StopAndRemoveCommand(int index)
    {
        commands[index].Stop();
        if (commands[index] is Intercept)
            intercepts.Remove(commands[index] as Intercept);
        else if (commands[index] is Follow)
            ;
        else if (commands[index] is Move)
            moves.Remove(commands[index] as Move);
        else if (commands[index] is TrussMove)
            trussMoves.Remove(commands[index] as TrussMove);
        commands.RemoveAt(index);
    }

    //Used before setting command
    public void StopAndRemoveAllCommands()
    {
        for (int i = commands.Count - 1; i >= 0; i--) {
            StopAndRemoveCommand(i);
        }
    }

    //When the user shift right-clicks, maintain current commands when adding
    public void AddCommand(Command c)
    {
        //Add command to commands list no matter what
        commands.Add(c);
        //Also add to the appropriate list based on type
        if (c is Intercept)
            intercepts.Add(c as Intercept);
        else if (c is Follow)
            ;
        else if (c is Move)
            moves.Add(c as Move);
        else if (c is TrussMove)
            trussMoves.Add(c as TrussMove);
    }

    //Clear all other commands before adding
    public void SetCommand(Command c)
    {
        StopAndRemoveAllCommands();
        commands.Clear();
        moves.Clear();
        intercepts.Clear();
        trussMoves.Clear();
        AddCommand(c);

    }
    //---------------------------------

    public void DecorateAll()
    {
        Command prior = null;
        foreach (Command c in commands) {
            Decorate(prior, c);
            prior = c;
        }
    }

    //decoration logic (UI logic) in general is always convoluted. Ugh
    public void Decorate(Command prior, Command current)
    {
        if (current.line != null) {
            current.line.gameObject.SetActive(entity.isSelected);
            if (prior == null)
                current.line.SetPosition(0, entity.position);
            else
            {
                current.line.SetPosition(0, prior.line.GetPosition(prior.line.positionCount - 1));
            }

            if (current is Intercept) { //Most specific
                Intercept intercept = current as Intercept;
                if (intercept.isRunning)// 
                    intercept.line.SetPosition(1, intercept.predictedMovePosition);
                else
                    intercept.line.SetPosition(1, intercept.targetEntity.position);
                intercept.line.SetPosition(2, intercept.targetEntity.position);

            } else if (current is Follow) { // Less specific
                Follow f = current as Follow;
                f.line.SetPosition(1, f.targetEntity.position + f.offset);
                f.line.SetPosition(2, f.targetEntity.position);
                //f.line.SetPosition(1, f.predictedMovePosition);

            } else if(current is TrussMove){
                TrussMove tm = current as TrussMove;

                Vector3 norm1 = (prior == null) ? entity.transform.up : (prior as TrussMove).destination.transform.up;
                Vector3 norm2 = tm.destination.transform.up;
                Vector3 p1 = tm.line.GetPosition(0) + norm1 * 0.05f;
                Vector3 p2 = tm.destination.position + norm2 * 0.05f;

                tm.line.positionCount = (norm1 == norm2) ? 2 : 3;

                if(tm.line.positionCount <= 2)
                    tm.line.SetPosition(1, p2 + norm2 * 0.01f);
                else
                {
                    Vector3 edgePoint = Utils.GetEdgePoint(p1, p2, norm1, norm2);
                    Debug.Log("equal norms: " + (norm1 == norm2) + " edgePoint: " + edgePoint);
                    tm.line.SetPosition(1, edgePoint);
                    tm.line.SetPosition(2, p2);
                }
            }
            //Moveposition never changes
        }

        //potential fields lines
        if (!(current is Follow) && !(current is Intercept) && AIMgr.inst.isPotentialFieldsMovement) {
            Move m = current as Move;
            m.potentialLine.SetPosition(0, entity.position);
            Vector3 newpos = Vector3.zero;
            newpos.x = Mathf.Sin(entity.desiredHeading * Mathf.Deg2Rad) * entity.desiredSpeed;
            newpos.z = Mathf.Cos(entity.desiredHeading * Mathf.Deg2Rad) * entity.desiredSpeed;
            newpos *= 20;
            newpos.y = entity.transform.localPosition.y ; // 1;
            m.potentialLine.SetPosition(1, entity.position + newpos);
            m.potentialLine.gameObject.SetActive(entity.isSelected);
        }
    }

}
