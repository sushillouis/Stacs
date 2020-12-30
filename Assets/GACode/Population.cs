using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Population 
{
    Options options;
    Individual[] individuals = new Individual[Constants.MAX_POPULATION];
    public float sumFitness;
    public float sumObjective;
    public float maxFitness;
    public float bestObjective;
    public float avgFitness;
    public float avgObjective;
    public float minFitness;
    public int maxi;
    public int mini;

    public Individual bestEver;
    public float bestFitnessEver;

    IComparer MyFitnessComparer;

    public Population(Options opts)
    {
        options = opts;
        MyFitnessComparer = new FitnessComparer();
        bestEver = new Individual(options);
    }

    public void Init()
    {
        for(int i = 0; i < options.populationSize * options.chcLambda; i++)
        {
            individuals[i] = new Individual(options);
        }
        //PrintPop(0, options.populationSize);
        EvaluatePopulation(0, options.populationSize);
        //Debug.Log("Evaluated Population");
        //PrintPop(0, options.populationSize);
    }

    public void EvaluatePopulation(int start, int end)
    {
        for(int i = start; i < end; i++)
        {
            //individuals[i].fitness = Evaluator.Evaluate(individuals[i]);
            individuals[i].objectiveFunction = individuals[i].Evaluate();
            individuals[i].fitness = 500 - individuals[i].objectiveFunction;
        }
    }

    public void Statistics()
    {
        sumFitness = individuals[0].fitness;
        sumObjective = individuals[0].objectiveFunction;
        minFitness = individuals[0].fitness;
        maxFitness = individuals[0].fitness;
        bestObjective = individuals[0].objectiveFunction;
        mini = 0;
        maxi = 0;
        for(int i = 1; i < options.populationSize; i++)
        {
            sumFitness += individuals[i].fitness;
            sumObjective += individuals[i].objectiveFunction;
            if(minFitness > individuals[i].fitness) {
                minFitness = individuals[i].fitness;
                mini = i;//don't care about worst objective, generally speaking. BUt if you do you can add its computation here
            }
            if(maxFitness < individuals[i].fitness) {
                maxFitness = individuals[i].fitness;
                bestObjective = individuals[i].objectiveFunction;
                maxi = i;
            }
        }
        avgFitness = sumFitness / options.populationSize;
        avgObjective = sumObjective / options.populationSize;

        if(individuals[maxi].fitness > bestFitnessEver) {
            bestEver.Copy(individuals[maxi]);
        }
    }

    public void Report(int gen)
    {
        string rString = "";
        rString = System.String.Format("{0:00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00} \n", 
            gen, maxFitness, avgFitness, minFitness, 500 - maxFitness, 500 - avgFitness);

        Debug.Log(rString);
        GAUtils.AppendToFile(options.outFilename, rString);
    }

    public void PrintPop(int start, int end)
    {
        for(int i = start; i < end; i++)
        {
            Debug.Log(individuals[i].StringTo());
        }
    }

    public void CHCGeneration(Population child)
    {
        int pi1, pi2, ci1, ci2;
        Individual p1, p2, c1, c2;
        for(int i = 0; i < options.populationSize; i += 2)
        {
            pi1 = ProportionalSelector();//GAUtils.RandInt(0, options.populationSize);
            pi2 = ProportionalSelector();//GAUtils.RandInt(0, options.populationSize);
            ci1 = options.populationSize + i;
            ci2 = options.populationSize + i + 1;

            p1 = individuals[pi1]; p2 = individuals[pi2];
            c1 = individuals[ci1]; c2 = individuals[ci2];

            //XoverAndMutate(p1, p2, c1, c2);
            SequenceXoverAndMutate(p1, p2, c1, c2);
        }
        Halve(child);
    }

    public int ProportionalSelector()
    {
        int i = -1;
        double sum = 0;
        double limit = GAUtils.Random.NextDouble() * sumFitness;
        do
        {
            i = i + 1;
            sum += individuals[i].fitness;
        } while(sum < limit && i < options.populationSize - 1);
        return i;
    }

    public void XoverAndMutate(Individual p1, Individual p2, Individual c1, Individual c2)
    {
        for(int i = 0; i < options.chromosomeLength; i++) { // copy chromosomes to child
            c1.chromosome[i] = p1.chromosome[i];
            c2.chromosome[i] = p2.chromosome[i];
        }

        if(GAUtils.Flip(options.pCross))
        { // cut and splice from parents
            int xp1 = GAUtils.RandInt(0, options.chromosomeLength);
            int xp2 = GAUtils.RandInt(0, options.chromosomeLength);
            int x1 = System.Math.Min(xp1, xp2);
            int x2 = System.Math.Max(xp1, xp2);
            for(int i = x1; i < x2; i++)
            {
                c2.chromosome[i] = p1.chromosome[i];
                c1.chromosome[i] = p2.chromosome[i];
            }
        }
        c1.Mutate(options.pMut);
        c2.Mutate(options.pMut);
    }

    public void Halve(Population child)
    {
        EvaluatePopulation(options.populationSize, options.populationSize * options.chcLambda);
        //PrintPop(0, options.populationSize * options.chcLambda);
        System.Array.Sort(individuals, //MyFitnessComparer defined in Individual
            0, options.populationSize * options.chcLambda, MyFitnessComparer);
        for(int i = 0; i < options.populationSize; i++){
            child.individuals[i] = individuals[i];
        }
    }

    public void SequenceXoverAndMutate(Individual p1, Individual p2, Individual c1, Individual c2)
    {
        for(int i = 0; i < options.chromosomeLength; i++) { // copy chromosomes to child
            c1.chromosome[i] = p1.chromosome[i];
            c2.chromosome[i] = p2.chromosome[i];
        }
        c1.SwapMutate(options.pMut);
        c2.SwapMutate(options.pMut);
        //c1.ReverseMutate(options.pMut);
        //c2.ReverseMutate(options.pMut);
    }
}


