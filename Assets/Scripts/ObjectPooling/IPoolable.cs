using UnityEngine;

public interface IPoolable
{
    void ResetForPool();
    int row { get; set; }
    int column { get; set; }
}
