using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static Match3Enums;

public class LevelFactory
{
    private LevelManager levelManager;
    private bool[] isEmpty;

    public LevelFactory(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public Level CreateLevel(LevelBuildData levelData)
    {
        Level level = new Level(levelData);
        CreateLevelBackgrounds(level);
        CreateLevelObstacles(level);
        CreateLevelTiles(level);
        return level;
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
    public Tile CreateTile(Level level)//TDO: implement tile creation
    {
        GameObject tileGO = ObjectPoolManager.SpawnObject(
                levelManager.tilePrefab,
                Vector3.zero,
                Quaternion.identity,
                ObjectPoolManager.PoolType.Tile
            );
        tileGO.transform.SetParent(LevelManager.Instance.tilesContainer.transform, false);
        Tile tile = tileGO.GetComponent<Tile>();
        tile.ResetForPool();
        return tile;
    }

    public void TileStartPosition(Tile tile)//set tile position based on column because row is always 0 at start and they will fall down
    {
        Vector3 startPosition = levelManager.GetTilePosition(0, tile.column);
        tile.transform.position = startPosition;
    }
}
