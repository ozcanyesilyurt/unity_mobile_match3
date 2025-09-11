using UnityEngine;

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

    [Header("Sprites")]
    public Sprite[] backgrounds;
    public Sprite[] obstacles;
    public Sprite[] tileTypes;
}
