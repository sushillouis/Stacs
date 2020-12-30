using System.Collections;
using System.Collections.Generic;

public class Evaluator
{
    public static float Evaluate(Individual individual)
    {
        float sum = 0;
        for(int i = 0; i < GA.inst.options.chromosomeLength; i++) {
            sum += individual.chromosome[i];
        }
        return sum;
    }

    public static Graph Init(string pathname)
    {
        return new Graph(pathname);
    }

}
