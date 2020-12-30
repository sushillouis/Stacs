using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
    public Graph graph;
    public int nRobots;
}

static class Constants
{
    public const int MAX_CHROMOSOME_LENGTH = 10;
    public const int MAX_POPULATION = 2000;
    public const int MAX_VERTICES = 10;
}

public class GA 
{
    public static GA inst;
    public Options options;

    public Population parent;
    public Population child;

    private static SemaphoreSlim semaphore;

    public GA(Options opts)
    {
        inst = this;
        options = opts;
        GAUtils.Randomize(options.seed);
        options.graph = Evaluator.Init(options.graphFilename);
        options.nRobots = 1;
        options.chromosomeLength = options.nRobots + options.graph.nEdges;
        semaphore = new SemaphoreSlim(1, 1);
    }

    public void Init()
    {
        parent = new Population(options);
        parent.Init();
        parent.Statistics();
        parent.Report(0);
        FillReport(0, parent);

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
            FillReport(i, child);

            Population tmp = parent;
            parent = child;
            child = tmp;
        }
        parent.EvaluatePopulation(0, options.populationSize);
        semaphore.Dispose();
    }

    public void FillReport(int gen, Population pop)
    {
        GAReport gar = 
            new GAReport(gen, pop.maxFitness, pop.avgFitness, pop.minFitness, 
            pop.bestObjective, pop.avgObjective, pop.bestEver);

        semaphore.WaitAsync(0);
        GATest.inst.reports.Add(gar);
        semaphore.Release();

    }
}
