using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class LevelFactory
{
    private LevelManager levelManager;
    private bool[] noObstacle;
    private Dictionary<TileType, Sprite> tileSpritesDict = new Dictionary<TileType, Sprite>();
    private Dictionary<int, int> columnTileCount = new Dictionary<int, int>();

    public LevelFactory(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public Level CreateLevel(LevelBuildData levelData)
    {
        Level level = new Level(levelData);
        columnTileCount.Clear();
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
        noObstacle = new bool[totalCells];
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
            noObstacle[i] = true;
            BackgroundType backgroundType = Util.GetRandomInArray(level.backgroundTypesAllowed);
            bgCell.GetComponent<Image>().sprite = SpriteManager.Instance.GetRepeatingSpriteBackground(backgroundType, i);
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
            ObstacleType obstacleType = Util.GetRandomInArray(level.obstacleTypesAllowed);
            obstacle.GetComponent<Image>().sprite = SpriteManager.Instance.GetRandomSpriteObstacle(obstacleType);
            level.bgAndObstacles[i] = obstacle;
            noObstacle[i] = false;
        }
        return level;
    }
    public Level CreateLevelTiles(Level level)
    {
        SetTileSpritesDict(level);//set tile spprite dictionary based on level data

        int totalCells = level.rowCount * level.columnCount;
        level.tiles = new GameObject[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            if (!noObstacle[i]) continue;

            int row = i / level.columnCount;
            int column = i % level.columnCount;
            TileType tileType = Util.GetRandomInArray(level.tileTypesAllowed);
            TilePower tilePower = TilePower.Normal;//implement tile power later
            Tile tile = CreateTile(level, tileType, tilePower);
            tile.NewCoordinates(row, column);
            TileStartPosition(tile);

            tile.sprite = tileSpritesDict[tile.type];
            tile.GetComponent<Image>().sprite = tile.sprite;
            level.tiles[i] = tile.gameObject;
        }


        return level;
    }
    public Tile CreateTile(Level level, TileType type, TilePower tilePower)
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
        tile.type = type;
        tile.power = tilePower;
        tile.sprite = tileSpritesDict[type];
        tileGO.GetComponent<Image>().sprite = tile.sprite;
        return tile;
    }

    public void TileStartPosition(Tile tile)//set tile position to final grid location
    {
        GridLayoutGroup backgroundGrid = LevelManager.Instance.backgroundGrid;
        
        // Calculate position manually using grid layout properties
        float cellWidth = backgroundGrid.cellSize.x;
        float cellHeight = backgroundGrid.cellSize.y;
        float spacingX = backgroundGrid.spacing.x;
        float spacingY = backgroundGrid.spacing.y;
        
        // Calculate total grid dimensions
        int columns = backgroundGrid.constraintCount;
        int rows = backgroundGrid.transform.childCount / columns; // Calculate actual rows
        
        float totalWidth = (columns * cellWidth) + ((columns - 1) * spacingX);
        float totalHeight = (rows * cellHeight) + ((rows - 1) * spacingY);
        
        // Calculate tile position relative to grid center
        float xPos = (tile.column * (cellWidth + spacingX)) - (totalWidth * 0.5f) + (cellWidth * 0.5f);
        float yPos = (totalHeight * 0.5f) - (tile.row * (cellHeight + spacingY)) - (cellHeight * 0.5f);
        
        // Set the position directly
        tile.transform.localPosition = new Vector3(xPos, yPos, 0f);
    }

    public void SetTileSpritesDict(Level level)//set tile sprite based on level data
    {
        foreach (var tiletype in level.tileTypesAllowed)
        {
            Sprite sprite = SpriteManager.Instance.GetRandomSpriteTileType(tiletype);
            //store sprites in a dictionary for easy access later
            tileSpritesDict[tiletype] = sprite;
        }
    }
}
