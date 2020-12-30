using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading;

[System.Serializable]
public class GAReport
{
    public int gen;
    public float max;
    public float avg;
    public float min;
    public float bestObjective;
    public float avgObjective;
    public Individual bestEver;

    public GAReport(int g, float mx, float av, float mn, float bestO, float avgO, Individual b)
    {
        gen = g;
        max = mx;
        avg = av;
        min = mn;
        bestObjective = bestO;
        avgObjective = avgO;
        bestEver = new Individual(b.options);
        bestEver.Copy(b);
    }

}

public class GATest : MonoBehaviour
{

    public static GATest inst;
    private void Awake()
    {
        inst = this;
    }

    public StacsPanel GAPanel;
    public Button GAStart;
    public Button GAStop;
    public InputField PopulationSize;
    public InputField ChromosomeLength;
    public InputField Maxgen;
    public InputField PCross;
    public InputField PMut;
    public InputField RandomSeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Thread gaThread;
    public string ApplicationDataPath;

    public List<GAReport> reports;

    public void OnStartGA()
    {
        //Create threads, start GA
        Debug.Log("On start GA");
        ApplicationDataPath = Application.persistentDataPath;
        reports = new List<GAReport>();
        gaThread = new Thread(RunGA);
        gaThread.Start();
    }

    public void RunGA()
    {
        Debug.Log("Running GA");

        Options options = new Options();
        SetOptions(options);

        GA ga = new GA(options);
        ga.Init();
        ga.Run();

        Debug.Log("Exiting GA");
    }

    public void SetOptions(Options options)
    {
        options.populationSize = 10; // int.Parse(PopulationSize.text);
        options.maxGen = 15;// int.Parse(Maxgen.text);
        options.pCross = 0.95f;// float.Parse(PCross.text);
        options.pMut = 1;// 0.05f;// float.Parse(PMut.text);
        options.chromosomeLength = 0;// int.Parse(ChromosomeLength.text);
        options.seed = 12345; // int.Parse(RandomSeed.text);
        options.chcLambda = 2;
        options.outFilename = ApplicationDataPath + "/output.txt";
        Debug.Log("Output: " + options.outFilename);

        options.graphFilename = "graph-raw.csv";
    }

}
