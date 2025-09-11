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
        int obstacleCount = Mathf.Min(level.obstacleCount, totalCells);
        var obstacleSet = new HashSet<int>();
        while (obstacleSet.Count < obstacleCount)
        {
            obstacleSet.Add(Random.Range(0, totalCells));
        }

        for (int i = 0; i < totalCells; i++)
        {
            if (!obstacleSet.Contains(i)) continue;

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
        }
        return level;
    }
    public Level CreateLevelTiles(Level level)//TDO: implement tile creation
    {
        return null;
    }
}
