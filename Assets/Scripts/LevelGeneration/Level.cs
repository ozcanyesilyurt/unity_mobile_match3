using System.Collections.Generic;
using UnityEngine;
using static Match3Enums;

public class Level
{
    #region DataFields
    public int rowCount;
    public int columnCount;
    public int obstacleCount;
    public Sprite[] backgrounds;
    public Sprite[] obstacles;
    public Sprite[] tileTypes;
    public Dictionary<TileType, bool> tileTypesAllowed;
    #endregion

    #region RuntimeFields
    public GameObject[] bgAndObstacles;// Background and Obstacles
    public GameObject[] tiles;
    #endregion

    public Level(int rows, int columns, int obstacleCount, Sprite[] backgrounds, Sprite[] obstacles, Sprite[] tileTypes, Dictionary<TileType, bool> tileTypesAllowed)
    {
        this.rowCount = rows;
        this.columnCount = columns;
        this.obstacleCount = obstacleCount;
        this.backgrounds = backgrounds;
        this.obstacles = obstacles;
        this.tileTypes = tileTypes;
        this.tileTypesAllowed = tileTypesAllowed;
    }
    public Level(LevelBuildData data)
    {
        this.rowCount = data.rowCount;
        this.columnCount = data.columnCount;
        this.backgrounds = data.backgrounds;
        this.obstacles = data.obstacles;
        this.tileTypes = data.tileTypes;
        this.tileTypesAllowed = data.tileTypesAllowed;

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
