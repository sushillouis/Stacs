using System.Collections;
using System.Collections.Generic;

public class Options
{
    public int populationSize;
    public int maxGen;
    public int chromosomeLength;
    public float pCross;
    public float pMut;
    public int chcLambda;

    public int seed;
    public string outFilename;

    public string graphFilename;
    public int nRobots;
    public int nEdges;
}

static class Constants
{
    public const int MAX_CHROMOSOME_LENGTH = 1000;
    public const int MAX_POPULATION = 2000;
    public const int MAX_VERTICES = 1000;
}

public class GA 
{
    public static GA inst;
    public Options options;

    public Population parent;
    public Population child;

    public GA(Options opts)
    {
        inst = this;
        options = opts;
        GAUtils.Randomize(options.seed);
        Evaluator.Init(options.graphFilename);
    }

    public void Init()
    {
        parent = new Population(options);
        parent.Init();
        parent.Statistics();
        parent.Report(0);

        child = new Population(options);
        child.Init();
    }

    public void Run()
    {
        for(int i = 1; i < options.maxGen; i++)
        {
            parent.CHCGeneration(child);
            child.Statistics();
            child.Report(i);

            Population tmp = parent;
            parent = child;
            child = tmp;
        }

    }


}
