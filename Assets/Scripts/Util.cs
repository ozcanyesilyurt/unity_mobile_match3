using UnityEngine;

public static class Util
{
    public static T GetRandomInArray<T>(T[] array)
    {
        int index = Random.Range(0, array.Length);
        return array[index];
    }
    public static T GetRepeatingElement<T>(T[] array, int index)
    {
        return array[index % array.Length];
    }

    
}
