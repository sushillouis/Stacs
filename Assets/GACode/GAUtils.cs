using System.Collections;
using System.Collections.Generic;


public class GAUtils 
{

    public static System.Random Random;
    public static void Randomize(int seed)
    {
        Random = new System.Random(seed);
    }

    public static bool Flip(float prob)
    {
        return (Random.NextDouble() < prob);
    }

    public static int RandInt(int low, int high)
    {//greater than equal to low and strictly less than high
        return Random.Next(low, high);
    }

    public static void AppendToFile(string filename, string report)
    {
        System.IO.File.AppendAllText(filename, report);        
    }

    public static int AddOneModulo(int x, int modulus)
    { // base 0
        return (x + 1 >= modulus ? 0 : x+1);
    }
    

}
