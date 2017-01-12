using System.Collections;
using System;
using System.Linq;

public static class Utility {
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random pseudoRandom = new System.Random(seed);
        Enumerable
            .Range(0, array.Length - 1)
            .ToList()
            .ForEach(
                (index) =>
                {
                    int randomIndex = pseudoRandom.Next(index, array.Length);
                    T tempItem = array[randomIndex];
                    array[randomIndex] = array[index];
                    array[index] = tempItem;
                }
            );
        return array;
    }
}
