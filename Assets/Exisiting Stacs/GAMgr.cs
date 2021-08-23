using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;


public class GAMgr : MonoBehaviour
{
    public int numRuns = 1;
    public int numRobots = 1;

    // Start is called before the first frame update
    [ContextMenu("Run GA")]
    public void RunGA()
    {
        string dir = Application.dataPath + "/MMkCPP/ga.exe";
        string graph = Application.dataPath + "/Routing/graph.csv";
        print(dir);
        print("runs: " + numRuns + " robots: " + numRobots);
        ExecuteProcess(dir, graph + ' ' + numRuns + ' ' + numRobots);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string ExecuteProcess(string command, string arguments)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();

            // Write the redirected output to this application's window.
            print(output);
            GetComponent<SceneMgr>().routeOutput = output;
            Console.WriteLine(output);

            process.WaitForExit();
        }

        Console.WriteLine("\n\nPress any key to exit.");
        Console.ReadLine();

        /*
        int timeout = 10000000; 
        Process process = new Process();
        //process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        StringBuilder output = new StringBuilder();
        StringBuilder error = new StringBuilder();

        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
        {
            process.OutputDataReceived += (sender, e) => {
                if (e.Data == null)
                {
                    outputWaitHandle.Set();
                }
                else
                {
                    output.AppendLine(e.Data);
                    GetComponent<SceneMgr>().climbingRoutePath = e.Data;
                    print(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    errorWaitHandle.Set();
                }
                else
                {
                    error.AppendLine(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (process.WaitForExit(timeout) &&
                outputWaitHandle.WaitOne(timeout) &&
                errorWaitHandle.WaitOne(timeout))
            {
                // Process completed. Check process.ExitCode here.
            }
            else
            {
                // Timed out.
            }
        }
        print("DONE!");
        */
        return "";
    }
}

/*
 *         try
        {
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
            process.StartInfo.Arguments = "/c" + command;

            process.EnableRaisingEvents = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();

            string standard_output;
            while ((standard_output = process.StandardOutput.ReadLine()) != null)
            {
                print(standard_output);
                if (standard_output.Contains("xx"))
                {
                    //do something
                    break;
                }
            }
            process.WaitForExit();

            process.Close();

            //print(ExitCode);
        }
        catch (Exception e)
        {
            print(e);
        }
        print("DONE!");
*/