using System.Collections.Generic;
using UnityEngine;
using static Match3Enums;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "LevelBuildData", menuName = "LevelBuild/LevelBuildData", order = 1)]
public class LevelBuildData : ScriptableObject //for storing level data in the inspector
{
    [Header("Configuration")]
    public int rowCount;
    public int columnCount;
    public int obstacleCount;

    [Header("Obstacle Randomization")]
    public bool obstacleCountIsRandom;
    [Range(0f, 100f)]
    public float obstaclePercent;

    [Header("Tile Types")]
    [ShowInInspector]
    public Dictionary<TileType, bool> tileTypesAllowed = new()
    {
        { TileType.Red, true },
        { TileType.Blue, true },
        { TileType.Green, true },
        { TileType.Yellow, true },
        { TileType.Purple, true },
    };

    [Header("Tile Power Percentage")]
    [Range(0f, 100f)]
    public float horizontalClearPercent;
    [Range(0f, 100f)]
    public float verticalClearPercent;
    [Range(0f, 100f)]
    public float bombPercent;
    [Range(0f, 100f)]
    public float colorClearPercent;

    [Header("Sprites")]
    public Sprite[] backgrounds;
    public Sprite[] obstacles;
    public Sprite[] tileTypes;
}
