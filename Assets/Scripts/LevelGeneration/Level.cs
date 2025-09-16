using System.Collections.Generic;
using UnityEngine;
using static Match3Enums;

public class Level
{
    #region DataFields
    public int rowCount;
    public int columnCount;
    public int obstacleCount;
    public TileType[] tileTypesAllowed;
    public BackgroundType[] backgroundTypesAllowed;
    public ObstacleType[] obstacleTypesAllowed;
    public Dictionary<TilePower, float> tilePowerPercentages = new();
    #endregion

    #region RuntimeFields
    public GameObject[] bgAndObstacles;// Background and Obstacles
    public GameObject[] tiles;
    #endregion

    public Level(int rows, int columns, int obstacleCount, TileType[] tileTypesAllowed, BackgroundType[] backgroundTypesAllowed, ObstacleType[] obstacleTypesAllowed)
    {
        this.rowCount = rows;
        this.columnCount = columns;
        this.obstacleCount = obstacleCount;
        this.tileTypesAllowed = tileTypesAllowed;
    }
    public Level(LevelBuildData data)
    {
        this.rowCount = data.rowCount;
        this.columnCount = data.columnCount;
        this.tileTypesAllowed = data.tileTypesAllowed;
        this.backgroundTypesAllowed = data.backgroundTypesAllowed;
        this.obstacleTypesAllowed = data.obstacleTypesAllowed;
        this.tilePowerPercentages = data.tilePowerPercentages;

        if (data.obstacleCountIsRandom)//check if obstacle count is random
        {
            int totalCells = rowCount * columnCount;
            this.obstacleCount = Mathf.RoundToInt(totalCells * data.obstaclePercent / 100f);
        }
        else
        {
            this.obstacleCount = data.obstacleCount;
        }
    }
}
