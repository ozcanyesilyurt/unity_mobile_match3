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
    public TileType[] tileTypesAllowed = new TileType[]
    {
        TileType.Red,
        TileType.Blue,
        TileType.Green,
        TileType.Yellow,
        TileType.Purple
    };
    public BackgroundType[] backgroundTypesAllowed = new BackgroundType[]
    {
        BackgroundType.White,
        BackgroundType.Black,
        BackgroundType.None
    };
    public ObstacleType[] obstacleTypesAllowed = new ObstacleType[]
    {
        ObstacleType.Rock,
        ObstacleType.Wood,
        ObstacleType.Ice,
        ObstacleType.Metal
    };

    [Header("Tile Power Percentage")]
    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "Power Type", ValueLabel = "Percentage Chance")]
    public Dictionary<TilePower, float> tilePowerPercentages = new()
{
    { TilePower.HorizontalClear, 0f },
    { TilePower.VerticalClear, 0f },
    { TilePower.Bomb, 0f },
    { TilePower.ColorClear, 0f }
};

    [OnInspectorGUI]
    private void ValidateTilePowerPercentages()
    {
        foreach (var key in new List<TilePower>(tilePowerPercentages.Keys))
        {
            tilePowerPercentages[key] = Mathf.Clamp(tilePowerPercentages[key], 0f, 100f);
        }
    }

}
