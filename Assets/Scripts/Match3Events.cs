using System;
using UnityEngine;

public class Match3Events : MonoBehaviour
{
    public static Action OnDetectMatches;
    public static Action OnTilesRemoved;
    public static Action OnTilesFall;
    public static Action OnTrySwap;
    public static Action OnSwapCancel;
    public static Action OnSwapSuccess;

}