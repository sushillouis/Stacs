using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// FileIO class establishes methods for reading from and writing to files
/// </summary>
public class FileIO : MonoBehaviour
{
    public static FileIO instance; //FileIO instance accessible by other scripts

    /// <summary>
    /// called upon application start up
    /// </summary>
    private void Awake()
    {
        // creates singleton FileIO instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //WriteToFile("Bridges/test.txt", "Hello world");   
    }

    /// <summary>
    /// uses the file explorer to locate a position to save the file, as well as a filename, and then writes contents to the path
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="output"></param>
    /// <param name="overwrite"></param>
    /// <param name="isBridgeFile"></param>
    public void WriteToFile(string fileName, string output, bool overwrite, bool isBridgeFile)
    {
        print(fileName);
        if (isBridgeFile == true)
        {
            var path = EditorUtility.SaveFilePanel(
            "Save Bridge as txt file",
            "DefaultBridge",
            fileName,
            "txt");
            if (overwrite)
            {
                StreamWriter sr = File.CreateText(path);
                sr.WriteLine(output);
                sr.Close();
            }
            else
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(output);
                tw.Close();
            }

        }
        else 
        {
            if (overwrite)
            {
                StreamWriter sr = File.CreateText(fileName);
                sr.WriteLine(output);
                sr.Close();
            } else
            {
                TextWriter tw = new StreamWriter(fileName, true);
                tw.WriteLine(output);
                tw.Close();
            }
        }
    }

    /// <summary>
    /// given a filename, opens a streamreader for the file and processes the contents into a string
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns> a string containing the contents of the target file </returns>
    public string ReadFromFile(string fileName)
    {
        string fileInfo = "";
        StreamReader reader = new StreamReader(fileName);
        fileInfo = reader.ReadToEnd();
        reader.Close();
        return fileInfo;
    }

    /// <summary>
    /// given a bridge file following the correct formatting, returns the vertex and connection information needed to construct the bridge
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns> an array of strings, each string containing different information about the bridge </returns>
    public string[] ReadBridgeFile(string fileName)
    {
        string[] tempInfo;
        string vertexInfo = "";
        string connectionInfo = null;
        string[] fileInfo = new string[2];
        StreamReader reader = new StreamReader(fileName);
        //fileInfo = reader.ReadToEnd();
        while(!reader.EndOfStream)
        {
            tempInfo = reader.ReadLine().Split(' ');
            if (tempInfo.Length == 5)
            {
                vertexInfo += tempInfo[1] + " ";
                vertexInfo += tempInfo[2] + " ";
                vertexInfo += tempInfo[3] + " ";
            }
            if (tempInfo.Length == 3)
            {
                connectionInfo += tempInfo[0] + " ";
                connectionInfo += tempInfo[1] + " ";
            }
        }
        print("vertex info:" + vertexInfo + '\n');
        print("Connection info:" + connectionInfo + '\n');
        reader.Close();
        fileInfo[0] = vertexInfo;
        fileInfo[1] = connectionInfo;

        return fileInfo;
    }
}
