using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFactory
{
    private LevelManager levelManager;
    private bool[] isEmpty;
    public LevelFactory(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public Level CreateBGO(Level level)
    {
        return CreateLevelObstacles(CreateLevelBackgrounds(level));
    }

    public Level CreateLevelBackgrounds(Level level)//use scriptable object for bg
    {
        GridLayoutGroup backgroundGrid = LevelManager.Instance.backgroundGrid;
        
        backgroundGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;//implement data columns to grid layout component
        backgroundGrid.constraintCount = level.columnCount;//implement data columns to grid layout component

        int totalCells = level.rowCount * level.columnCount;
        isEmpty = new bool[totalCells];
        level.bgAndObstacles = new GameObject[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            GameObject bgCell = ObjectPoolManager.SpawnObject(
                levelManager.backgroundPrefab,
                Vector3.zero,
                Quaternion.identity,
                ObjectPoolManager.PoolType.Background
            );
            bgCell.transform.SetParent(backgroundGrid.transform, false);
            isEmpty[i] = true;
            Sprite bgSprite = Util.GetRepeatingElement(level.backgrounds, i);
            bgCell.GetComponent<Image>().sprite = bgSprite;
            level.bgAndObstacles[i] = bgCell;
        }
        return level;
    }

    public Level CreateLevelObstacles(Level level)//use scriptable object for obstacles
    {
        int totalCells = level.rowCount * level.columnCount;
        int obstacleCount = level.obstacleCount;
        List<int> obstacleIndexes = new List<int>();
        for (int i = 0; i < obstacleCount; i++)
        {
            int obstacleIndex = Random.Range(0, totalCells);
            if (obstacleIndexes.Contains(obstacleIndex)) continue;
            obstacleIndexes.Add(obstacleIndex);
        }
        for (int i = 0; i < totalCells; i++)
        {
            for (int j = 0; j < obstacleIndexes.Count; j++)
            {
                if (i == obstacleIndexes[j])
                {
                    int siblingIndex = level.bgAndObstacles[i].transform.GetSiblingIndex();
                    ObjectPoolManager.ReturnObjectToPool(level.bgAndObstacles[i]);

                    GameObject obstacle = ObjectPoolManager.SpawnObject(
                        levelManager.obstaclePrefab,
                        Vector3.zero,
                        Quaternion.identity,
                        ObjectPoolManager.PoolType.Obstacle
                    );
                    obstacle.transform.SetParent(LevelManager.Instance.backgroundGrid.transform, false);
                    obstacle.transform.SetSiblingIndex(siblingIndex);
                    Sprite obstacleSprite = Util.GetRandomInArray(level.obstacles);
                    obstacle.GetComponent<Image>().sprite = obstacleSprite;

                    level.bgAndObstacles[i] = obstacle;
                    isEmpty[i] = false;

                    break;
                }
            }
        }
        return level;
    }
    public Level CreateLevelTiles(Level level)//TDO: implement tile creation
    {
        return null;
    }
}
