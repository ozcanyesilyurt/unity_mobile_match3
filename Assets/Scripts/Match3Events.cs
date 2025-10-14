using System;
using UnityEngine;

public class Match3Events : MonoBehaviour
{
    public static Action OnDetectMatches;
    public static Action OnTilesRemoved;
    public static Action OnTileTouchesBottom;
    public static Action OnTrySwap;
    public static Action OnSwapCancel;
    public static Action OnSwapSuccess;

    public static Action<int> OnScoreAdded;
    public static Action OnScoreReset;

    public static Action OnBackgroundMove;
    public static Action OnNewBackgroundImage;
    
    public static Action OnCharacterMoveRight;


}