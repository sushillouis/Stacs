using System.Collections;
using System.Collections.Generic;

public class FitnessComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (int) (((Individual)y).fitness - ((Individual)x).fitness);
    }
}

public class Individual
{
    public Options options;
    public int[] chromosome = new int[Constants.MAX_CHROMOSOME_LENGTH];
    int chromosomeLength = 0;
    int nRobots = 0;
    int nEdges = 0;
    public float fitness;
    public float objectiveFunction;

    public Individual(Options opts)
    {
        options = opts;
        chromosomeLength = opts.nEdges + opts.nRobots;
        nRobots = opts.nRobots;
        nEdges = opts.nEdges;
        InitBinaryChromosome();
        InitIntegerChromosome();
    }
    
    public void InitBinaryChromosome()
    {
        for(int i = 0; i < chromosomeLength; i++) {
            chromosome[i] = (GAUtils.Flip(0.5f) ? 1 : 0);
        }
    }

    public void InitIntegerChromosome()
    {
        for(int i = 0; i < chromosomeLength; i++) {
            chromosome[i] = i;
        }
        for(int i = 0; i < chromosomeLength/2; i++) {
            swap(GAUtils.RandInt(0, chromosomeLength), GAUtils.RandInt(0, chromosomeLength));
        }
    }

    public int RobotId(int n)
    {
        //return chromosomeLength % nEdges;
        return 0;
    }


    public void swap(int i, int j)
    {
        int tmp = chromosome[i];
        chromosome[i] = chromosome[j];
        chromosome[j] = tmp;
    }

    public void SwapMutate(float prob)
    {//swap two locations
        if(GAUtils.Flip(prob)) {
            swap(GAUtils.RandInt(0, chromosomeLength), GAUtils.RandInt(0, chromosomeLength));
        }
    }

    public void ReverseMutate(float prob)
    {//reverse a slice of the string
        if(GAUtils.Flip(prob)) {
            int x1 = GAUtils.RandInt(0, chromosomeLength);
            int x2 = GAUtils.RandInt(0, chromosomeLength);
            int start = System.Math.Min(x1, x2);
            int end = System.Math.Max(x1, x2);

            int j = end;
            for(int i = start; i < j; i++, j--) {
                swap(i, j);
            }
        }
    }

    public void Mutate(float prob)
    {
        for(int i = 0; i < options.chromosomeLength; i++)
        {
            if(GAUtils.Flip(prob))
            {
                chromosome[i] = 1 - chromosome[i];
            }
        }
    }

    public string StringTo()
    {
        string tmp = "";
        for(int i = 0; i < options.chromosomeLength; i++)
        {
            tmp += chromosome[i].ToString("0");
        }
        tmp += "\nFit: " + fitness.ToString("0.0") + "\n";
        return tmp;
    }
}
