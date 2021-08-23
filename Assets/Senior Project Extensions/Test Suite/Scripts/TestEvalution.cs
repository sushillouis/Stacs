using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// a class that contains the functionallity for scripted test scences 
/// </summary>
public class TestEvalution : MonoBehaviour
{
    public static TestEvalution inst;
    public List<StacsEntity> trackedRobots;
    private List<Vector3> trackedRobotPreviousPos;

    //to cac total grade average grades from all secions
    public float totalRobotTravelDistance = 0;
    public float targetTotalRobotTravelDistance = 0;

    public float totalTime = 0;
    public float targetTime = 0;

    public float totalBatteryUsage = 0;
    public float targetTotalBatteryUsage = 0;

    public int totalDefectsFound = 0;
    public int actualDefects = 0;

    // TODO consider loading the targets

    public bool testMode;
    private bool testEnded = false;
    private bool errorOccured = false;

    public string resultsPath = "Results/results.txt";

    /// <summary>
    /// this function tracks the robots sttarting positione 
    /// </summary>
    private void Awake()
    {
        inst = this;
        //this will start the robot moving 
        trackedRobotPreviousPos = new List<Vector3>();
        foreach (StacsEntity robot in trackedRobots)
        {
            trackedRobotPreviousPos.Add(robot.transform.position);
        }
    }

    /// <summary>
    /// calculates the total distance traved of all climbing robots 
    /// and the percent over the target distance traveled if the robots when over the target 
    /// </summary>
    /// <returns></returns> a float containg the percent grade for this section 
    public float DistanceError()//return grade for this section
    {
        float difference = totalRobotTravelDistance - targetTotalRobotTravelDistance; //amount over target
        if (difference > 0)//find percent greater than target
        {
            float percent = difference / targetTotalRobotTravelDistance;
            return 1.0f - percent;
        }
        else
        {
            return 1.0f;
        }
    }

    /// <summary>
    /// calculates the total battery used of all climbing robots 
    /// and the percent over the target battery usage if the robots when over the target
    /// </summary>
    /// <returns></returns> a float containg the percent grade for this section 
    public float BatteryError()//return grade for this section
    {
        float difference = totalBatteryUsage - targetTotalBatteryUsage; //positive num if acutal batts used greater than target
        if (difference > 0)//find percent greater than target
        {
            float percent = difference / targetTotalBatteryUsage;
            return 1.0f - percent;
        }
        else
        {
            return 1.0f;
        }
    }

    /// <summary>
    /// calulates the grade for defects based on amount of defects found 
    /// </summary>
    /// <returns></returns> grade for this section
    public float DefectsError()
    {
        if (totalDefectsFound > 0)
        {
            return actualDefects / totalDefectsFound;
        }
        return 0;
    }

    /// <summary>
    /// calulates the final grade for the given test based on time taken
    /// </summary>
    /// <returns></returns> the final grade to be stored in the results page 
    public float FinalGrade()
    {
        /* float distance = DistanceError();
         float battery = BatteryError();
         float defects = DefectsError();
         float a = 1;
         float b = 1;
         float c = 1;
         //a, b, c, are weight that we may want to change


         float final = a * distance + b * battery + c * defects;
         final /= 3;
         return final;   */
        float grade = 0;
        if (UIMgr.inst.mTest == TestState.EasyTest1 && UIMgr.inst.mTestType == EGameType.Test) //easy test
        {
            if (totalTime < 35)
            {
                grade = 100;
            }
            else if (totalTime >= 35 && totalTime < 40)
            {
                grade = 90;
            }
            else if (totalTime >= 40 && totalTime < 45)
            {
                grade = 80;
            }
            else if (totalTime >= 45 && totalTime < 50)
            {
                grade = 70;
            }
            else if (totalTime >= 50)
            {
                grade = 0;
            }
        }
        else if (UIMgr.inst.mTest == TestState.MediumTest && UIMgr.inst.mTestType == EGameType.Test) //medium test
        {
            if (totalTime < 35)
            {
                grade = 100;
            }
            else if (totalTime >= 35 && totalTime < 40)
            {
                grade = 90;
            }
            else if (totalTime >= 40 && totalTime < 45)
            {
                grade = 80;
            }
            else if (totalTime >= 45 && totalTime < 50)
            {
                grade = 70;
            }
            else if (totalTime >= 50)
            {
                grade = 0;
            }
        }
        else if (UIMgr.inst.mTest == TestState.HardTest && UIMgr.inst.mTestType == EGameType.Test) //hard  test
        {
            if (totalTime < 35)
            {
                grade = 100;
            }
            else if (totalTime >= 35 && totalTime < 40)
            {
                grade = 90;
            }
            else if (totalTime >= 40 && totalTime < 45)
            {
                grade = 80;
            }
            else if (totalTime >= 45 && totalTime < 50)
            {
                grade = 70;
            }
            else if (totalTime >= 50)
            {
                grade = 0;
            }
        }
        else if (UIMgr.inst.mTest == TestState.EasyTest1 && UIMgr.inst.mTestType == EGameType.Practice) //easy prac test
        {
            if (totalTime < 35)
            {
                grade = 100;
            }
            else if (totalTime >= 35 && totalTime < 40)
            {
                grade = 90;
            }
            else if (totalTime >= 40 && totalTime < 45)
            {
                grade = 80;
            }
            else if (totalTime >= 45 && totalTime < 50)
            {
                grade = 70;
            }
            else if (totalTime >= 50)
            {
                grade = 0;
            }
        }
        else if (UIMgr.inst.mTest == TestState.MediumTest && UIMgr.inst.mTestType == EGameType.Practice) // medium prac test
        {
            if (totalTime < 35)
            {
                grade = 100;
            }
            else if (totalTime >= 35 && totalTime < 40)
            {
                grade = 90;
            }
            else if (totalTime >= 40 && totalTime < 45)
            {
                grade = 80;
            }
            else if (totalTime >= 45 && totalTime < 50)
            {
                grade = 70;
            }
            else if (totalTime >= 50)
            {
                grade = 0;
            }
        }
        return grade;
    }

    public void PrintGrade(float grade)
    {
        Debug.Log("You scored a:" + grade);

    }

    /// <summary>
    /// updates the totalRobotTravelDistance for all tracked robots  
    /// </summary>
    public void UpdateRobotPositionTracking()
    {
        for (int i = 0; i < trackedRobots.Count; i++)
        {
            totalRobotTravelDistance += (trackedRobotPreviousPos[i] - trackedRobots[i].transform.position).magnitude;
            trackedRobotPreviousPos[i] = trackedRobots[i].transform.position;
        }
    }

    /// <summary>
    /// saves the results to a file 
    /// pauses the game and shows the results to user,
    ///    
    /// /// </summary>
    public void EndTest()
    {
        testEnded = true;
        WriteToFile();
        UIMgr.inst.PauseGame();
        Debug.Log("next");
        UIMgr.inst.resultsPagePanel.SetActive(true);
        Debug.Log("wows");
        UIMgr.inst.GradeText.text = FinalGrade().ToString("0");
        UIMgr.inst.ResumeGame();
    }

    /// <summary>
    /// display the fail test ui and write a failed test to results 
    /// </summary>
    public void FailTest()
    {
        Debug.Log("Fail Test");
        //gnna add a fail ui stuff
        testEnded = true;
        WriteToFileFail();
        UIMgr.inst.PauseGame();
        UIMgr.inst.resultsPagePanelFailed.SetActive(true);
        UIMgr.inst.ResumeGame();
    }


 

    /// <summary>
    /// sets proper strings bases on the typeof test and then calls functions from FileIO class
    /// to write results of test to file
    /// </summary>
    public void WriteToFile()
    {
        string output = "";

        output += "Steel Truss Bridge" + "\t"; // TODO get bridge name
        if(UIMgr.inst.mTest == TestState.EasyTest1)
        {
            output += "Easy" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.MediumTest)
        {
            output += "Medium" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.HardTest)
        {
            output += "Hard" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Test)
        {
            output += "Test" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Practice)
        {
            output += "Practice Test" + "\t";
        }
        //output += testMode.ToString() + "\t";
        output += FinalGrade().ToString() + "\t";
        output += totalRobotTravelDistance.ToString() + "\t";
        output += totalTime.ToString() + "\t";
        output += totalBatteryUsage.ToString() + "\t";
        output += totalDefectsFound.ToString();

        FileIO.instance.WriteToFile(resultsPath, output, false, false);
    }

    /// <summary>
    /// sets proper strings bases on the typeof test and then calls functions from FileIO class
    /// to write results of failure to file
    /// </summary>
    public void WriteToFileFail()
    {
        string output = "";

        output += "Steel Truss Bridge" + "\t"; // TODO get bridge name
        if (UIMgr.inst.mTest == TestState.EasyTest1)
        {
            output += "Easy" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.MediumTest)
        {
            output += "Medium" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.HardTest)
        {
            output += "Hard" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Test)
        {
            output += "Test" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Practice)
        {
            output += "Practice Test" + "\t";
        }
        //output += testMode.ToString() + "\t";
        output += "0" + "\t";
        output += totalRobotTravelDistance.ToString() + "\t";
        output += totalTime.ToString() + "\t";
        output += totalBatteryUsage.ToString() + "\t";
        output += totalDefectsFound.ToString();

        FileIO.instance.WriteToFile(resultsPath, output, false, false);
    }
   
    //for easy test select robot then press o 
    //robot will start rount then after 15 second all commands will be cleared
    //test is complete when robot reached the way point

    /// <summary>
    /// corountine error for easy nad hard test 
    /// waits 15 seconds then clears the robots of all commands
    /// </summary>
    /// <returns></returns>
    IEnumerator Error()
    {
        if (UIMgr.inst.mTest == TestState.EasyTest1)
        {
            Debug.Log("Error easy");
            yield return new WaitForSeconds(15);
            AIMgr.inst.HandleClear(trackedRobots);

           
        }
        else if(UIMgr.inst.mTest == TestState.HardTest)
        {
            Debug.Log("Error for hard started");
            yield return new WaitForSeconds(15);
            AIMgr.inst.HandleClear(trackedRobots[0]);
        }
        errorOccured = true;
    }

    /// <summary>
    /// script for east test sets robot on path towrds waypoint then calles coroutine error 
    /// </summary>
      public void easyTest1()
    {
        Vector3 testEasy = new Vector3(-19.2f, 18.3f, -16.0f);
        AIMgr.inst.HandleMove(trackedRobots, testEasy);

        StartCoroutine(Error());    //wait 15 seconds then do it
    }

    /// <summary>
    /// sets robots to full speed and error flag to true 
    /// </summary>
    public void mediumTest()
    {
        //Debug.Log("MediumTest");
        foreach (StacsEntity robot in trackedRobots)
        {
            robot.desiredSpeed = 4.0f; 
        }
        errorOccured = true;

    }
    /// <summary>
    /// sets both robots to max speed then calls coroutine error 
    /// </summary>
    public void hardTest()
    {
        //Debug.Log("Hard Test");
        Vector3 testHard = new Vector3(0f, 31.11f, -15.25f);
        AIMgr.inst.HandleMove(trackedRobots[0], testHard);
        trackedRobots[1].desiredSpeed = 4.0f;
        StartCoroutine(Error());
        


    }

    /// <summary>
    /// based on the tpe of test check to see if object is complete by checking distance of robot to waypoint 
    /// </summary>
    /// <returns></returns> truen when robot is withing 4 units of ways point
    bool checkEnd()
    {
        int counter = 0;
        float distance;
        foreach (StacsEntity robot in trackedRobots)
        {

            if(UIMgr.inst.mTest == TestState.EasyTest1 || UIMgr.inst.mTest == TestState.MediumTest)
            {
                Vector3 targetPos = new Vector3(-19.3f, 18.3f, -15.25f); //position of pink way point 
                distance = Vector3.Distance(robot.transform.position, targetPos);
                if(distance < 4.0f)
                {
                    EndTest();
                }
            }
            else if (UIMgr.inst.mTest == TestState.HardTest)
            {
                Vector3 targetPos = new Vector3(0.0f, 31.11f, -15.25f); //position of pink way point 
                distance = Vector3.Distance(robot.transform.position, targetPos);
                Debug.Log(distance);
                if (distance < 4.0f)
                {
                    counter++;
                }
            }

        }
        if(counter == 2)
        {
           EndTest();
        }

        return true;
    }

    /// <summary>
    /// pauses game and shows hint pannel
    /// </summary>
    public void hint()
    {
        Debug.Log("hint");
        if (trackedRobots[0].speed == 0.0f && UIMgr.inst.mTest == TestState.EasyTest1)
        {

            UIMgr.inst.PauseGame();
            UIMgr.inst.hintPanel.SetActive(true);
            errorOccured = false;

        }
        if(UIMgr.inst.mTest == TestState.MediumTest && totalRobotTravelDistance > 0.7f)
        {
            UIMgr.inst.PauseGame();
            UIMgr.inst.hintPanel.SetActive(true);
            errorOccured = false;
        }

        
    }

    /// <summary>
    /// calls UpdateRobotPositionTracking() updates totaltime and check to see if the test has ended 
    /// </summary>
    private void Update()
    {
        UpdateRobotPositionTracking();
        totalTime += Time.deltaTime;

        if(testEnded == false )
        {
            checkEnd();
        }
        if(errorOccured && UIMgr.inst.mTestType == EGameType.Practice )
        {
            Debug.Log("wow");
            hint();
        }
        

    }

}

